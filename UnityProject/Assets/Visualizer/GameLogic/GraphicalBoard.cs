using System;
using UnityEngine;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    [Serializable()]
    // Wrapper around a Board, implements the Graphics specific code so it can be rendered
    public class GraphicalBoard : Board 
    {
        [NonSerialized]
        private Board _boardCopy; // would contain a saved version of the map before the agent started cleaning
        
        // Map Telemetry
        protected override int DirtyTiles 
        {
            get => _dirtyTiles;
            set {  _dirtyTiles = value; SendTelemetry(); }
        }

        [NonSerialized]
        private MapTelemetry _telemetry = new MapTelemetry(); // reused, to send telemetry

        public GraphicalBoard(int sizeX, int sizeZ): base(sizeX , sizeZ, false)
        {
            DoOnAllGridEntries(((i, j) => new GraphicalTile(PrefabContainer.Instance.mapReference.transform, i, j)));
            HookDelegates();
            Init();
        }

        public GraphicalBoard(Board board) : base( board.sizeX , board.sizeZ , false )
        {
            // create the tile grid from the grid of states
            DoOnAllGridEntries(((i, j) => new GraphicalTile(PrefabContainer.Instance.mapReference.transform, board.Grid[i, j])));
            HookDelegates();
            Init();
        }
        
        private void LoadMapState()
        {
            if (_boardCopy != null )
            {
                // TODO: warning , mark on tile is persistent, fix it !!!
                DoOnAllGridEntries((i, j) => Grid[i,j].SetState(_boardCopy.Grid[i,j]));
            
                Init();
            }
        }
        
        private void HookDelegates()
        {
            GameStateManager.Instance.OnSceneReset += LoadMapState;
            GameStateManager.Instance.OnSceneStart += TakeSnapshot;
        }

        private void Init()
        {
            Refresh(); // draw map graphics
            DirtyTiles = GetAllDirtyTiles().Count;
            // Tiles = GetAllTiles().Count;
        }

        public GraphicalTile GetGraphicalTile( int gridX , int gridZ )
        {
            return (GraphicalTile) (GetTile(gridX, gridZ));
        }
        
        public GraphicalTile PointToTile( Vector3 point )
        {
            // find out on which tile this point lies 
            var xIndex = (int) point.x / 10; // TODO: remove magic numbers !
            var zIndex = (int) point.z / 10;
            
            // Debug.Log("x: " + xIndex + " z: " + zIndex );
            
            return ( GraphicalTile) Grid[xIndex , zIndex];
        }
        
        // gets the neighbor in the specified direction, if it does not exist, returns null
        public Vector3 GetClosestEdgeWorldPos( Vector3 point )
        {
            // assume the point is on the Map
            var tile = PointToTile(point);
            return tile.GetClosestEdgeWorldPos(point);
        }

        public bool isEdgeOnMapBorder( Vector3 edge )
        {
            // if edge has a zero in X or Z or Max value of X or Z then it's on the border
            return (edge.x == 0 || edge.z == 0 || edge.x == (sizeX * 10) || edge.z == (sizeZ * 10)); 
        }

        // restore it as it was just before the agent started cleaning
        
        private void TakeSnapshot()
        {
            _boardCopy = new Board(this);
        }

        private void SendTelemetry()
        {
            _telemetry.DirtyTiles = _dirtyTiles;
            GlobalTelemetryHandler.Instance.UpdateMapTelemetry(_telemetry);
        }

        public override void RemoveAllWalls()
        {
            base.RemoveAllWalls();
            Refresh();
        }
        
        // just for testing
        private void Refresh()
        {
            var graphicalGrid = Grid; // TODO: can we avoid this ? 
            // refresh every tile
            for (var i = 0; i < Grid.GetLength(0); ++i)
            {
                for (var j = 0; j < Grid.GetLength(1); ++j)
                {
                    ((GraphicalTile) graphicalGrid[i, j]).Refresh(); // for the lolz
                }
            }
        }

        public void Destroy() // destroy the current Map, cleans up and destroys all GameObjects related to it
        {
            // unhook from all events
            GameStateManager.Instance.OnSceneReset -= LoadMapState;
            GameStateManager.Instance.OnSceneStart -= TakeSnapshot;
            
            var graphicalGrid = Grid; // TODO: can we avoid this ? 

            // destroy all tiles in grid
            for (var i = 0; i < Grid.GetLength(0); ++i)
            {
                for (var j = 0; j < Grid.GetLength(1); ++j)
                {
                    ((GraphicalTile) graphicalGrid[i, j]).Destroy();
                }
            }
        }
    }
}
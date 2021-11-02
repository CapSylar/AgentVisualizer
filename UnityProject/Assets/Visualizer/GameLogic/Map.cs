using System;
using System.Collections.Generic;
using UnityEngine;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    public class Map
    {
        // the floor, the actual grid of tiles which also contain the position of the walls
        public Tile[,] Grid ;
        // reference to the Agent
        private Agent Agent = null;
        
        public int sizeX , sizeZ; // not actual units, just the number of tiles in each direction

        private MapState _savedState; // would contain a saved version of the map before the agent started cleaning
        
        // Map Telemetry
        private int _dirtyTiles;

        private int DirtyTiles
        {
            get => _dirtyTiles;
            set {  _dirtyTiles = value; SendTelemetry(); }
        }

        private MapTelemetry _telemetry = new MapTelemetry(); // reused, to send telemetry

        public Map(int sizeX, int sizeZ)
        {
            Grid = new Tile[sizeX,sizeZ];
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;

            DoOnAllGridEntries(((i, j) => Tile.CreateTile(PrefabContainer.Instance.mapReference.transform, i, j)));
            HookDelegates();
            Init();
        }

        public Map(MapState mapState)
        {
            // create the tile grid from the grid of states
            var stateGrid = mapState.stateGrid;
            
            Grid = new Tile[stateGrid.GetLength(0), stateGrid.GetLength(1)];
            sizeX = stateGrid.GetLength(0);
            sizeZ = stateGrid.GetLength(1);
            
            DoOnAllGridEntries(((i, j) => Tile.CreateTile(PrefabContainer.Instance.mapReference.transform, i, j, stateGrid[i, j])));
            HookDelegates();
            Init();
        }
        
        private void LoadMapState()
        {
            var stateGrid = _savedState.stateGrid;
            DoOnAllGridEntries((i, j) => Grid[i,j].SetState(stateGrid[i,j]));
            
            Init();
        }

        private void DoOnAllGridEntries(Func<int, int, Tile> lambda )
        {
            for (var i = 0; i < Grid.GetLength(0); ++i)
            for (var j = 0; j < Grid.GetLength(1); ++j)
            {
                // create Tile with loaded state
                Grid[i, j] = lambda(i, j);
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
        }

        // public void PlaceWall( int tileX , int tileY , TILE_EDGE edge )
        // {
        //     var referenceTile = Grid[tileX, tileY];
        //     referenceTile.SetWall(edge, true);
        //     // update the adjacent tile, it has the wall in the opposite direction
        //     
        //     var opposite = edge.GetOpposite(); 
        //     
        //     //TODO: can this be written in a better way?
        //     switch (edge)
        //     {
        //         case TILE_EDGE.UP:
        //             Grid[tileX, tileY + 1].SetWall(opposite , true);
        //             break;
        //         case TILE_EDGE.DOWN:
        //             Grid[tileX, tileY - 1].SetWall(opposite , true);
        //             break;
        //         case TILE_EDGE.RIGHT:
        //             Grid[tileX+1, tileY].SetWall(opposite , true);
        //             break;
        //         case TILE_EDGE.LEFT:
        //             Grid[tileX-1, tileY].SetWall(opposite , true);
        //             break;
        //     }
        // }

        public void  SetActiveAgent( Agent agent )
        {
            this.Agent = agent; 
        }
        
        
        public Tile PointToTile( Vector3 point )
        {
            // find out on which tile this point lies 
            var xIndex = (int) point.x / 10; // TODO: remove magic numbers !
            var zIndex = (int) point.z / 10;
            
            // Debug.Log("x: " + xIndex + " z: " + zIndex );
            
            return Grid[xIndex , zIndex];
        }

        public int Manhattan( Tile tile1 , Tile tile2 )
        {
            return Math.Abs(tile1.GridX - tile2.GridX) + Math.Abs(tile1.GridZ - tile2.GridZ);
        }

        public List<Tile> GetAllDirtyTiles()
        {
            //TODO: speed this up, we could keep track of all the dirty tiles
            var list = new List<Tile>();

            for (var i = 0; i < Grid.GetLength(0); ++i)
            for (var j = 0; j < Grid.GetLength(1); ++j)
            {
                if ( Grid[i,j].IsDirty )
                    list.Add(Grid[i,j]);
            }

            return list;
        }

        // set all tiles to clean
        public void MopTheFloor()
        {
            DoOnAllGridEntries((i, j) => SetTileDirt(Grid[i, j] , false));
        }
        
        // destroy all walls
        public void RemoveAllWalls()
        {
            // calling remove all walls on Tiles is ok as long as we do it on all of them at once,
            // doing it on individual tiles creates inconsistencies in the Map
            
            DoOnAllGridEntries((i, j) => Grid[i,j].RemoveAllWalls());
        }

        public List<Tile> GetReachableNeighbors( Tile tile )
        {
            // get the neighbors such that no walls exist in between
            var neighbors = new List<Tile>();

            for (var direction = 0; direction < 4; ++direction) // iterate over all directions
            {
                Tile temp;
                // if does not have a wall in this direction and indeed has a neighbor ( not on the border ) 
                if (!tile.HasWall((TILE_EDGE) direction) && (temp = GetNeighbor(tile, (TILE_EDGE) direction)) != null)
                {
                    neighbors.Add(temp);
                }
            }
            
            return neighbors;
        }

        public bool IsTileWallOnEdge( Tile tile , TILE_EDGE direction )
        {
            return (GetNeighbor(tile, direction) == null);
        }
        
        public void SetTileWall(Tile tile, TILE_EDGE direction , bool state )
        {
            // each tile is only responsible for the upper and right walls
            // if we want a lower wall on the current tile we have to assign the upper on the tile below
            tile.SetWall(direction , state);
            
            // walls are between two tiles, set the other tile that wasn't directly selected
            GetNeighbor(tile , direction).SetWall(direction.GetOpposite(), state );
        }

        public Tile SetTileDirt(Tile tile, bool isDirty)
        {
            if (tile.IsDirty != isDirty) // if state really changed
            {
                tile.IsDirty = isDirty;
                DirtyTiles += isDirty ? 1 : -1;
            }
            
            return tile;
        }

        public Tile GetTile( int gridX , int gridZ )
        {
            if (gridX >= 0 && gridX < sizeX && gridZ >= 0 && gridZ < sizeZ)
                return Grid[gridX, gridZ];
            return null;
        }

        public Tile GetLeft(Tile tile)
        {
            return ( tile.GridX > 0 ) ? Grid[tile.GridX-1,tile.GridZ] : null;
        }

        public Tile GetRight ( Tile tile )
        {
            return ( tile.GridX < sizeX-1 ) ? Grid[tile.GridX+1,tile.GridZ] : null;
        }

        public Tile GetUp(Tile tile)
        {
            return (tile.GridZ < sizeZ-1) ? Grid[tile.GridX,tile.GridZ+1] : null;
        }

        public Tile GetDown(Tile tile)
        {
            return (tile.GridZ > 0) ? Grid[tile.GridX,tile.GridZ-1] : null;
        }

        // gets the neighbor in the specified direction, if it does not exist, returns null
        public Tile GetNeighbor ( Tile tile , TILE_EDGE direction)
        {
            Tile neighbor = null;
            
            switch (direction)
            {
                case TILE_EDGE.UP:
                    neighbor = GetUp(tile);
                    break;
                case TILE_EDGE.RIGHT:
                    neighbor = GetRight(tile);
                    break;
                case TILE_EDGE.DOWN:
                    neighbor = GetDown(tile);
                    break;
                case TILE_EDGE.LEFT:
                    neighbor = GetLeft(tile);
                    break;
            }

            return neighbor;
        }

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
            _savedState = new MapState(this);
        }

        private void SendTelemetry()
        {
            _telemetry.DirtyTiles = _dirtyTiles;
            GlobalTelemetryHandler.Instance.UpdateMapTelemetry(_telemetry);
        }
        
        // just for testing
        private void Refresh()
        {
            // refresh every tile
            for (int i = 0; i < Grid.GetLength(0); ++i)
            {
                for (int j = 0; j < Grid.GetLength(1); ++j)
                {
                    Grid[i,j].Refresh(); // for the lolz
                }
            }
        }

        public void Destroy() // destroy the current Map, cleans up and destroys all GameObjects related to it
        {
            // destroy all tiles in grid
            for (var i = 0; i < Grid.GetLength(0); ++i)
            {
                for (var j = 0; j < Grid.GetLength(1); ++j)
                {
                    Grid[i,j].Destroy();
                }
            }
        }
    }
}

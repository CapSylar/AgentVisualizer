using System;
using System.Collections;
using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;
using Visualizer.UI;

namespace Visualizer.AgentBrains.GoodBrains
{
    public class TspNearestNeighborFullVisibility : BaseBrain
    {
        private Board _currentBoard;
        private Agent _actor;
        
        // for Brain Telemetry
        private List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();
        private int _globalPathLength = 0;

        public int GlobalPathLength
        {
            get => _globalPathLength;
            private set
            {
                _globalPathLength = value;
                SendTelemetry(_globalPathLength);
            }
        }

        public TspNearestNeighborFullVisibility( Board board )
        {
            _currentBoard = board;

            _messages.Add(new BrainMessageEntry( "global path length:" , "" ));
        }

        private IEnumerator GenerateGlobalPath()
        {
            Tile currentTile = _actor.CurrentTile;
            var dirtyTiles = _currentBoard.GetAllDirtyTiles();

            GlobalPathLength = 0;

            while (dirtyTiles.Count > 0)
            {
                GlobalPathLength +=
                    GetPathToNearestNeighbor(_currentBoard, dirtyTiles, currentTile, Commands, out var closestTile);
                currentTile = closestTile; // start position for next iteration is the current closest Dirt Tile

                //TODO: use index, runs in O(N) now!!!!
                dirtyTiles.Remove(closestTile); // so it won't be picked again
            }

            yield return null;
        }

        public static int GetPathToNearestNeighbor( Board graphicalBoard , List<Tile> dirtyTiles , Tile start , Queue<AgentMove> commands , out Tile closestTile )
        {
            // find closest tile to currentTile
            closestTile = GetNearestDirty(graphicalBoard, dirtyTiles, start);
                
            // found the closest tile
            // get the path to it
            Bfs.DoNormalBfs( graphicalBoard , start , closestTile , out var path );

            PathToMoveCommands( path , commands );
            commands.Enqueue(new CleanDirtMove(closestTile));

            return path.Count;
        }

        public static Tile GetNearestDirty(  Board graphicalBoard , List<Tile> dirtyTiles  , Tile startGraphicalTile )
        {
            // assumes the list if not empty
            // find closest tile to currentTile
            var minIndex = 0;
            var minDistance = Int32.MaxValue;
                
            for (var i = 0; i < dirtyTiles.Count; ++i)
            {
                var temp = graphicalBoard.BfsDistance(startGraphicalTile, dirtyTiles[i]);
                if (minDistance > temp )
                {
                    minDistance = temp;
                    minIndex = i;
                }
            }

            return dirtyTiles[minIndex];
        }
        
        public override void Start(Agent actor)
        {
            // Init telemetry
            GlobalPathLength = 0; // sends telemetry
            this._actor = actor;

            // start path generation
            
            //TODO: find a solution to this
            PrefabContainer.Instance.StartCoroutine(GenerateGlobalPath());
        }

        public new void Reset()
        {
            base.Reset();
            // reset telemetry
            GlobalTelemetryHandler.Instance.DestroyBrainTelemetryFields();
        }
        
        private void SendTelemetry( int value )
        {
            _messages[0].value = "" + value;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }
    }
}
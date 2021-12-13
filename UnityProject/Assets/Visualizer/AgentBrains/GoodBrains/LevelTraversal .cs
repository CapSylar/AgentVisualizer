using System;
using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains.GoodBrains
{
    public class LevelTraversal : BaseBrain
    {
        // state
        private Board _currentGraphicalBoard;
        private List<Tile> _tiles;
        private GraphicalTile _lastCleaned = null;
        
        public LevelTraversal( Board graphicalBoard )
        {
            _currentGraphicalBoard = graphicalBoard;
        }

        private void GenerateGlobalPath()
        {
            _tiles = _currentGraphicalBoard.GetAllTiles();
            var numLocalPaths = _tiles.Count;
            
            for (int i = 0; i <  numLocalPaths ; ++i)
            {
                GenerateLocalPath();
            }
        }

        private void GenerateLocalPath()
        {
            // generate the complete path the agent must follow to completely clean the map
            
            // do BFS to the closest tile
            // use manhattan distance ( not accurate since we could have walls in between )
            // get the closest Tile using manhattan
            // nothing cleaned yet, we are still at the start point, use agent position, else use last cleaned tile
            // as starting point, this is where the agent will be at
            var currentTile = (_lastCleaned == null) ? AttachedAgent.CurrentTile : _lastCleaned ;
            
            var min = Int32.MaxValue;
            //var min = 0;
            Tile closestTile = null;
            
            // choose closest tile to clean
            foreach (var Tile in _tiles)
            {
                //TODO: remove manhattan and use BFS itself to get the actual correct distance
                var dist = _currentGraphicalBoard.ManhattanDistance(currentTile, Tile);
                if (min > dist)
                {
                    closestTile = Tile;
                    min = dist;
                }
            }

            _tiles.Remove(closestTile); // remove it so it won't be picked again
            
            // Do BFS to the closestTile
            Bfs.DoNormalBfs( _currentGraphicalBoard , currentTile , closestTile ,  out var path );
            
            // convert path to commands
            path.RemoveAt(0); // agent would be on this tile already,
            // Bfs returns it for correctness
            // add all to command list
            
            //TODO: fix level traversal down below 
            // foreach (var tile in path)
            // {
            //     Commands.Enqueue(new GoAction(tile));
            // }
            //
            // if(closestTile.isDirty)
            // {
            //     Commands.Enqueue(new CleanDirtAction(closestTile)); 
            // }
            //
            // _lastCleaned = closestTile; // used as starting point for next pass if any
           
        }

        public override void Start( Agent agent )
        {
            GenerateGlobalPath();
        }

        public override void Reset()
        {
            // reset state variables
            base.Reset();
            _lastCleaned = null;
        }
    }
}

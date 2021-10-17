using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains
{
    public class BfsToClosestTile : BaseBrain
    {
        private Map currentMap;
        private int GridX, GridZ;
        private Queue<AgentAction> commands;
        
        // state

        private List<Tile> _dirtyTiles;
        private Tile _lastCleaned = null;
        
        public BfsToClosestTile( Map map )
        {
            currentMap = map;
            commands = new Queue<AgentAction>();
        }

        private void GenerateGlobalPath()
        {
            _dirtyTiles = currentMap.GetAllDirtyTiles();
            var numLocalPaths = _dirtyTiles.Count;
            
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
            Tile closestTile = null;
            
            foreach (var dirtyTile in _dirtyTiles)
            {
                var dist = currentMap.Manhattan(currentTile, dirtyTile);
                if (min > dist)
                {
                    closestTile = dirtyTile;
                    min = dist;
                }
            }

            _dirtyTiles.Remove(closestTile); // remove it so it won't be picked again
            
            // Do BFS to the closestTile 
            List<Tile> path;
            DoBfs( currentTile , closestTile , out path );
            
            // convert path to commands
            
            // add all to command list
            foreach (var tile in path)
            {
                commands.Enqueue(new GoAction(tile));
            }
            
            commands.Enqueue(new CleanDirtAction(closestTile));
            _lastCleaned = closestTile; // used as stating point for next pass if any
        }

        private void DoBfs( Tile start , Tile end , out List<Tile> path )
        {
            path = new List<Tile>();
            
            // do BFS to get the path to the dirty tile
            // we can't navigate directly because walls could be present

            var explored = new HashSet<Tile>();
            var parent = new Dictionary<Tile, Tile>(); // child , parent mapping
            var queue = new Queue<Tile>();
            
            queue.Enqueue(start);
            parent.Add(start,null); // no parent for first tile
            explored.Add(start);

            while (queue.Count > 0)
            {
                var tile = queue.Dequeue();
                if (tile == end)
                {
                    break; // found it!!
                }

                var neighbors = currentMap.GetReachableNeighbors(tile);
                foreach (var neighbor in neighbors.Where(neighbor => !explored.Contains(neighbor)))
                {
                    explored.Add(neighbor);
                    queue.Enqueue(neighbor);
                    // we explored them from the current tile
                    parent.Add(neighbor, tile);
                }
            }
            
            // get the path
            var pathEnd = end;
            
            for (;;)
            {
                var temp = parent[pathEnd];
                path?.Add(pathEnd);
                if (temp == null)
                    break;
                pathEnd = temp;
            }

            path?.Reverse(); // we got the path in reverse, reverse it!
        }

        public override AgentAction GetNextDest()
        {
            if (commands.Count > 0)
                return commands.Dequeue();

            return null;
        }

        public override void Start()
        {
            GenerateGlobalPath();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
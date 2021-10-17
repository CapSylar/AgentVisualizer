using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains
{
    public class BfsToClosestTile : BaseBrain
    {
        private Map currentMap;
        private int GridX, GridZ;
        private Queue<AgentAction> commands;
        
        public BfsToClosestTile( Map map , int gridX = 0  , int gridZ = 0 )
        {
            currentMap = map;
            GridX = gridX;
            GridZ = gridZ;
        }

        private void DoBfs()
        {
            // do BFS to the closest tile
            // use manhattan distance ( not accurate since we could have walls in between )
            var dirtyTiles = currentMap.GetAllDirtyTiles();

            // get the closest Tile using manhattan
            var currentTile = currentMap.GetTile(GridX, GridZ);
            int min = Int32.MaxValue;
            Tile closestTile = null ;
            
            foreach (var dirtyTile in dirtyTiles)
            {
                var dist = currentMap.Distance(currentTile, dirtyTile);
                if (min > dist)
                {
                    closestTile = dirtyTile;
                    min = dist;
                }
            }
            
            // do BFS to get the path to the dirty tile
            // we can't navigate directly because walls could be present

            var explored = new HashSet<Tile>();
            var parent = new Dictionary<Tile, Tile>(); // child , parent mapping
            var queue = new Queue<Tile>();
            
            queue.Enqueue(currentTile);
            parent.Add(currentTile,null); // no parent for first tile
            explored.Add(currentTile);

            while (queue.Count > 0)
            {
                var tile = queue.Dequeue();
                if (tile == closestTile)
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
            var path = new List<Tile>();
            
            for (;;)
            {
                var temp = parent[closestTile];
                path.Add(closestTile);
                if (temp == null)
                    break;
                closestTile = temp;
            }

            path.Reverse(); // we got path in reverse, reverse it
            
            // create commands list
            
            commands = new Queue<AgentAction>();

            foreach (var tile in path)
            {
                commands.Enqueue(new GoAction(tile));
            }
        }

        public override AgentAction GetNextDest()
        {
            if (commands.Count > 0)
                return commands.Dequeue();

            return null;
        }

        public override void Start()
        {
            DoBfs();
        }

        public override void Pause()
        {
            throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }
    }
}
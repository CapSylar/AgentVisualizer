using System.Collections.Generic;
using System.Linq;
using Visualizer.GameLogic;

namespace Visualizer.Algorithms
{
    public static class Bfs
    {
        // perform Breadth First Search
        public static void DoBfs( Map map , Tile start , Tile end , out List<Tile> path)
        {
            path = new List<Tile>();
            
            // do BFS to get the path to the dirty tile
            // we can't navigate directly because walls could be present

            var explored = new HashSet<Tile>();
            var parent = new Dictionary<Tile, Tile>(); // child => parent mapping
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

                var neighbors = map.GetReachableNeighbors(tile);
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
    }
}

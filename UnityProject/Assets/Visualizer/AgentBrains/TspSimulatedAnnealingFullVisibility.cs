using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Random = System.Random;

namespace Visualizer.AgentBrains
{
    public class TspSimulatedAnnealingFullVisibility : BaseBrain
    {
        private Map currentMap;
        private Queue<AgentAction> commands;
        
        // state
        
        public TspSimulatedAnnealingFullVisibility( Map map )
        {
            currentMap = map;
            commands = new Queue<AgentAction>();
        }

        private void GenerateGlobalPath()
        {
            var dirtyTiles = currentMap.GetAllDirtyTiles();
            // use indices of dirt tiles in list to access adjacency matrix

            var distances = new int[dirtyTiles.Count, dirtyTiles.Count];

            for (int row = 0; row < dirtyTiles.Count; ++row)
            {
                for (int col = 0; col < dirtyTiles.Count; ++col)
                {
                    if (row == col) // distance to itself
                    {
                        distances[row, col] = 0;
                        continue;
                    }

                    List<Tile> path;
                    //TODO: make a more efficient version of DoBfs just for this
                    Bfs.DoBfs(currentMap, dirtyTiles[row] , dirtyTiles[col] , out path);
                    distances[row, col] = path.Count;
                }
            }
            
            // generate a default condifuration
            var oldConfig = new TspConfiguration(dirtyTiles);
            oldConfig.Shuffle();

            double temp = 1;
            double coolingRate = 0.01f; // gives good results!

            Random rnd = new Random();

            while (temp > 0.01f)
            {
                var newConfig = oldConfig.GetSimilarConfiguration();

                var oldDistance = oldConfig.GetRouteLength( distances , dirtyTiles );
                var newDistance = newConfig.GetRouteLength(distances, dirtyTiles);

                var rand = rnd.NextDouble();
                if (newDistance <= oldDistance && Math.Exp((oldDistance - newDistance)/temp) > rand )
                    oldConfig = newConfig; // take it!
                
                Debug.Log("Configuration distance for now: " + oldConfig.GetRouteLength(distances , dirtyTiles ));

                temp *= ( 1 - coolingRate );
            }

            Tile lastVisited = null;
            foreach (var city in oldConfig.Route)
            {
                // get the Local route using BFS
                List<Tile> localRoute;
                Bfs.DoBfs(currentMap, lastVisited == null
                    ? AttachedAgent.CurrentTile
                    : lastVisited, city, out localRoute);
                lastVisited = city;

                foreach (var tile in localRoute)
                {
                    commands.Enqueue(new GoAction(tile));
                }

                commands.Enqueue(new CleanDirtAction(city));
            }
        }

        public override AgentAction GetNextDest()
        {
            return commands.Count > 0 ? commands.Dequeue() : null;
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
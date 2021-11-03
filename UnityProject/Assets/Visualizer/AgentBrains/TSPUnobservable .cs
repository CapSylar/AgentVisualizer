using System;
using System.Collections;
using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.UI;
using Random = System.Random;

namespace Visualizer.AgentBrains
{
    public class TSPUnobservable : BaseBrain
    {
        private Map currentMap;
        private Agent actor;
        
        // Brain Telemetry
        private List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();

        public TSPUnobservable( Map map )
        {
            currentMap = map;
            
            _messages.Add(new BrainMessageEntry( "global path length:" , "" ));
        }

        private IEnumerator GenerateGlobalPath( double coolingRate )
        {
            var dirtyTiles = currentMap.GetAllTiles();
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
                    
                    distances[row, col] = currentMap.BfsDistance(dirtyTiles[row] , dirtyTiles[col]);
                }
            }
            
            // generate a default configuration
            var oldConfig = new TspConfiguration(dirtyTiles);
            oldConfig.Shuffle();

            double temp = 1;

            Random rnd = new Random();

            while (temp > 0.01f)
            {
                var newConfig = oldConfig.GetSimilarConfiguration();

                var oldDistance = oldConfig.GetRouteLength( distances , dirtyTiles );
                var newDistance = newConfig.GetRouteLength(distances, dirtyTiles);

                var rand = rnd.NextDouble();
                if (newDistance <= oldDistance && Math.Exp((oldDistance - newDistance)/temp) > rand )
                    oldConfig = newConfig; // take it!
                
                // Debug.Log("Configuration distance for now: " + oldConfig.GetRouteLength(distances , dirtyTiles ));

                temp *= ( 1 - coolingRate );

                SendTelemetry( oldConfig.GetRouteLength( distances , dirtyTiles ) );
                yield return null; // wait till next frame 
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
                    Commands.Enqueue(new GoAction(tile));
                }
                if(lastVisited.IsDirty){
                Commands.Enqueue(new CleanDirtAction(city));
                }
            }
            
            IsReady = true; // brain ready to be used
        }

        private void SendTelemetry( int distance )
        {
            _messages[0].value = "" + distance;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }

        public override AgentAction GetNextAction()
        {
            return Commands.Count > 0 ? Commands.Dequeue() : null;
        }

        public override void Start( Agent actor )
        {
            // Init telemetry
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
            this.actor = actor;
            
            // set up PopupWindow and callbacks 
            var x = new List<Tuple<string, Func<string, bool>>>();
            x.Add(new Tuple<string, Func<string, bool>>("cooling Rate" , s =>
            {
                var value = Double.Parse(s);
                return value > 0 && value < 1;
            } ));

            new PopUpHandler(x, Callback);
        }

        private void Callback(List<string> results )
        {
            // start routing
            actor.StartCoroutine(GenerateGlobalPath(Double.Parse(results[0])));
        }

        public override void Reset()
        {
            base.Reset();
            // reset telemetry
            GlobalTelemetryHandler.Instance.DestroyBrainTelemetryFields();
        }
    }
}

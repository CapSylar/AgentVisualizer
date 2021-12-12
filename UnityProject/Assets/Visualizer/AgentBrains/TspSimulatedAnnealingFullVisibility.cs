using System;
using System.Collections;
using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer.AgentBrains
{
    public class TspSimulatedAnnealingFullVisibility : BaseBrain
    {
        private Map currentMap;
        private Agent _actor;

        // Brain Telemetry
        private List<BrainMessageEntry> _messages = new List<BrainMessageEntry>();
        private static int TELEMETRY_UPDATE_LOOP = 3000 ; // send telemetry after every * 

        public TspSimulatedAnnealingFullVisibility( Map map )
        {
            currentMap = map;
            _messages.Add(new BrainMessageEntry( "current path:" , "" ));
        }

        private IEnumerator GenerateGlobalPath( double coolingRate )
        {
            var cities = new Dictionary<Tile, int>();
            
            var dirtyTiles = currentMap.GetAllDirtyTiles();
            dirtyTiles.Insert(0,_actor.CurrentTile);

            for (var i = 0; i < dirtyTiles.Count; ++i)
            {
                cities.Add(dirtyTiles[i] , i); // +1 since we already inserted the start city
            }

            // use indices of dirt tiles in list to access adjacency matrix
            var distances = new int[cities.Count, cities.Count];

            for (var row = 0; row < cities.Count; ++row)
            {
                for (var col = 0; col < cities.Count; ++col)
                {
                    if (row == col) // distance to itself
                    {
                        distances[row, col] = 0;
                        continue;
                    }

                    distances[row, col] = currentMap.BfsDistance(dirtyTiles[row], dirtyTiles[col]);
                }
            }
            
            // generate a default configuration
            var oldConfig = new TspConfiguration( new List<Tile>(dirtyTiles) );
            oldConfig.Shuffle(1, oldConfig.GetRouteCityCount() ); // don't change position of first city which is the agent position

            double temp = 1;

            var rnd = new Random();

            var loops = 0;
            
            while (temp > 0.005f)
            {
                if (loops == TELEMETRY_UPDATE_LOOP)
                {
                    loops = 0;
                    SendTelemetry( oldConfig.GetRouteLength( distances , cities ) );
                    yield return null; // skip till next frame
                }
                
                var newConfig = oldConfig.GetSimilarConfiguration();

                var oldDistance = oldConfig.GetRouteLength( distances , cities );
                var newDistance = newConfig.GetRouteLength(distances, cities);

                var rand = rnd.NextDouble();

                if (newDistance <= oldDistance || Math.Exp((oldDistance - newDistance)/temp) > rand )
                    oldConfig = newConfig; // take it!

                temp *= ( 1 - coolingRate );
                ++loops;
            }
            
            Tile lastVisited = null;
            
            // send it once again on exit
            SendTelemetry( oldConfig.GetRouteLength( distances , cities ) );
            
            // convert calculations into agent actions
            foreach (var city in oldConfig.Route)
            {
                // get the Local route using BFS
                Bfs.DoBfs(currentMap, lastVisited == null
                    ? AttachedAgent.CurrentTile
                    : lastVisited, city, out var localRoute);
                lastVisited = city;

                localRoute.RemoveAt(0); // current tile not accounted for

                foreach (var tile in localRoute)
                {
                    Commands.Enqueue(new GoAction(tile));
                }

                Commands.Enqueue(new CleanDirtAction(city));
            }

            yield return null ;
        }

        private void SendTelemetry( int distance )
        {
            _messages[0].value = "" + distance;
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
        }

        public override void Start( Agent actor )
        {
            // Init telemetry
            GlobalTelemetryHandler.Instance.UpdateBrainTelemetry(_messages);
            _actor = actor;
            
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
            _actor.StartCoroutine(GenerateGlobalPath(Double.Parse(results[0])));
        }

        public override void Reset()
        {
            base.Reset();
            // reset telemetry
            GlobalTelemetryHandler.Instance.DestroyBrainTelemetryFields();
        }
    }
}
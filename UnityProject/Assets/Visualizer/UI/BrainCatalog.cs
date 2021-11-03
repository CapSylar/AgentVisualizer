using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Visualizer.AgentBrains;

namespace Visualizer.UI
{
    public static class BrainCatalog
    {
        // good enough for now at least
        private static readonly Dictionary<string, Type> Map = new Dictionary<string, Type>()
        {
            {"TSP-Simulated Annealing" , typeof(TspSimulatedAnnealingFullVisibility)},
            {"TSP-Nearest Neighbor" , typeof(TspNearestNeighborFullVisibility)},
            {"Unobservable BFS", typeof(LevelTraversal)},
            {"Unobservable TSP", typeof(TSPUnobservable)},
        };

        public static List<string> GetAllBrainNames()
        {
            var brains = new List<string>();
            foreach ( var name in Map.Keys )
            {
                brains.Add(name);
            }

            return brains;
        }
        
        public static Type NameToBrain(string name)
        {
            return Map[name];
        }
    }
}
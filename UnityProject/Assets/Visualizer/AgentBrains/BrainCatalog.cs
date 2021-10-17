using System;
using System.Collections.Generic;

namespace Visualizer.AgentBrains
{
    public static class BrainCatalog
    {
        // good enough for now at least
        private static readonly Dictionary<string, Type> Map = new Dictionary<string, Type>()
        {
            {"BFS to closest Dirt" , typeof(BfsToClosestTile) },
            {"TSP-Simulated Annealing" , typeof(TspSimulatedAnnealingFullVisibility)}
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
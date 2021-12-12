using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.AgentBrains.EvilBrains;
using Visualizer.AgentBrains.GoodBrains;
using Visualizer.AgentBrains;

namespace Visualizer.UI.Catalogs
{
    public static class BrainCatalog
    {
        // good enough for now at least
        private static readonly Dictionary<string, Type> GoodAlgorithms = new Dictionary<string, Type>()
        {
            {"BFS-Clean Closest Dynamic" , typeof(BfsCleanNearestTileDynamic)},
            {"Maximizer-Minimax dirt placer" , typeof(MaximizerMinimaxFullVisibility)},
            {"TSP-Nearest Neighbor" , typeof(TspNearestNeighborFullVisibility)},
            {"TSP-Simulated Annealing" , typeof(TspSimulatedAnnealingFullVisibility)},
            {"BFS-LD-Partial Visibility" , typeof(DfsPartialVisibility)},
            {"Dfs-No Visibility" , typeof(DfsNoVisibility)},
            {"Unobservable BFS", typeof(LevelTraversal)},
            {"Genetics", typeof(TspGeneticFullVisibility)},
        };

        private static readonly Dictionary<string, Type> EvilAlgorithms = new Dictionary<string, Type>()
        {
            {"BFS-Stain Closest", typeof(BfsStainClosestTileFullVisibility)},
            {"Minimizer-Minimax dirt placer" , typeof(MinimizerMinimaxFullVisibility)}
        };

        public static List<string> GetAllGoodBrainNames()
        {
            return GoodAlgorithms.Keys.ToList();
        }

        public static List<string> GetAllEvilBrainNames()
        {
            return EvilAlgorithms.Keys.ToList();
        }
        
        public static Type GetGoodBrain (string name)
        {
            return GoodAlgorithms[name];
        }

        public static Type GetEvilBrain(string name)
        {
            return EvilAlgorithms[name];
        }
    }
}
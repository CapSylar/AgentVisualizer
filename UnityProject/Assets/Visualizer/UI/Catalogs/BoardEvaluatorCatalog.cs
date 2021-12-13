using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.Algorithms.BoardEvaluation;

namespace Visualizer.UI.Catalogs
{
    public static class BoardEvaluatorCatalog
    {
        private static readonly Dictionary<string, Type> BoardEvaluators = new Dictionary<string, Type>()
        {
            {"Clean Count" , typeof(CleanCountEvaluator)},
            {"Clean and POIs in Vicinity" , typeof(CleanAndPoIinVicinityEvaluator)},
            {"Clean and distance to POIs" , typeof(CleanAndDistanceToPoiEvaluator)},
        };

        public static List<string> GetAllBoardEvaluators()
        {
            return BoardEvaluators.Keys.ToList();
        }

        public static Type GetBoardEvaluator(string name)
        {
            return BoardEvaluators[name];
        }
        
    }
}
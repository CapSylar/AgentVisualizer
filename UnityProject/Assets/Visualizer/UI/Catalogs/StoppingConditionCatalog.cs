using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.GameLogic.Conditions;

namespace Visualizer.UI.Catalogs
{
    public static class StoppingConditionCatalog
    {
        private static readonly Dictionary<string, Type> StoppingConditions = new Dictionary<string, Type>()
        {
            {"No Stopping Condition" , typeof(NoStoppingCondition)},
            {"Round limit", typeof(RoundLimitStoppingCondition)}
        };

        public static List<string> GetAllStoppingConditions()
        {
            return StoppingConditions.Keys.ToList();
        }

        public static Type GetCondition( string name )
        {
            return StoppingConditions[name];
        }
        
    }
}
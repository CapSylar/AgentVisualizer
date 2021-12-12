using System;
using System.Collections.Generic;
using Visualizer.UI;

namespace Visualizer.GameLogic.Conditions
{
    public class RoundLimitGraphicalConstructor : GraphicalConstructor
    {
        private Action<StoppingCondition> _callback;
        // takes a callback, constructs the condition, then calls the callback with the object
        public override void Construct( Action<StoppingCondition> callback )
        {
            this._callback = callback;
            // create a popupmenu 
            
            // set up PopupWindow and callbacks 
            var x = new List<Tuple<string, Func<string, bool>>>();
            x.Add(new Tuple<string, Func<string, bool>>("Round Limit" , s =>
            {
                var value = int.Parse(s);
                return value > 0 && value < Int32.MaxValue ;
            } ));

            new PopUpHandler(x, Callback);
        }

        private void Callback( List<string> parameters )
        {
            // in this case we only need one parameter
            _callback(new RoundLimitStoppingCondition(int.Parse(parameters[0])));
        }
    }
}
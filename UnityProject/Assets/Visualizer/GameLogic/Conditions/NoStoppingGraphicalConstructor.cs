using System;

namespace Visualizer.GameLogic.Conditions
{
    public class NoStoppingGraphicalConstructor : GraphicalConstructor 
    {
        // No stopping doesn't really need a graphical constructor, but one is provided and used for consistency

        public override void Construct(Action<StoppingCondition> callback)
        {
            callback(new NoStoppingCondition());
        }
    }
}
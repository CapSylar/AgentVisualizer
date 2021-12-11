using System;

namespace Visualizer.GameLogic.Conditions
{
    public abstract class GraphicalConstructor
    {
        public abstract void Construct(Action<StoppingCondition> callback);
    }
}
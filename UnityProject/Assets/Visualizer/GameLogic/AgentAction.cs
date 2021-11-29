using System.Threading;
using Visualizer.GameLogic;

namespace Visualizer.GameLogic
{
    public abstract class AgentAction
    {
        //just does the Action using the current Agent as actor
        public abstract void Do( Agent actor );

        public abstract void Do( GraphicalAgent actor );

        public abstract bool IsDone();
    }
}
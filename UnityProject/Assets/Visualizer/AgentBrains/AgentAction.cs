using System.Threading;

namespace Visualizer.AgentBrains
{
    public abstract class AgentAction
    {
        //just does the Action using the current Agent as actor
        public abstract  void Do( Agent actor );

        public abstract bool IsDone();
    }
}
using System.Threading;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains
{
    public abstract class AgentAction
    {
        //just does the Action using the current Agent as actor
        public abstract  void Do( Agent Actor );

        public abstract bool IsDone();
    }
}
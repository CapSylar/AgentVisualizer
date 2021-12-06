using System.Threading;
using Visualizer.GameLogic;

namespace Visualizer.GameLogic.AgentMoves
{
    public abstract class AgentMove
    {
        //just does the Action using the current Agent as actor
        public abstract void Do( Agent actor );
        public abstract void Do( GraphicalAgent actor );

        // gets the reverse of the Move, clean => soil, go forward => go backward
        public abstract AgentMove GetReverse();
        public abstract bool IsDone();

        public virtual void Reset() { }
    }
}
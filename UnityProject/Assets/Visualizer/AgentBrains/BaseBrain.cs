using System.Collections.Generic;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.AgentBrains
{
    public abstract class BaseBrain
    {
        // reference to the agent the brain is attached to
        protected Agent AttachedAgent;

        // commands queue that the Agent gets its actions from
        protected Queue<AgentMove> Commands = new Queue<AgentMove>();

        public virtual AgentMove GetNextAction()
        {
            return Commands.Count > 0 ? Commands.Dequeue() : null;
        }
        
        public virtual bool HasNextAction()
        {
            return Commands.Count > 0;
        }

        public virtual void Reset() // reset the brain
        {
            // IsReady = false;
            Commands.Clear();
        }
        
        // Child Brains must implement these methods
        public abstract void Start( Agent actor ); // start the brain, agent is passed so we can hook a coroutine to it, not the cleanest way 

        public virtual void Update() { } // called periodically by the agent, acts like Unity's update method
        
        public void SetAttachedAgent(Agent agent)
        {
            AttachedAgent = agent;
        }

        // helper function used brains to convert a path of Tiles into move commands
        // do not remove source tile when calling
        protected static void PathToMoveCommands ( List<Tile> path , Queue<AgentMove> commands )
        {
            for (var i = 0; i < path.Count -1 ; ++i)
            {
                commands.Enqueue(new GoMove(path[i] , path[i+1]));
            }
        }

    }
}



using UnityEngine;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains
{
    public abstract class BaseBrain
    {
        // reference to the agent the brain is attached to
        protected Agent AttachedAgent;
        private  bool _IsReady = false ;

        public bool IsReady // called by the agent 
        {
            get => _IsReady;
            protected set => _IsReady = value;
        }

        // Child Brains must implement these methods
        public abstract AgentAction GetNextAction(); // for use by the agent movement routines

        public abstract void Start( Agent actor ); // start the brain, agent is passed so we can hook a coroutine to it, not the cleanest way 
        public abstract void Reset(); // reset the brain

        public void SetAttachedAgent(Agent agent)
        {
            AttachedAgent = agent;
        }
        
    }
}


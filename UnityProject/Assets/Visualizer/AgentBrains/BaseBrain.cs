
using UnityEngine;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains
{
    public abstract class BaseBrain
    {
        // reference to the agent the brain is attached to
        protected Agent AttachedAgent;
        // Base methods that are already implemented
        
        // Child Brains must implement these methods
        public abstract AgentAction GetNextDest(); // for use by the agent movement routines

        public abstract void Start(); // start the brain
        public abstract void Pause(); // pause the brain
        public abstract void Reset(); // reset the brain

        public void SetAttachedAgent(Agent agent)
        {
            AttachedAgent = agent;
        }
    }
}


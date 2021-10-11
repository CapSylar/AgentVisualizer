
using UnityEngine;

namespace Visualizer.AgentBrains
{
    public abstract class BaseBrain
    {
        // Base methods that are already implemented
        
        // Child Brains must implement these methods
        public abstract Vector3 GetNextDest(); // for use by the agent movement routines

        public abstract void Start(); // start the brain
        public abstract void Pause(); // pause the brain
        public abstract void Reset(); // reset the brain
    }
}


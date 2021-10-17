using System;
using UnityEngine.Serialization;

namespace Visualizer.GameLogic
{
    [Serializable()]
    public class AgentState
    {
        // contains all the data to be loaded/saved from the Agent
        // for now we only need to save the location inside the map in terms of tiles

        // In case we want to save the Map with no agent, then valid = false, and AgentState just serves as an empty placeholder
        public bool valid;
        public int tileX;
        public int tileZ;

        public AgentState()
        {
            valid = false;
        }
        public AgentState(Agent agent)
        {
            valid = true;
            var vec = agent.getGridPosition();
            tileX = (int)vec.x;
            tileZ = (int)vec.y;
        }
        
    }
}
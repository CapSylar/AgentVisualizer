using UnityEngine;
using Visualizer.AgentBrains;

namespace Visualizer
{
    
    public class GameStateBehavior : MonoBehaviour
    {
        public GameObject _planePrefab;
        public GameObject _agentPrefab;

        public int X;
        public int Y;

        void Start()
        {
            // create new map
            var newMap = new Map(_planePrefab, X, Y);
            // create a new brain
            BaseBrain newBrain = new retardedBrain(newMap);
            // create a new agent
            Agent.CreateAgent(_agentPrefab, newBrain, newMap, 0, 0);
        }

        void Update()
        {
            // do something here
        }
    }

}

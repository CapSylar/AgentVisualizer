using System.Collections;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer
{
    public class Agent : MonoBehaviour
    {
        private BaseBrain _currentBrain;
        private Map _currentMap;

        public int tileX, tileZ; // position inside the map grid in terms of tiles
        private Tile currentTile;
        
        // state variables
        public Tile CurrentTile
        {
            get => currentTile;
            set => currentTile = value;
        }

        private bool isRunning = false;
        private AgentAction lastAction = null;
        
        //TODO: have to refactor these two Init functions
        void Init( BaseBrain brain , Map map , int x , int z )
        {
            _currentBrain = brain;
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            currentTile = _currentMap.GetTile(x, z);
            gameObject.transform.transform.position = currentTile.getWorldPosition();
        }

        void Init(BaseBrain brain, Map map, AgentState state)
        {
            _currentBrain = brain;
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            //TODO: why is valid not checked here ?
            currentTile = _currentMap.GetTile(state.tileX, state.tileZ);
            gameObject.transform.transform.position = currentTile.getWorldPosition();
        }

        void FixedUpdate()
        {
            if (isRunning)
            {
                Move();
            }
        }

        private void Move()
        {
            if (lastAction == null || lastAction.IsDone())
            {
                lastAction = _currentBrain.GetNextDest();
                lastAction?.Do(this);
            }
        }
        
        public static Agent CreateAgent(BaseBrain brain, Map map, AgentState state)
        {
            var gameObject = Instantiate(PrefabContainer.Instance.agentPrefab);
            var component = gameObject.AddComponent<Agent>();

            if (state.valid) // was the agent position saved with the Map ? 
            {
                component.Init( brain , map , state );
            }
            else
            {
                component.Init( brain , map , 0 , 0 ); // 0 0 as defaults
            }
            return component;
        }
        public static Agent CreateAgent(BaseBrain brain , Map map ,  int x , int z )
        {
            var gameObject = Instantiate(PrefabContainer.Instance.agentPrefab , PrefabContainer.Instance.mapReference.transform ); //TODO: transform shouldn't be used here
            var component = gameObject.AddComponent<Agent>();

            component.Init( brain , map , x, z );
            return component;
        }
        
        // forward to the brain

        public void StartAgent()
        {
            _currentBrain.Start();
            isRunning = true;
        }

        public void PauseAgent()
        {
            _currentBrain.Pause();
            isRunning = false;
        }

        public void ResetAgent()
        {
            _currentBrain.Reset();
            isRunning = false;
        }
        
        public void Destroy()
        {
            Destroy(gameObject); // byebye!
        }
    }
}

using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    public class Agent : MonoBehaviour
    {
        private BaseBrain _currentBrain;
        private Map _currentMap;

        private Tile _currentTile;
        
        // state variables
        public Tile CurrentTile
        {
            get => _currentTile;
            set => _currentTile = value;
        }

        private bool isRunning = false;
        private AgentAction lastAction = null;
        
        void Init( Map map , int x , int z )
        {
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            _currentTile = _currentMap.GetTile(x, z);
            gameObject.transform.transform.position = _currentTile.getWorldPosition();
        }

        void Init( Map map, AgentState state)
        {
            Init( map , state.tileX , state.tileZ );
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
                component.Init( map , state );
            }
            else
            {
                component.Init( map , 0 , 0 ); // 0 0 as defaults
            }
            return component;
        }
        public static Agent CreateAgent( Map map ,  int x , int z )
        {
            var gameObject = Instantiate(PrefabContainer.Instance.agentPrefab , PrefabContainer.Instance.mapReference.transform ); //TODO: transform shouldn't be used here
            var component = gameObject.AddComponent<Agent>();

            component.Init( map , x, z );
            return component;
        }

        public void SetBrain(BaseBrain brain)
        {
            _currentBrain = brain;
            brain.SetAttachedAgent(this);
        }

        public Vector2 getGridPosition()
        {
            return new Vector2( _currentTile.GridX , _currentTile.GridZ );
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

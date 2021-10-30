using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    public enum AGENT_STATE // assuming z is looking up and x to the right and we are looking down in 2D
    {
        NOT_RUNNING = 0, // was never running 
        RUNNING, // is running right now
        PAUSED, // is pause, but can be resumed
    }

    public class Agent : MonoBehaviour
    {
        private BaseBrain _currentBrain;
        private Map _currentMap;
        private Tile _currentTile;
        
        // agent initial position
        private Tile _initialTile;
        
        // state variables

        private AGENT_STATE state = AGENT_STATE.NOT_RUNNING; // created as not running, needs to be initialized 
        
        public Tile CurrentTile
        {
            get => _currentTile;
            set => _currentTile = value;
        }

        private AgentAction lastAction = null;
        
        void Init( Map map , int x , int z )
        {
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            _initialTile = _currentTile = _currentMap.GetTile(x, z);
            gameObject.transform.transform.position = _currentTile.getWorldPosition();
            
            // hook the needed events
            GameStateManager.Instance.OnSceneReset += ResetAgent;
            GameStateManager.Instance.OnScenePause += PauseAgent;
            GameStateManager.Instance.OnSceneStart += StartAgent;
            GameStateManager.Instance.OnSceneResume += StartAgent;
        }

        void Init( Map map, AgentState state)
        {
            Init( map , state.tileX , state.tileZ );
        }

        void FixedUpdate()
        {
            if (state == AGENT_STATE.RUNNING)
            {
                Move();
            }
        }
        
        public static void SetSpeed(int speedMultiplier) // sets the multiplier globally for all agents
        {
            GoAction.SetMultiplier(speedMultiplier); // set it for all future GoActions
        }

        private void Move()
        {
            if (lastAction == null || lastAction.IsDone())
            {
                lastAction = _currentBrain.GetNextAction();
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
            if ( state == AGENT_STATE.NOT_RUNNING )
            {
                _currentBrain.Start(); // if he was not running before
            }

            state = AGENT_STATE.RUNNING;
        }

        public void PauseAgent()
        {
            //TODO: check again, is this state really needed ?
            // _currentBrain.Pause();
            state = AGENT_STATE.PAUSED;
        }

        public void ResetAgent()
        {
            state = AGENT_STATE.NOT_RUNNING;
            // reset the agents position
            _currentTile = _initialTile;
            gameObject.transform.position = _currentTile.getWorldPosition();
            
            // reset brain at last using restored agent
            _currentBrain.Reset();
        }
        
        public void Destroy()
        {
            Destroy(gameObject); // byebye!
        }
    }
}

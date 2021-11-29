using System;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    // Graphical Wrapper around the base Agent class, contains UI specific code

    [Serializable()]
    public class GraphicalAgent : Agent
    {
        [NonSerialized()]
        private GameObject _gameObject;

        // telemetry object, reused every time
        [NonSerialized()]
        private AgentTelemetry _telemetry = new AgentTelemetry();
        
        public int Steps
        {
            get => _steps;
            set { _steps = value; SendTelemetry(); }
        }

        public int Turns
        {
            get => _turns;
            set { _turns = value; SendTelemetry(); }
        }
        
        public GraphicalAgent ( BaseBrain brain , GraphicalBoard board , int gridX , int gridZ ) : base ( brain , 
            board , gridX , gridZ )
        {
            // create the Agent gameObject
            _gameObject = GameObject.Instantiate(PrefabContainer.Instance.agentPrefab);
            _gameObject.transform.position = board.GetTile(gridX, gridZ).GetWorldPosition();
            
            // hook the needed events
            GameStateManager.Instance.OnSceneReset += ResetAgent;
            GameStateManager.Instance.OnScenePause += PauseAgent;
            GameStateManager.Instance.OnSceneStart += StartAgent;
            GameStateManager.Instance.OnSceneResume += StartAgent;
            
            // init telemetry with start values
            SendTelemetry();
        }

        public GraphicalAgent(BaseBrain brain, GraphicalBoard board, Agent agent ) :
            this ( brain , board , agent.initialGridX , agent.initialGridZ) { }

        public GraphicalAgent(GraphicalBoard board, Agent agent) : this( null , board ,
            agent.initialGridX , agent.initialGridZ ) { }

        public GraphicalAgent(GraphicalBoard board, int gridX, int gridZ) : this( null , board , gridX , gridZ) { } 
        
        public static void SetSpeed(int speedMultiplier) // sets the multiplier globally for all agents
        {
            GoAction.SetMultiplier(speedMultiplier); // set it for all future GoActions
        }
    
        private void SendTelemetry()
        {
            _telemetry.Steps = _steps;
            _telemetry.Turns = _turns;
                
            GlobalTelemetryHandler.Instance.UpdateAgentTelemetry(_telemetry);
        }
        
        //TODO: duplicate code between Agent, fix !!
        protected override void Move()
        {
            if (_lastAction == null)
            {
                if (_currentBrain.HasNextAction())
                {
                    _lastAction = _currentBrain.GetNextAction();
                    _lastAction?.Do(this);
                }
            }
            else if (_lastAction.IsDone())
            {
                InvokeOnActionDone();
                _lastAction = null;
            }
        }

        public override void ResetAgent()
        {
            // reset the agents position
            _currentTile = _currentBoard.GetTile(initialGridX , initialGridZ);
            _gameObject.transform.position = _currentTile.GetWorldPosition();
            
            // clear telemetry data
            Steps = Turns = 0;
            
            base.ResetAgent();
        }

        public override void Destroy()
        {            
            //TODO: get hold of a GameObject 
            // unhook all events
            GameStateManager.Instance.OnSceneReset -= ResetAgent;
            GameStateManager.Instance.OnScenePause -= PauseAgent;
            GameStateManager.Instance.OnSceneStart -= StartAgent;
            GameStateManager.Instance.OnSceneResume -= StartAgent;
            GameObject.Destroy(_gameObject);
            
            base.Destroy();
        }
        
        public Transform GetTransform()
        {
            return _gameObject.transform;
        }
        
    }
}
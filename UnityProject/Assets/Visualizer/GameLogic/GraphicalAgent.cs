using System;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.GameLogic.AgentMoves;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    // Graphical Wrapper around the base Agent class, contains UI specific code
    [Serializable()]
    public class GraphicalAgent : Agent
    {
        [NonSerialized()]
        public  GameObject _gameObject;

        // telemetry object, reused every time
        [NonSerialized()]
        private AgentTelemetry _telemetry = new AgentTelemetry();
        
        public override int Steps
        {
            get => base.Steps;
            set { base.Steps = value; SendTelemetry(); }
        }

        public override int Turns
        {
            get => base.Turns;
            set { base.Turns = value; SendTelemetry(); }
        }
        
        public GraphicalAgent ( BaseBrain brain , GraphicalBoard board , int gridX , int gridZ , GameObject prefab ) : base ( brain , 
            board , gridX , gridZ )
        {
            // create the Agent gameObject
            _gameObject = GameObject.Instantiate(prefab);
            _gameObject.transform.position = board.GetTile(gridX, gridZ).GetWorldPosition();

            // init telemetry with start values
            SendTelemetry();
        }

        public GraphicalAgent(BaseBrain brain, GraphicalBoard board, Agent agent , GameObject prefab ) :
            this ( brain , board , agent.initialGridX , agent.initialGridZ , prefab ) { }

        public GraphicalAgent(GraphicalBoard board, Agent agent , GameObject prefab ) : this( null , board ,
            agent.initialGridX , agent.initialGridZ , prefab ) { }

        public GraphicalAgent(GraphicalBoard board, int gridX, int gridZ , GameObject prefab ) : 
            this( null , board , gridX , gridZ , prefab ) { } 
        
        public static void SetSpeed(int speedMultiplier) // sets the multiplier globally for all agents
        {
            GoMove.SetMultiplier(speedMultiplier); // set it for all future GoActions
        }
    
        private void SendTelemetry()
        {
            _telemetry.Steps = base.Steps;
            _telemetry.Turns = base.Turns;
                
            GlobalTelemetryHandler.Instance.UpdateAgentTelemetry(_telemetry);
        }
        
        //TODO: duplicate code between Agent, fix !!
        protected override void Move()
        {
            if (_currentBrain.HasNextAction())
            {
                _lastAction = _currentBrain.GetNextAction();
                _lastAction?.Do(this);
            }
        }

        public override void Reset()
        {
            // reset the agents position
            _currentTile = _currentBoard.GetTile(initialGridX , initialGridZ);
            _gameObject.transform.position = _currentTile.GetWorldPosition();
            
            // clear telemetry data
            Steps = Turns = 0;
            
            base.Reset();
        }

        public override void Destroy()
        {
            GameObject.Destroy(_gameObject);
            _gameObject = null;
            
            base.Destroy();
        }
        
        public Transform GetTransform()
        {
            return _gameObject.transform;
        }
    }
}
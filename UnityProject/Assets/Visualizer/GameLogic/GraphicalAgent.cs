using System;
using TMPro;
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

        public GraphicalAgent ( BaseBrain brain , GraphicalBoard board , int gridX , int gridZ , GameObject prefab , int id = 0 ) : base ( brain , 
            board , gridX , gridZ )
        {
            // create the Agent gameObject
            _gameObject = GameObject.Instantiate(prefab);

            _id = id;
            // get Number tag and assign id to it
            var text = _gameObject.GetComponentInChildren<TMP_Text>();
            text.SetText(""+_id);
            
            _gameObject.transform.position = board.GetTile(gridX, gridZ).GetWorldPosition();
        }

        public GraphicalAgent(BaseBrain brain, GraphicalBoard board, Agent agent , GameObject prefab , int id = 0 ) :
            this ( brain , board , agent.initialGridX , agent.initialGridZ , prefab , id ) { }

        public GraphicalAgent(GraphicalBoard board, Agent agent , GameObject prefab , int id = 0 ) : this( null , board ,
            agent.initialGridX , agent.initialGridZ , prefab , id ) { }

        public GraphicalAgent(GraphicalBoard board, int gridX, int gridZ , GameObject prefab , int id = 0 ) : 
            this( null , board , gridX , gridZ , prefab , id ) { }

        public static void SetSpeed(int speedMultiplier) // sets the multiplier globally for all agents
        {
            GoMove.SetMultiplier(speedMultiplier); // set it for all future GoActions
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
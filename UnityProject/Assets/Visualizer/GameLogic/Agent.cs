using System;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.GameLogic.AgentActions;

namespace Visualizer.GameLogic
{
    [Serializable()]
    public class Agent
    {
        [NonSerialized]
        protected BaseBrain _currentBrain;
        [NonSerialized]
        protected Board _currentBoard;
        [NonSerialized]
        protected Tile _currentTile;
        public Tile CurrentTile
        {
            get => _currentTile;
            set
            {
                _currentTile = value;
                OnTileChange?.Invoke();
            }
        }

        public Board CurrentBoard
        {
            get => _currentBoard;
            protected set => _currentBoard = value;
        }

        // agent initial position
        public int initialGridX, initialGridZ;
        
        // state variables

        [NonSerialized]
        protected int _steps;
        [NonSerialized]
        protected int _turns;

        public int Steps
        {
            get => _steps;
            set => _steps = value;
        }

        //TODO: turns do not work for now, fix !
        public int Turns
        {
            get => _turns;
            set => _turns = value;
        }

        // delegates

        public event Action OnTileChange; // called when the agent moves a Tile
        public event Action OnActionDone; // called when the agent finishes a Agent Action
        
        // protected AGENT_STATE _state = AGENT_STATE.NOT_RUNNING; // created as not running, needs to be initialized 
        
        [NonSerialized]
        protected AgentAction _lastAction;

        public Agent(BaseBrain brain, Board board, int gridX, int gridZ)
        {
            _currentBoard = board;
            initialGridX = gridX;
            initialGridZ = gridZ;
            _currentBrain = brain;
            _currentTile = _currentBoard.GetTile(gridX, gridZ);
        }

        public Agent ( BaseBrain brain , Board board, Agent agent ) : this( brain , board , agent.initialGridX , agent.initialGridZ ) { }
        
        public Agent() : this( null , null , 0 , 0 ) { }
        public Agent(Board board, int gridX, int gridZ) : this(null, board, gridX, gridZ) { }

        public void Update()
        {
            Move();
        }
        
        protected virtual void Move()
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

        protected void InvokeOnActionDone()
        {
            OnActionDone?.Invoke();
        }

        public void SetBrain(BaseBrain brain)
        {
            _currentBrain = brain;
            brain.SetAttachedAgent(this);
        }

        public Vector2 GetGridPosition()
        {
            return new Vector2( _currentTile.GridX , _currentTile.GridZ );
        }
        
        // TODO: find a cleaner way to do these
        public void HookToEventOnTileChange( Action callBack ) { OnTileChange += callBack; }
        public void UnHookEventOnTileChange(Action callback) { OnTileChange -= callback; }
        public void HookToEventOnActionDone(Action callback) { OnActionDone += callback; }
        public void UnHookEventOnActionDone(Action callback) { OnActionDone -= callback;}

        // forward to the brain

        public virtual void Start()
        {
            _currentBrain.Start( this );
        }

        public virtual void Reset()
        {
            // reset brain before removing it
            _currentBrain?.Reset();
            _currentBrain = null;
            
            if (OnTileChange != null)
            {
                // unhook all events
                foreach (var eventHandler in OnTileChange?.GetInvocationList())
                {
                    OnTileChange -= (Action) eventHandler;
                }
            }
        }

        public virtual void Destroy() { }
        
    }
}

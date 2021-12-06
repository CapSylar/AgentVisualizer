using System;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.GameLogic.AgentMoves;

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
        
        [NonSerialized]
        protected AgentMove _lastAction;

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

        // called once per turn by Game
        public void Update()
        {
            _currentBrain?.Update();
            Move();
        }

        public bool IsDone()
        {
            return _lastAction == null || _lastAction.IsDone();
        }

        // puts a single action in motion
        protected virtual void Move()
        {
            if (_currentBrain.HasNextAction())
            {
                _lastAction = _currentBrain.GetNextAction();
                _lastAction?.Do(this);
            }
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
        }

        public void DoMove( AgentMove move )
        {
            move.Do(this);
        }

        public virtual void Destroy() { }
        
    }
}

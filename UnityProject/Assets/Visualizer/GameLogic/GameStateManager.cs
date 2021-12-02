using System;
using System.Collections.Generic;
using System.Linq;
using Visualizer.AgentBrains;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    // is a singleton, only one instance of the GameState class at any point in time
    public delegate void OnSceneStateChange();
    
    public class GameStateManager
    {
        // this is a singleton, only a single instance should exist at any time during the game
        private static GameStateManager _instance;

        public static GameStateManager Instance => _instance;

        public GameStateManager()
        {
            _instance = this; // TODO: fix the singleton 
        }

        public GraphicalBoard CurrentBoard { get; private set; }

        private Game _currentGame;
        
        private List<Agent> _goodAgents = new List<Agent>();
        private List<Agent> _evilAgents = new List<Agent>();

        private Type currentGoodBrainType;
        private Type currentEvilBrainType;
        
        // state
        
        private enum GameState
        {
            NOT_RUNNING,
            RUNNING,
            PAUSED
        }
        
        private GameState _state = GameState.NOT_RUNNING;

        
        // events
        public event OnSceneStateChange OnSceneStart;
        public event OnSceneStateChange OnScenePause;
        // public event OnSceneStateChange OnSceneResume;
        public event OnSceneStateChange OnSceneReset;

        public void Load( string path ) // load a game configuration
        {
            //TODO: fix loading, broken for now
            // Board board;
            // Agent agent;
            // GameLogic.GameState.Load( path , out board , out agent );
            //
            // SetCurrentMap(new GraphicalBoard(board));
            // SetCurrentAgent( new GraphicalAgent(new TspSimulatedAnnealingFullVisibility(CurrentBoard), CurrentBoard, agent) , );
        }
        
        public void Save(string path) // save a game configuration
        {
            //TODO: fix saving, broken for now
            // GameLogic.GameState state = new GameLogic.GameState( CurrentBoard , currentGoodAgent );
            // state.Save( path );
        }

        public void Update()
        {
            if (_state == GameState.RUNNING)
            {
                _currentGame.Update();
            }
        }

        public void SetCurrentAgent( int x , int z , bool isGood = true )
        {
            // for now we keep only one per team
            SetCurrentAgent( new GraphicalAgent( CurrentBoard , x , z , isGood? PrefabContainer.Instance.agentPrefab :
                PrefabContainer.Instance.agentEnemyPrefab ) , isGood );
        }

        public void SetCurrentAgent( Agent agent , bool isGood = true )
        {
            // for now we keep only one per team
            if (isGood) // add a good agent
            {
                foreach (var goodAgent in _goodAgents)
                {
                    goodAgent.Destroy();
                }
                
                _goodAgents.Add( agent );
            }
            else // add an evil agent
            {
                foreach (var evilAgent in _evilAgents)
                {
                    evilAgent.Destroy();
                }
                
                _evilAgents.Add( agent );
            }
        }
        
        public void SetCurrentBrain(Type brainType , bool isGood )
        {
            if (isGood)
                currentGoodBrainType = brainType;
            else
                currentEvilBrainType = brainType;
        }
        
        public void RemoveAgent( int gridX , int gridZ , bool isGood )
        {
            // Nuke them all for now
            if (isGood)
            {
                foreach (var goodAgent in _goodAgents)
                {
                    goodAgent.Destroy();
                }
            }
            else
            {
                foreach (var goodAgent in _evilAgents)
                {
                    goodAgent.Destroy();
                }
            }
        }

        public void SetCurrentMap( GraphicalBoard newGraphicalBoard )
        {
            CurrentBoard?.Destroy();
            CurrentBoard = newGraphicalBoard;
        }
        
        public void StartGame()
        {
            // start the game if it can be started or unpause if it was paused

            switch (_state)
            {
                case GameState.PAUSED:
                    _state = GameState.RUNNING; // unpause
                    break;
                case GameState.NOT_RUNNING: // create a game and start it
                    CreateGame();
                    _state = GameState.RUNNING;
                    break;
            }

            OnSceneStart?.Invoke();
        }

        private void CreateGame()
        {
            // create all the good and evil agents with their brains
            
            //careful,lines below are very loose in structure!!!
            //assumes all children of BaseBrain need Map as a constructor parameter only
            foreach (var goodAgent in _goodAgents)
            {
                goodAgent.SetBrain((BaseBrain)Activator.CreateInstance(currentGoodBrainType, CurrentBoard));
            }

            foreach (var evilAgent in _evilAgents)
            {
                evilAgent.SetBrain((BaseBrain)Activator.CreateInstance(currentEvilBrainType, CurrentBoard));
            }
            
            // create the game

            _currentGame = new Game(CurrentBoard, _goodAgents.Concat(_evilAgents).ToList());
        }

        public void ResetGame()
        {
            OnSceneReset?.Invoke();
            _currentGame?.Reset();
            _state = GameState.NOT_RUNNING;
        }
        
        public void PauseGame()
        {
            OnScenePause?.Invoke();
            _state = GameState.PAUSED;
        }

        public void SetSpeed( int speedMultiplier ) // speed multiplier between 1 and 10 
        {
            GraphicalAgent.SetSpeed(speedMultiplier);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.Algorithms;
using Visualizer.Algorithms.BoardEvaluation;
using Visualizer.GameLogic.Conditions;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    // is a singleton, only one instance of the GameState class at any point in time
    public delegate void OnSceneStateChange();
    
    public class GameStateManager
    {
        public enum GameState
        {
            NOT_RUNNING,
            RUNNING,
            PAUSED,
            STOPPED
        }

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

        private Type _currentGoodBrainType;
        private Type _currentEvilBrainType;

        private StoppingCondition _stoppingCondition;
        
        private BoardEvaluator _currentBoardEvaluator;
        public BoardEvaluator CurrentBoardEvaluator => _currentBoardEvaluator;

        // state

        public GameState State { get; private set; } = GameState.NOT_RUNNING;

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
            if (State != GameState.RUNNING) return;
            
            _currentGame.PlayTurn();
                
            if (_currentGame.HasEnded())
            {
                StopGame();
            }
        }

        public void AddAgent( int x , int z , bool isGood = true )
        {
            AddAgent( new GraphicalAgent( CurrentBoard , x , z , isGood? PrefabContainer.Instance.agentPrefab :
                PrefabContainer.Instance.agentEnemyPrefab , _goodAgents.Count + _evilAgents.Count ) , isGood ); // just give ids in incremental order
        }

        public void AddAgent( Agent agent , bool isGood = true )
        {
            if (isGood) // add a good agent
            {
                _goodAgents.Add( agent );
            }
            else // add an evil agent
            {
                _evilAgents.Add( agent );
            }
        }
        
        public void SetCurrentBrain(Type brainType , bool isGood )
        {
            if (isGood)
                _currentGoodBrainType = brainType;
            else
                _currentEvilBrainType = brainType;
        }
        
        public void RemoveAgent( int gridX , int gridZ , bool isGood )
        {
            var tile = CurrentBoard.GetTile(gridX, gridZ);
            var list = isGood ? _goodAgents : _evilAgents;
            
            // search for the agent to be destroyed in the corresponding list
            for (var i = 0; i < list.Count; ++i)
            {
                if (list[i].CurrentTile == tile)
                {
                    list[i].Destroy();
                    list.RemoveAt(i);
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

            switch (State)
            {
                case GameState.PAUSED:
                    State = GameState.RUNNING; // unpause
                    break;
                case GameState.NOT_RUNNING: // create a game and start it
                    CreateGame();
                    State = GameState.RUNNING;
                    break;
            }

            OnSceneStart?.Invoke();
        }

        private void CreateGame()
        {
            //TODO: for now only a single brain type per team
            // create all the good and evil agents with their brains
            
            //careful, lines below are very loose in structure!!!
            //assumes all children of BaseBrain need Map as a constructor parameter only
            foreach (var goodAgent in _goodAgents)
            {
                goodAgent.SetBrain((BaseBrain)Activator.CreateInstance(_currentGoodBrainType, CurrentBoard));
            }

            foreach (var evilAgent in _evilAgents)
            {
                evilAgent.SetBrain((BaseBrain)Activator.CreateInstance(_currentEvilBrainType, CurrentBoard));
            }
            
            // create the game
            _currentGame = new Game(CurrentBoard, _goodAgents.Concat(_evilAgents).ToList() , _stoppingCondition );
        }

        public void ResetGame()
        {
            OnSceneReset?.Invoke();
            _currentGame?.Reset();
            State = GameState.NOT_RUNNING;
        }
        
        public void PauseGame()
        {
            OnScenePause?.Invoke();
            State = GameState.PAUSED;
        }

        public void StopGame()
        {
            // send result to scoreBoardHandler
            ScoreBoardHandler.Instance.ShowResults(PerformanceEvaluator.GetBestTeamWise(_currentGame));

            State = GameState.STOPPED; // needs a reset before it can run again
        }

        public void SetSpeed( int speedMultiplier ) // speed multiplier between 1 and 10 
        {
            GraphicalAgent.SetSpeed(speedMultiplier);
        }

        public void SetCurrentStoppingCondition(Type stoppingCondition)
        {
            var condition = (StoppingCondition)Activator.CreateInstance(stoppingCondition);
            condition.GetGraphicalConstructor().Construct(SetCurrentStoppingCondition);
        }

        private void SetCurrentStoppingCondition(StoppingCondition condition)
        {
            _stoppingCondition = condition;
        }

        public void SetCurrentBoardEvaluator(Type boardEvaluator)
        {
            _currentBoardEvaluator = (BoardEvaluator) Activator.CreateInstance(boardEvaluator);
        }
    }
}

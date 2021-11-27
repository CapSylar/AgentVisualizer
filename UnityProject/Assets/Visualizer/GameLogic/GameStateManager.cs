using System;
using UnityEngine;
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

        public static GameStateManager Instance
        {
            get
            {
                return _instance;
            }
        }
        
        public GameStateManager()
        {
            _instance = this; // TODO: fix the singleton 
        }

        public GraphicalBoard CurrentGraphicalBoard { get; private set; }
        
        [HideInInspector]
        public Agent currentAgent;

        [HideInInspector]
        public Type currentBrainType;
        
        // state

        private bool isPaused = false;
        
        // events

        public event OnSceneStateChange OnSceneStart;
        public event OnSceneStateChange OnScenePause;
        public event OnSceneStateChange OnSceneResume;
        public event OnSceneStateChange OnSceneReset;

        public void Load( string path ) // load a game configuration
        {
            Board board;
            AgentState agentState;
            GameState.Load( path , out board , out agentState );

            SetCurrentMap(new GraphicalBoard(board));
            SetCurrentAgent(Agent.CreateAgent(new TspSimulatedAnnealingFullVisibility(CurrentGraphicalBoard), CurrentGraphicalBoard, agentState));
        }
        
        public void Save(string path) // save a game configuration
        {
            GameState state = new GameState( CurrentGraphicalBoard , currentAgent );
            state.Save( path );
        }

        public void SetCurrentAgent( int x , int z )
        {
            currentAgent?.Destroy(); // only one agent allowed 
            currentAgent = Agent.CreateAgent( CurrentGraphicalBoard , x , z  );
            // CurrentGraphicalBoard.SetActiveAgent(currentAgent);
        }

        public void SetCurrentAgent(Agent agent)
        {
            currentAgent?.Destroy(); // only one agent allowed 
            currentAgent = agent;
            // CurrentGraphicalBoard.SetActiveAgent(currentAgent);
        }
        
        public void RemoveCurrentAgent()
        {
            currentAgent?.Destroy();
            currentAgent = null;
            // CurrentGraphicalBoard.SetActiveAgent(null);
        }

        public void SetCurrentMap( GraphicalBoard newGraphicalBoard )
        {
            CurrentGraphicalBoard?.Destroy();
            CurrentGraphicalBoard = newGraphicalBoard;
        }
        
        public void StartGame()
        {
            // start the game if it can be started or unpause if it was paused 

            if (isPaused)
            {
                isPaused = false;
            }
            else if (currentAgent != null)
            {
                //TODO: careful,line below is very loose in structure!!!
                //TODO: assumes all children of BaseBrain need Map as a constructor parameter only
                currentAgent.SetBrain((BaseBrain)Activator.CreateInstance(currentBrainType, CurrentGraphicalBoard));
            }
            
            OnSceneStart?.Invoke();
        }

        public void ResetGame()
        {
            OnSceneReset?.Invoke();
            isPaused = false;
        }
        
        public void PauseGame()
        {
            OnScenePause?.Invoke();
            isPaused = true;
        }

        public void SetCurrentBrain(Type brainType)
        {
            currentBrainType = brainType;
        }

        public void SetSpeed( int speedMultiplier ) // speed multiplier between 1 and 10 
        {
            Agent.SetSpeed(speedMultiplier);
        }
    }
}

using System;
using UnityEngine;
using Visualizer.AgentBrains;

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

        private Map _currentMap;
        public Map currentMap { get; set; }
        
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
            MapState tileState;
            AgentState agentState;
            GameState.Load( path , out tileState , out agentState );

            currentMap = new Map(tileState);
            currentAgent = Agent.CreateAgent(new TspSimulatedAnnealingFullVisibility(currentMap), currentMap, agentState);
        }
        
        public void Save(string path) // save a game configuration
        {
            GameState state = new GameState( currentMap , currentAgent );
            state.Save( path );
        }

        public void SetCurrentAgent( int x , int z )
        {
            currentAgent?.Destroy(); // only one agent allowed 
            currentAgent = Agent.CreateAgent( currentMap , x , z  );
            currentMap.SetActiveAgent(currentAgent);
        }
        
        public void RemoveCurrentAgent()
        {
            currentAgent?.Destroy();
            currentAgent = null;
            currentMap.SetActiveAgent(null);
        }

        public void SetCurrentMap( Map newMap )
        {
            currentMap?.Destroy();
            currentMap = newMap;
        }
        
        public void StartGame()
        {
            // start the game if it can be started or unpause if it was paused 

            if (isPaused)
            {
                isPaused = false;
            }
            else if (currentAgent)
            {
                //TODO: careful,line below is very loose in structure!!!
                //TODO: assumes all children of BaseBrain need Map as a constructor parameter only
                currentAgent.SetBrain((BaseBrain)Activator.CreateInstance(currentBrainType, currentMap));
            }
            
            OnSceneStart?.Invoke();
        }

        public void ResetGame()
        {
            OnSceneReset?.Invoke();
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

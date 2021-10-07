using System;
using UnityEngine;
using Visualizer.GameLogic;
using Random = UnityEngine.Random;

namespace Visualizer
{
    // is a singleton, only one instance of the GameState class at any point in time
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this) // more than one instance, suicide!!!
            {
                Destroy(this.gameObject);
            }
            else // we are the first instance, assign ourselves
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject); // persistent across scene loads
            }
        }
        
        public GameObject _planePrefab;
        public GameObject _agentPrefab;
        public GameObject _wallPrefab;
        public GameObject _wallPrefabPreview;
        public GameObject _dirtyPlanePrefab;
        public Texture _dirtTexture;
        public GameObject _mapReference;

        public int X;
        public int Z;
        
        private Map _currentMap;
        public Map currentMap { get; set; }
        
        [HideInInspector]
        public Agent currentAgent;
        void Start()
        {
        }

        void Update()
        {
        }

        public void Load( string path ) // load a game configuration
        {
            TileState[,] tileState;
            AgentState agentState;
            GameState.Load("Assets/Visualizer/Maps/map000.map" , out tileState , out agentState );

            currentMap = new Map(_planePrefab, _mapReference, tileState);

            if ( agentState.valid )
                currentAgent = Agent.CreateAgent( _agentPrefab , new retardedBrain(currentMap) , currentMap , 0 , 0  );
            else
                currentAgent = Agent.CreateAgent( _agentPrefab , new retardedBrain(currentMap) , currentMap , agentState );
        }
        
        public void Save(string path) // save a game configuration
        {
            GameState state = new GameState( currentMap );
            state.Save( path );
        }

        public void SetCurrentAgent( int x , int z )
        {
            currentAgent.Destroy(); // only one agent allowed 
            currentAgent = Agent.CreateAgent( _agentPrefab , new retardedBrain(currentMap) , currentMap , x , z  );
            currentMap.SetActiveAgent(currentAgent);
        }
        
        public void RemoveCurrentAgent()
        {
            currentAgent.Destroy();
            currentMap.SetActiveAgent(null);
        }
    }

}

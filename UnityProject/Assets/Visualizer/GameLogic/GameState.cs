using UnityEngine;

namespace Visualizer
{
    // is a singleton, only one instance of the GameState class at any point in time
    public class GameState : MonoBehaviour
    {
        private static GameState _instance;

        public static GameState Instance
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
        public int Y;
        
        
        private Map _currentMap;
        public Map currentMap { get; set; }

        
        private GameObject testing;

        void Start()
        {
            // create new map
            // currentMap = new Map(_planePrefab, _mapReference,  X, Y);
            currentMap = Map.LoadMap("Assets/Visualizer/Maps/map000.map", _planePrefab, _mapReference);
            // create a new brain
            // BaseBrain newBrain = new retardedBrain(currentMap);
            // // create a new agent
            // Agent.CreateAgent(_agentPrefab, newBrain, currentMap, 0, 0);
        }

        void Update()
        {
         
        }
    }

}

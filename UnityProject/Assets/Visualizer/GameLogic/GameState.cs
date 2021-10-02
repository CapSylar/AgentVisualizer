using UnityEngine;
using UnityEngine.UIElements;
using Visualizer.AgentBrains;

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

        public GameObject _mapReference; 

        public int X;
        public int Y;

        private Map currentMap;
        
        private GameObject testing;

        void Start()
        {
            // create new map
            currentMap = new Map(_planePrefab, _mapReference,  X, Y);
            // currentMap.SaveMap("Map000.map");
            // create a new brain
            BaseBrain newBrain = new retardedBrain(currentMap);
            // create a new agent
            Agent.CreateAgent(_agentPrefab, newBrain, currentMap, 0, 0);

            testing = Instantiate(_wallPrefab);
        }

        void Update()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            // if(Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            // {
            //     if (testing.gameObject.activeSelf)
            //     {
            //         
            //     }
            // }

            if (Physics.Raycast(inputRay, out hit))
            {
                var point = transform.InverseTransformPoint(hit.point);
                // Debug.Log("returned from inverse point transform: " + point );

                var tile = currentMap.PointToTile(point);
                var closestEdge = tile.GetClosestEdgeWorldPos(point);
                var closesDirection = tile.GetClosestEdge(point);

                // rotate wall in the correct direction
                if (closesDirection == TILE_EDGE.UP || closesDirection == TILE_EDGE.DOWN)
                    testing.transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    testing.transform.rotation = Quaternion.Euler(0,90,0);
                
                // should be close enough

                if (Vector3.Distance(closestEdge, point) < 3) //TODO: remove magic number
                {
                    testing.gameObject.SetActive(true);
                    testing.transform.position = closestEdge;
                }
                else
                {
                    testing.gameObject.SetActive(false);
                }
            }
        }
    }

}

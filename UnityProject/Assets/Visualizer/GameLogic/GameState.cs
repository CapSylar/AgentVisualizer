using System;
using System.Runtime.CompilerServices;
using UnityEngine;
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
        public GameObject _mapReference; 

        public int X;
        public int Y;

        private Map currentMap;
        
        private GameObject testing;

        void Start()
        {
            // create new map
            currentMap = new Map(_planePrefab, _mapReference,  X, Y);
            // create a new brain
            BaseBrain newBrain = new retardedBrain(currentMap);
            // create a new agent
            Agent.CreateAgent(_agentPrefab, newBrain, currentMap, 0, 0);

            testing = Instantiate(_agentPrefab);
        }

        void Update()
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(inputRay, out hit))
            {
                var point = transform.InverseTransformPoint(hit.point);
                // Debug.Log("returned from inverse point transform: " + point );

                var tile = currentMap.PointToTile(point);
                var closestEdge = tile.GetClosestEdgeWorldPos(point);
                var helloworld = tile.GetClosestEdge(point);
                
                testing.transform.position = closestEdge;
                Debug.Log("closest_edge " + helloworld);
            }
        }
    }

}

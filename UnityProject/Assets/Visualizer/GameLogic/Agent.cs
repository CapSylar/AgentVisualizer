using System.Runtime.CompilerServices;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer
{
    public class Agent : MonoBehaviour
    {
        private BaseBrain _currentBrain;
        private Map _currentMap;

        public int tileX, tileZ; // position inside the map grid in terms of tiles
        private Vector3 currentDest;
        private Vector3 gameObjectPosition; 
        
        //TODO: have to refactor these two Init functions
        void Init( BaseBrain brain , Map map , int x , int z )
        {
            _currentBrain = brain;
            _currentMap = map;
            _currentMap.SetActiveAgent(this);
            
            // set the GameObject to the desired x and z
            gameObjectPosition = gameObject.transform.position;
            currentDest = gameObject.transform.localPosition = new Vector3(x * 10 , 0, z * 10 ); // TODO: not good fix the 10 
        }

        void Init(BaseBrain brain, Map map, AgentState state)
        {
            _currentBrain = brain;
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            gameObjectPosition = gameObject.transform.position;
            currentDest = gameObject.transform.localPosition = new Vector3(state.tileX * 10 , 0, state.tileZ * 10 ); // TODO: not good fix the 10 as well
        }

        void FixedUpdate()
        {
            // if (Vector3.Distance(currentDest, gameObject.transform.position) < 0.1f) // close enough
            // {
            //     // Debug.Log("requested next destination");
            //     currentDest = _currentBrain.GetNextDest(); // get next destination
            // }
            // else // go to the next destination
            // {
            //     gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, currentDest, 0.05f);
            // }
        }

        public static Agent CreateAgent(BaseBrain brain, Map map, AgentState state)
        {
            var gameObject = Instantiate(PrefabContainer.Instance.agentPrefab);
            var component = gameObject.AddComponent<Agent>();
            
            component.Init( brain , map , state );
            return component;
        }
        public static Agent CreateAgent(BaseBrain brain , Map map ,  int x , int z)
        {
            var gameObject = Instantiate(PrefabContainer.Instance.agentPrefab , PrefabContainer.Instance.mapReference.transform ); //TODO: transform shouldn't be used here
            var component = gameObject.AddComponent<Agent>();

            component.Init( brain , map , x, z );
            return component;
        }

        public void Destroy()
        {
            Destroy(gameObject); // byebye!
        }
    }
}

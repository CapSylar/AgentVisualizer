using UnityEngine;
using Visualizer.AgentBrains;

namespace Visualizer
{
    public class Agent : MonoBehaviour
    {
        private BaseBrain _currentBrain;
        private Map _currentMap;

        private Vector3 currentDest;
        private Vector3 gameObjectPosition; 

        void Init( BaseBrain brain , Map map , int x , int z )
        {
            _currentBrain = brain;
            _currentMap = map;
            
            // set the GameObject to the desired x and z
            gameObjectPosition = gameObject.transform.position;
            currentDest = gameObject.transform.position = new Vector3(x, 0, z);
        }

        void FixedUpdate()
        {
            if (Vector3.Distance(currentDest, gameObject.transform.position) < 0.1f) // close enough
            {
                // Debug.Log("requested next destination");
                currentDest = _currentBrain.GetNextDest(); // get next destination
            }
            else // go to the next destination
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, currentDest, 0.05f);
            }
        }

        public static void CreateAgent(GameObject prefab , BaseBrain brain , Map map ,  int x , int z)
        {
            var gameObject = Instantiate(prefab);
            var component = gameObject.AddComponent<Agent>();

            component.Init( brain , map , x, z );
        }
    }
}

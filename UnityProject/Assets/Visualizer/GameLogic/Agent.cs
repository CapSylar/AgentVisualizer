using System.Collections;
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
        private Tile currentTile;
        private Tile currentDest;
        
        //TODO: have to refactor these two Init functions
        void Init( BaseBrain brain , Map map , int x , int z )
        {
            _currentBrain = brain;
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            currentTile = currentDest = _currentMap.GetTile(x, z);
            gameObject.transform.transform.position = currentTile.getWorldPosition();
        }

        void Init(BaseBrain brain, Map map, AgentState state)
        {
            _currentBrain = brain;
            _currentMap = map;
            _currentMap.SetActiveAgent(this);

            //TODO: why is valid not checked here ?
            currentTile = currentDest = _currentMap.GetTile(state.tileX, state.tileZ);
            gameObject.transform.transform.position = currentTile.getWorldPosition();
        }

        void FixedUpdate()
        {
            //TODO: clean this up!!
            if (Vector3.Distance(currentDest.getWorldPosition(), gameObject.transform.position) < 0.1f) // close enough
            {
                StopAllCoroutines();
                currentTile = currentDest; // update our position
                currentDest = _currentBrain.GetNextDest(); // get next destination

                if (currentTile != currentDest) // got new destination
                {
                    StartCoroutine("OrientAndGo");
                }
            }
        }

        IEnumerator OrientAndGo()
        {
            // get correct orientation for the agent, the Prefab should face the direction it is moving in
            var direction = currentTile.OrientationOf(currentDest);
            Debug.Log("direction is " + direction);
            var targetRotation = new Vector3(0,(int)(direction) * 90,0);

            while (Vector3.Magnitude(targetRotation - gameObject.transform.eulerAngles) > 0.5f)
            {
                gameObject.transform.eulerAngles = Vector3.Lerp(gameObject.transform.eulerAngles, targetRotation, 0.05f);
                yield return null; // wait till next frame
            }
            
            // now we can move to the destination

            while (Vector3.Distance(currentDest.gameObject.transform.position, gameObject.transform.position) > 0.05f)
            {
                gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                    currentDest.gameObject.transform.position, 0.05f);
                yield return null;
            }
            
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
        
        // forward to the brain

        public void StartAgent()
        {
            _currentBrain.Start();
        }

        public void PauseAgent()
        {
            _currentBrain.Pause();
        }

        public void ResetAgent()
        {
            _currentBrain.Reset();
        }
        
        public void Destroy()
        {
            Destroy(gameObject); // byebye!
        }
    }
}

using System.Collections;
using UnityEngine;

namespace Visualizer.AgentBrains
{
    public class GoAction : AgentAction
    {
        private Agent actor;
        private Tile destTile;
        private Transform tranform;
        private Tile currentAgentTile;
        
        // state

        private bool isDone = false;

        public GoAction( Tile dest )
        {
            destTile = dest;
        }
        
        public override void Do(Agent actor)
        {
            this.actor = actor;
            currentAgentTile = actor.CurrentTile;
            tranform = actor.gameObject.transform;

            actor.StartCoroutine(OrientAndGo());
        }
        
        private IEnumerator OrientAndGo()
        {
            // get correct orientation for the agent, the Prefab should face the direction it is moving in
            var direction = currentAgentTile.OrientationOf(destTile);
            var targetRotation = new Vector3(0,(int)(direction) * 90,0);

            while (Vector3.Magnitude(targetRotation - tranform.eulerAngles) > 0.5f)
            {
                tranform.eulerAngles = Vector3.Lerp(tranform.eulerAngles, targetRotation, 0.05f);
                yield return null; // wait till next frame
            }
            
            // now we can move to the destination

            var tileWorldPos = destTile.getTileWorldPos();

            while (Vector3.Distance(tileWorldPos, tranform.position) > 0.05f)
            {
                tranform.position = Vector3.Lerp(tranform.position,
                    tileWorldPos, 0.05f);
                yield return null;
            }

            isDone = true;
            
            // update the agent currentTile 
            actor.CurrentTile = destTile ;
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
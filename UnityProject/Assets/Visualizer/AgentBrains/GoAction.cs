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
        
        public override void Do(Agent Actor)
        {
            this.actor = Actor;
            currentAgentTile = Actor.CurrentTile;
            tranform = Actor.gameObject.transform;

            Actor.StartCoroutine(OrientAndGo());
        }
        
        private IEnumerator OrientAndGo()
        {
            // get correct orientation for the agent, the Prefab should face the direction it is moving in
            var direction = currentAgentTile.OrientationOf(destTile);
            
            var rotationInY = (int) (direction)*90;
            rotationInY = rotationInY > 180 ? rotationInY - 360 : rotationInY; // adjust,do shortest rotation

            var targetRotation = Quaternion.Euler(0,rotationInY,0);

            while (Quaternion.Angle(targetRotation,tranform.rotation)> 0.5f)
            {
                tranform.rotation = Quaternion.Lerp(tranform.rotation, targetRotation, 0.05f);
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
using System.Collections;
using UnityEngine;
using Visualizer.AgentBrains;
using Visualizer.GameLogic;

namespace Visualizer.GameLogic
{
    public class GoAction : AgentAction
    {
        private static int multiplier = 1; // current multiplier set by the agent himself for his GoActions
        
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
            actor.Steps++; // always moves from one tile to the next 
            
            // get correct orientation for the agent, the Prefab should face the direction it is moving in
            var direction = currentAgentTile.OrientationOf(destTile);
            
            var rotationInY = (int) (direction)*90;

            if (rotationInY > 0) // do we need to turn ? 
                actor.Turns++;
            
            rotationInY = rotationInY > 180 ? rotationInY - 360 : rotationInY; // adjust,do shortest rotation

            var targetRotation = Quaternion.Euler(0,rotationInY,0);

            while (Quaternion.Angle(targetRotation,tranform.rotation)> 0.5f)
            {
                tranform.rotation = Quaternion.Lerp(tranform.rotation, targetRotation, 0.04f * multiplier);
                yield return null; // wait till next frame
            }
            
            // now we can move to the destination

            var tileWorldPos = destTile.GetTileWorldPos();

            while (Vector3.Distance(tileWorldPos, tranform.position) > 0.04f)
            {
                tranform.position = Vector3.Lerp(tranform.position,
                    tileWorldPos, 0.05f * multiplier );
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

        public static void SetMultiplier( int value )
        {
            // assumes value is between 1 and 10
            multiplier = value;
        }
    }
}
using System.Collections;
using UnityEngine;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    public class GoAction : AgentAction
    {
        private static int multiplier = 1; // current multiplier set by the agent himself for his GoActions
        
        private Agent actor;
        private Tile _destTile;
        private Transform transform;
        private Tile _currentAgentTile;
        
        // state

        private bool isDone = false;

        public GoAction( Tile dest )
        {
            _destTile = dest;
        }
        
        // execute action in graphical mode
        public override void Do (GraphicalAgent actor)
        {
            this.actor = actor;
            _currentAgentTile = actor.CurrentTile;
            
            //TODO: check out this commented line
            transform = actor.GetTransform();

            // Execute the animation
            PrefabContainer.Instance.StartCoroutine(OrientAndGo());
        }

        public override void Do(Agent actor)
        {
            this.actor = actor;
            _currentAgentTile = actor.CurrentTile;
            
            ActuallyDoesIt();
        }

        private void ActuallyDoesIt()
        {
            // update the agent currentTile 
            actor.CurrentTile = _destTile ;
            isDone = true;
        }
        
        private IEnumerator OrientAndGo()
        {
            actor.Steps++; // always moves from one tile to the next 
            
            // get correct orientation for the agent, the Prefab should face the direction it is moving in
            var direction = _currentAgentTile.OrientationOf(_destTile);
            
            var rotationInY = (int) (direction)*90;
            
            rotationInY = rotationInY > 180 ? rotationInY - 360 : rotationInY; // adjust,do shortest rotation

            var targetRotation = Quaternion.Euler(0,rotationInY,0);
            
            if (Quaternion.Angle(targetRotation,transform.rotation) > 0.5f ) // do we need to turn ? 
                actor.Turns++;

            while (Quaternion.Angle(targetRotation,transform.rotation) > 0.5f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.04f * multiplier);
                yield return null; // wait till next frame
            }
            
            // now we can move to the destination

            var tileWorldPos = _destTile.GetWorldPosition();

            while (Vector3.Distance(tileWorldPos, transform.position) > 0.04f)
            {
                transform.position = Vector3.Lerp(transform.position,
                    tileWorldPos, 0.05f * multiplier );
                yield return null;
            }
            
            ActuallyDoesIt();
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
using System.Collections;
using UnityEditor;
using UnityEngine;
using Visualizer.UI;

namespace Visualizer.GameLogic.AgentMoves
{
    public class GoMove : AgentMove
    {
        private static int multiplier = 1; // current multiplier set by the agent himself for his GoActions
        
        private Agent _actor;
        
        private Tile _destTile;
        private Tile _srcTile; // adding src tile makes the move reversible
        
        private Transform _transform;
        private Tile _currentAgentTile;
        
        // state

        private bool _isDone = false;

        public GoMove( Tile src , Tile dest )
        {   
            _srcTile = src;
            _destTile = dest;
        }
        
        // execute action in graphical mode
        public override void Do (GraphicalAgent actor)
        {
            _actor = actor;
            _currentAgentTile = actor.CurrentTile;

            _transform = actor.GetTransform();

            var x = Helper();

            // Execute the animation
            PrefabContainer.Instance.StartCoroutine(OrientAndGo(x));
        }

        public override void Do(Agent actor)
        {
            _actor = actor;
            _currentAgentTile = actor.CurrentTile;

            Helper();
            
            ActuallyDoesIt();
        }

        private Quaternion Helper()
        {
            _actor.Steps++; // always moves from one tile to the next 
            
            var direction = _currentAgentTile.OrientationOf(_destTile);
            
            var rotationInY = (int) (direction)*90;
            
            rotationInY = rotationInY > 180 ? rotationInY - 360 : rotationInY; // adjust,do shortest rotation

            var targetRotation = Quaternion.Euler(0,rotationInY,0);
            
            // if (Quaternion.Angle(targetRotation,transform.rotation) > 0.5f ) // do we need to turn ? 
            //     actor.Turns++;

            return targetRotation;
        }

        private void ActuallyDoesIt()
        {
            // update the agent currentTile 
            _actor.CurrentTile = _destTile ;
            _isDone = true;
        }
        
        private IEnumerator OrientAndGo( Quaternion targetRotation )
        {
            while (Quaternion.Angle(targetRotation,_transform.rotation) > 0.5f)
            {
                _transform.rotation = Quaternion.Lerp(_transform.rotation, targetRotation, 0.04f * multiplier);
                yield return null; // wait till next frame
            }
            
            // now we can move to the destination

            var tileWorldPos = _destTile.GetWorldPosition();

            while (Vector3.Distance(tileWorldPos, _transform.position) > 0.04f)
            {
                _transform.position = Vector3.Lerp(_transform.position,
                    tileWorldPos, 0.05f * multiplier );
                yield return null;
            }
            
            ActuallyDoesIt();
        }

        public override bool IsDone()
        {
            return _isDone;
        }

        public override AgentMove GetReverse()
        {
            return new GoMove(_destTile, _srcTile);
        }

        public static void SetMultiplier( int value )
        {
            // assumes value is between 1 and 10
            multiplier = value;
        }
    }
}
using UnityEditor.ShortcutManagement;
using Visualizer.AgentBrains;
using Visualizer.GameLogic;

namespace Visualizer.GameLogic.AgentActions
{
    public class CleanDirtAction : AgentAction
    {
        // private Agent actor;
        private Tile _dirtyTile;

        public CleanDirtAction( Tile dirtyTile )
        {
            _dirtyTile = dirtyTile;
        }
        
        public override void Do(Agent actor)
        {
            ActuallyDoIt( actor );
        }

        public override void Do(GraphicalAgent actor)
        {
            ActuallyDoIt( actor );
        }

        private void ActuallyDoIt( Agent actor )
        {
            actor.CurrentBoard.SetTileDirt(_dirtyTile , false );
        }

        public override bool IsDone()
        {
            // an atomic action
            return true;
        }
    }
}
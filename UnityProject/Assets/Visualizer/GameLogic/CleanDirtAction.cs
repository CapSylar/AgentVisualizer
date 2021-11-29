using Visualizer.AgentBrains;
using Visualizer.GameLogic;

namespace Visualizer.GameLogic
{
    public class CleanDirtAction : AgentAction
    {
        // private Agent actor;
        private Tile _dirtyGraphicalTile;
        
        public CleanDirtAction( Tile dirtyGraphicalTile )
        {
            _dirtyGraphicalTile = dirtyGraphicalTile;
        }
        
        public override void Do(Agent actor)
        {
            ActuallyDoIt();
        }

        public override void Do(GraphicalAgent actor)
        {
            ActuallyDoIt();
        }

        private void ActuallyDoIt()
        {
            GameStateManager.Instance.CurrentBoard.SetTileDirt(_dirtyGraphicalTile , false );
        }

        public override bool IsDone()
        {
            // an atomic action
            return true;
        }
    }
}
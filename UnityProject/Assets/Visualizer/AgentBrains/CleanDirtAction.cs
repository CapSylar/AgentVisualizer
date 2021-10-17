using Visualizer.GameLogic;

namespace Visualizer.AgentBrains
{
    public class CleanDirtAction : AgentAction
    {
        // private Agent actor;
        private Tile _dirtyTile;
        
        public CleanDirtAction( Tile dirtyTile )
        {
            _dirtyTile = dirtyTile;
        }
        
        public override void Do(Agent Actor)
        {
            //TODO: parameter unused in this case, unclean interface
            _dirtyTile.IsDirty = false;
        }

        public override bool IsDone()
        {
            // an atomic action
            return true;
        }
    }
}
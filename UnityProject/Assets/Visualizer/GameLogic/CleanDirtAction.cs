using Visualizer.AgentBrains;
using Visualizer.GameLogic;

namespace Visualizer.GameLogic
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
            GameStateManager.Instance.currentMap.SetTileDirt(_dirtyTile , false );
        }

        public override bool IsDone()
        {
            // an atomic action
            return true;
        }
    }
}
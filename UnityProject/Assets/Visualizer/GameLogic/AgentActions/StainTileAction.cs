namespace Visualizer.GameLogic.AgentActions
{
    public class StainTileAction : AgentAction
    {
        private Tile _cleanTile;

        public StainTileAction(Tile cleanTile)
        {
            _cleanTile = cleanTile;
        }
        
        public override void Do(Agent actor)
        {
            ActuallyDoIt( actor );
        }

        public override void Do(GraphicalAgent actor)
        {
            ActuallyDoIt( actor );
        }

        public override bool IsDone()
        {
            // an atomic action
            return true;
        }

        private void ActuallyDoIt(Agent actor)
        {
            actor.CurrentBoard.SetTileDirt(_cleanTile, true);
        }
    }
}
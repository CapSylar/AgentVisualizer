namespace Visualizer.GameLogic.AgentMoves
{
    public class StainTileMove : AgentMove
    {
        private Tile _cleanTile;

        public StainTileMove(Tile cleanTile)
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

        public override AgentMove GetReverse()
        {
            return new CleanDirtMove(_cleanTile);
        }

        private void ActuallyDoIt(Agent actor)
        {
            actor.CurrentBoard.SetTileDirt(_cleanTile, true);
        }

        public override string ToString()
        {
            return $"StainTileMove:{{Tile:{_cleanTile}}}";
        }
    }
}
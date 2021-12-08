namespace Visualizer.GameLogic.AgentMoves
{
    public class CleanDirtMove : AgentMove
    {
        // private Agent actor;
        private Tile _dirtyTile;

        public CleanDirtMove( Tile dirtyTile )
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

        public override AgentMove GetReverse()
        {
            return new StainTileMove(_dirtyTile);
        }

        public override bool IsDone()
        {
            // an atomic action
            return true;
        }

        public override string ToString()
        {
            return $"CleanDirtMove:{{Tile:{_dirtyTile}}}";
        }
    }
}
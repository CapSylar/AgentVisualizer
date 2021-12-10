namespace Visualizer.GameLogic.AgentMoves
{
    public class CleanDirtMove : AgentMove
    {
        // private Agent actor;
        private Tile _dirtyTile;
        private bool _update;

        public CleanDirtMove( Tile dirtyTile , bool updateAgentMetrics = true )
        {
            _dirtyTile = dirtyTile;
            _update = updateAgentMetrics;
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
            if ( _update )
                ++actor.Cleaned;
        }

        public override AgentMove GetReverse()
        {
            return new StainTileMove(_dirtyTile , _update );
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
namespace Visualizer.GameLogic.AgentMoves
{
    public class StainTileMove : AgentMove
    {
        private Tile _cleanTile;

        private bool _update;

        public StainTileMove(Tile cleanTile , bool updateAgentMetrics = true )
        {
            _cleanTile = cleanTile;
            _update = updateAgentMetrics;
        }
        
        public override void Do(Agent actor)
        {
            ActuallyDoIt( actor);
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
            return new CleanDirtMove(_cleanTile , _update );
        }

        private void ActuallyDoIt(Agent actor )
        {
            actor.CurrentBoard.SetTileDirt(_cleanTile, true);
            if ( _update )
                ++actor.Stained;
        }

        public override string ToString()
        {
            return $"StainTileMove:{{Tile:{_cleanTile}}}";
        }
    }
}
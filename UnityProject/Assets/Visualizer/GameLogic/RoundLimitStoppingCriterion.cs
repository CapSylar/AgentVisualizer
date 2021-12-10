namespace Visualizer.GameLogic
{
    public class RoundLimitStoppingCriterion : StoppingCriterion
    {
        private readonly int _limit;
        
        public RoundLimitStoppingCriterion(int limit)
        {
            _limit = limit;
        }
        
        public override bool HasEnded(Game game)
        {
            return (game.RoundsPlayed >= _limit);
        }
    }
}
namespace Visualizer.GameLogic.Conditions
{
    public class RoundLimitStoppingCondition : StoppingCondition
    {
        private readonly int _limit;

        public RoundLimitStoppingCondition() { }

        public RoundLimitStoppingCondition(int limit)
        {
            _limit = limit;
        }
        
        public override bool HasEnded(Game game)
        {
            return (game.RoundsPlayed >= _limit);
        }

        public override GraphicalConstructor GetGraphicalConstructor()
        {
            return new RoundLimitGraphicalConstructor();
        }
    }
}
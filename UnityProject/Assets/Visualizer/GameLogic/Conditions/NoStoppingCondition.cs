namespace Visualizer.GameLogic.Conditions
{
    // always return not done, not really necessary, just makes the code much cleaner
    public class NoStoppingCondition : StoppingCondition
    {
        public NoStoppingCondition() {}
        public override bool HasEnded(Game game)
        {
            return false;
        }

        public override GraphicalConstructor GetGraphicalConstructor()
        {
            return new NoStoppingGraphicalConstructor();
        }
    }
}
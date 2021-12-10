namespace Visualizer.GameLogic
{
    // passed on to a game, to tell it when to stop
    public abstract class StoppingCriterion
    {
        public abstract bool HasEnded( Game game );
    }
}
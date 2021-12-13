using Visualizer.GameLogic;

namespace Visualizer.Algorithms.BoardEvaluation
{
    public abstract class BoardEvaluator
    {
        public abstract int Evaluate(Game game);
    }
}
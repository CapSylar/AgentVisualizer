using Visualizer.GameLogic;

namespace Visualizer.Algorithms.BoardEvaluation
{
    public class CleanCountEvaluator : BoardEvaluator
    {
        public CleanCountEvaluator() {}
        
        public override int Evaluate(Game game)
        {
            // quick and simple at first
            // just return the number of clean tiles
            var score = game.Board.CleanTiles ; 
            
            return score;
        }
    }
}
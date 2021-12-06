using Visualizer.GameLogic;

namespace Visualizer.Algorithms
{
    public class BoardEvaluator
    {
        public static int Evaluate( Game game )
        {
            // quick and simple at first
            // just return the number of clean tiles
        
            //TODO: GetAllDirtyTiles() returns a list then we just count them, too slow, get number directly
            var score = game.Board.NumOfTiles - game.Board.DirtyTiles; 
            
            return score;
        }
    }
}
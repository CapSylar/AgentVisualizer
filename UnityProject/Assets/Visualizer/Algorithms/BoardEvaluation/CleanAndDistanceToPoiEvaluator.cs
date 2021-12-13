using Visualizer.GameLogic;

namespace Visualizer.Algorithms.BoardEvaluation
{
    public class CleanAndDistanceToPoiEvaluator : BoardEvaluator
    {
        public CleanAndDistanceToPoiEvaluator() { } 

        public override int Evaluate(Game game)
        {
            const int side = 2;
            const double alpha = 0.6;
            const int bonusPoints = 20;
            var score = 0;
            
            foreach (var player in game.Players)
            {
                if (player.CurrentBrain.IsGood())
                {
                    var count = game.Board.DirtyTilesInSquare(side, player.CurrentTile);
                    score += count;
                    
                    if (count > 0)
                    {
                        score += 5 * (side - game.Board.AverageDistanceToTilesInSquare(side, player.CurrentTile, true)); // check for dirty tiles in vicinity
                    }
                    
                    score += player.CurrentTile.IsDirty ? bonusPoints : 0; // 10 additional points if he can clean right away
                }
                else  // evil player
                {
                    var count = game.Board.CleanTilesInSquare(side, player.CurrentTile);
                    score -= count;

                    if (count > 0)
                    {
                        score -= 5 * (side - game.Board.AverageDistanceToTilesInSquare(side, player.CurrentTile, false)); // check for clean tiles in vicinity
                    }
                    
                    score -= !player.CurrentTile.IsDirty ? bonusPoints : 0; // 10 additional points if he can stain right away
                }
            }
            
            return ( int ) (score * (1 - alpha) + alpha * game.Board.CleanTiles);
        }
    }
}
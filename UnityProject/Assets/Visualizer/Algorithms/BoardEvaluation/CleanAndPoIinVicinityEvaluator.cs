using Visualizer.GameLogic;

namespace Visualizer.Algorithms.BoardEvaluation
{
    public class CleanAndPoIinVicinityEvaluator : BoardEvaluator
    {
        public CleanAndPoIinVicinityEvaluator() {}
        
        public override int Evaluate(Game game)
        {
            // the score is calculated as follows
            
            const int side = 2;
            const double alpha = 0.80; // importance factor of Number of total clean tiles
            const int bonusPoints = 20;
            
            var score = 0; // get the position score of the agents
            
            foreach (var player in game.Players)
            {
                if (player.CurrentBrain.IsGood())
                {
                    score += game.Board.DirtyTilesInSquare(side, player.CurrentTile);
                    score += player.CurrentTile.IsDirty ? bonusPoints : 0; // 10 additional points if he can clean right away
                }
                else  // evil player
                {
                    score -= game.Board.CleanTilesInSquare(side, player.CurrentTile);
                    score -= !player.CurrentTile.IsDirty ? bonusPoints : 0; // 10 additional points if he can stain right away
                }
            }
            
            return ( int ) (score * (1 - alpha) + alpha * game.Board.CleanTiles);
        }
    }
}
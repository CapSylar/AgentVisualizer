using System.Resources;
using Visualizer.GameLogic;

namespace Visualizer.Algorithms
{
    public class BoardEvaluator
    {
        //TODO: should the board evaluation score be normalized ???
        
        public static int CleanCountEvaluator( Game game )
        {
            // quick and simple at first
            // just return the number of clean tiles
            var score = game.Board.CleanTiles ; 
            
            return score;
        }

        public static int CleanAndDistanceEvaluator(Game game)
        {
            // the score is calculated as follows
            // score = a * (Number of Clean tiles/Total number of tiles)  +  b * (Number of Dirty Tiles in Vicinity) - c * (Number of Clean Tiles in Vicinity)
            
            const int side = 2;
            const double alpha = 0.65; // importance factor of Number of total clean tiles

            var score = 0; // get the position score of the agents

            foreach (var player in game.Players)
            {
                if (player.CurrentBrain.IsGood())
                {
                    score += game.Board.DirtyTilesInSquare(side, player.CurrentTile);
                    score += player.CurrentTile.IsDirty ? 10 : 0; // 10 additional points if he can clean right away
                }
                else  // evil player
                {
                    score -= game.Board.CleanTilesInSquare(side, player.CurrentTile);
                    score -= !player.CurrentTile.IsDirty ? 10 : 0; // 10 additional points if he can stain right away
                }
            }
            
            return ( int ) (score * (1 - alpha) + alpha * game.Board.CleanTiles);
        }
    }
}
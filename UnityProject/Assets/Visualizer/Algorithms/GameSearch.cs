using System;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.Algorithms
{
    public static class GameSearch
    {
        private static int minimax_depth = 5;
        
        public static AgentMove MinimaxSearch( Game game , Agent player )
        {
            //TODO: for now always runs a minimizing player
            _game = game;

            AgentMove bestMove = null ;
            var best = Int32.MinValue;
            
            MoveGenerator.GenerateMoves( game.Board , player , out var moves );
            foreach (var move in moves)
            {
                player.DoMove(move);
                // evaluate the current game state
                var score = Minimax(minimax_depth, true ); // next player will maximize
                
                if (score > best)
                {
                    best = score;
                    bestMove = move;
                }
                
                player.DoMove(move.GetReverse());
            }
            
            return bestMove;
        }

        private static Game _game;
        private static int Minimax(int depth , bool maximizing )
        {
            if (depth == 0)
                return BoardEvaluator.Evaluate( _game ); // should return board evaluation

            // if (maximizing)
            // {
            //     var best = Int32.MinValue;
            //     foreach (var VARIABLE in )
            //     {
            //         var moveScore = Minimax(depth - 1, false);
            //         if (moveScore > best)
            //             best = moveScore;
            //     }
            //     
            // }
            // else // minimizing player
            // {
            //     var max = Int32.MaxValue;
            //     foreach (var VARIABLE in COLLECTION)
            //     {
            //         
            //     }
            // }

            return 0;
        }
    }
}
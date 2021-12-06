using System;
using System.Collections.Generic;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.Algorithms
{
    public static class GameSearch
    {
        private static int minimax_depth = 5;
        private static Game _game;

        public static AgentMove MinimaxSearch( Game game , Agent player )
        {
            _game = game;

            AgentMove bestMove = null ;
            var maximizer = player.CurrentBrain.IsGood() ;
            var bestScore = maximizer? int.MinValue : int.MaxValue;
            
            // generate the possible moves
            MoveGenerator.GenerateMoves( game.Board , player , out var moves );
            
            foreach (var move in moves)
            {
                player.DoMove(move);
                // evaluate the current game state
                var moveScore = Minimax(minimax_depth, game.WhoisAfter(player) ); // next player will maximize
                
                if ( (maximizer && moveScore > bestScore) || (!maximizer && moveScore < bestScore) )
                {
                    bestScore = moveScore;
                    bestMove = move; // set move as best move
                }
                
                player.DoMove(move.GetReverse());
            }
            
            return bestMove;
        }

        private static int Minimax(int depth , Agent player )
        {
            if (depth == 0)
                return BoardEvaluator.Evaluate( _game ); // should return board evaluation

            var maximizer = player.CurrentBrain.IsGood();
            MoveGenerator.GenerateMoves( _game.Board , player , out var moves );
            
            // run throught the moves and get the best score w.r.t the player

            var bestScore = maximizer ? int.MinValue : int.MaxValue;

            foreach (var move in moves)
            {
                player.DoMove(move); // do move then continue search
                var moveScore = Minimax(depth - 1, _game.WhoisAfter(player));

                player.DoMove(move.GetReverse()); // undo previously done move

                if ((maximizer && moveScore > bestScore) || (!maximizer && moveScore < bestScore))
                    bestScore = moveScore;
            }

            return bestScore;
        }
    }
}
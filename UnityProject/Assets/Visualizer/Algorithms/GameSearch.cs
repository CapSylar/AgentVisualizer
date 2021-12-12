using System;
using UnityEngine;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.Algorithms
{
    public static class GameSearch
    {
        private static int _minimaxDepth = 11;
        private static Game _game;

        private static int _boardEvaluations = 0; // for debug 

        //TODO: needs refactoring!!
        public static AgentMove MinimaxSearch( Game game , Agent player )
        {
            _boardEvaluations = 0;
            _game = game;

            AgentMove bestMove = null ;
            var maximizer = player.CurrentBrain.IsGood() ;
            var bestScore = maximizer? int.MinValue : int.MaxValue;
            
            // generate the possible moves
            MoveGenerator.GenerateMoves( game.Board , player , out var moves );

            var alpha = int.MinValue;
            var beta = int.MaxValue;
            
            foreach (var move in moves)
            {
                player.DoMove(move); 

                // evaluate the current game state
                var moveScore = Minimax(_minimaxDepth,
                    alpha , beta ,  game.WhoisAfter(player) ); // next player will maximize
                
                player.DoMove(move.GetReverse());

                if (maximizer)
                {
                    if (moveScore > bestScore) // new best score
                    {
                        bestScore = moveScore;
                        bestMove = move;
                    }
                    
                    alpha = Math.Max(moveScore, alpha);
                }
                else // minimizer
                {
                    if (moveScore < bestScore) // new best score
                    {
                        bestScore = moveScore;
                        bestMove = move;
                    }
                    
                    beta = Math.Min(moveScore, beta);
                }

                if (beta <= alpha) // check if we can prune
                    break;
            }
            
            Debug.Log("did " + _boardEvaluations + " evaluations for the next move");
            bestMove?.Reset();
            return bestMove;
        }

        // alpha is the minimum score the maximizing player is assured of
        // beta is the maximum score that the minimizing player is assured of
        private static int Minimax(int depth , int alpha , int beta , Agent player )
        {
            if (depth == 0) // evaluate and return
            {
                ++_boardEvaluations;
                var eval = BoardEvaluator.CleanAndDistanceEvaluator( _game );
                return eval;
            }

            MoveGenerator.GenerateMoves( _game.Board , player , out var moves );
            
            // run through the moves and get the best score w.r.t the player

            var maximizer = player.CurrentBrain.IsGood();
            var bestScore = maximizer ? int.MinValue : int.MaxValue;
            
            foreach (var move in moves)
            {
                player.DoMove(move); // do move then continue search

                var moveScore = Minimax(depth - 1, alpha , beta , _game.WhoisAfter(player));

                player.DoMove(move.GetReverse()); // undo previously done move
                
                if (maximizer) // maximizer
                {
                    bestScore = Math.Max(bestScore, moveScore);
                    alpha = Math.Max(moveScore, alpha);
                }
                else // minimizer
                {
                    bestScore = Math.Min(bestScore, moveScore);
                    beta = Math.Min(moveScore, beta);
                }
                
                // if it's the maximizer's turn to move, no need to explore further if we know that the minimizer had
                // a better move further up the tree, meaning it will never take this branch in the first place, do not explore it
                
                // if it's the minimizer's turn to move, beta being less than alpha also means that the maximizer had a better move further
                // up the tree, no need to continue exploring
                
                if (beta <= alpha) // check if we can prune
                {
                    break;
                }
            }

            return bestScore;
        }
    }
}
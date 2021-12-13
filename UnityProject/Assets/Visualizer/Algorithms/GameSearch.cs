using System;
using System.Collections.Generic;
using Visualizer.Algorithms.BoardEvaluation;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

//TODO: whole code is a pile of mess, refactor asap!!
//TODO: find a way to make the minimax min,max... nodes more versatile not hardcoded as they are now
namespace Visualizer.Algorithms
{
    public static class GameSearch
    {
        private static Game _game;
        
        //TODO: needs refactoring!!
        public static AgentMove MinimaxSearch( int depth , Game game , Agent player )
        {
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
                var moveScore = Minimax(depth,
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
            
            bestMove?.Reset();
            return bestMove;
        }
        
        public static AgentMove MinimaxSearchCounterNN( int depth , Game game , Agent player )
        {
            // invoked only by minimizing player
            _game = game;

            AgentMove bestMove = null ;
            var bestScore = int.MaxValue;
            
            // generate the possible moves
            MoveGenerator.GenerateMoves( game.Board , player , out var moves );

            foreach (var move in moves)
            {
                player.DoMove(move); 

                // evaluate the current game state
                var moveScore = MinimaxMaxisNn(depth, 
                    game.WhoisAfter(player) ); // next player will maximize
                
                player.DoMove(move.GetReverse());
                
                if (moveScore < bestScore) // new best score
                {
                    bestScore = moveScore;
                    bestMove = move;
                }
            }
            
            bestMove?.Reset();
            return bestMove;
        }

        // alpha is the minimum score the maximizing player is assured of
        // beta is the maximum score that the minimizing player is assured of
        private static int Minimax(int depth , int alpha , int beta , Agent player )
        {
            if (depth == 0) // evaluate and return
            {
                var eval = GameStateManager.Instance.CurrentBoardEvaluator.Evaluate( _game );
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
        
        private static int MinimaxMaxisNn (int depth , Agent player )
        {
            if (depth == 0) // evaluate and return
            {
                var eval = GameStateManager.Instance.CurrentBoardEvaluator.Evaluate( _game );
                return eval;
            }
            
            // run through the moves and get the best score w.r.t the player
            var maximizer = player.CurrentBrain.IsGood();

            List<AgentMove> moves = new List<AgentMove>();

            if (maximizer) // maximizer is a NN agent, simulate his search
            {
                if (_game.Board.DirtyTiles > 0) // agent will move to closest dirty
                {
                    Bfs.DoAvoidOccupiedBfs(_game, player.CurrentTile, tile => tile.IsDirty, out var path);

                    if (path.Count < 2) // the players current tile is dirty
                    {
                        moves.Add(new CleanDirtMove(player.CurrentTile));
                    }
                    else
                    {
                        moves.Add(new GoMove(path[0],path[1])); // only need first move
                    }
                }
                else
                {
                    var neighbors = _game.Board.GetReachableNeighbors(player.CurrentTile);

                    //TODO: needs to check that no other agent is on that Tile, else collision
                    // generate a move to each of the reachable neighbors
                    moves.Add(new GoMove(player.CurrentTile,neighbors[0]));
                }
            }
            else // normal min node, proper minimizer
            {
                MoveGenerator.GenerateMoves( _game.Board , player , out moves );
            }

            var bestScore = maximizer ? int.MinValue : int.MaxValue;
            
            foreach (var move in moves)
            {
                player.DoMove(move); // do move then continue search

                var moveScore = MinimaxMaxisNn(depth - 1, _game.WhoisAfter(player));

                player.DoMove(move.GetReverse()); // undo previously done move
                
                if (maximizer) // maximizer
                {
                    bestScore = Math.Max(bestScore, moveScore);
                }
                else // minimizer
                {
                    bestScore = Math.Min(bestScore, moveScore);
                }
            }

            return bestScore;
        }
    }
}
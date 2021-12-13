// using Visualizer.GameLogic;
//
// namespace Visualizer.Algorithms
// {
//     public class MaximizerNode
//     {
//         // implemented a minimax maximizer node
//         public int Search( Game game , int depth , int alpha , int beta , Agent player )
//         {
//             // assumes player is a good player, does not explicitly check for this condition
//             if (depth == 0) // evaluate and return
//             {
//                 var eval = BoardEvaluator.CleanAndDistanceEvaluator( game );
//                 return eval;
//             }
//             
//             MoveGenerator.GenerateMoves( game.Board , player , out var moves );
//
//             foreach (var move in moves)
//             {
//                 player.DoMove(move); // do move then continue search
//                 var moveScore = Minimax(depth - 1, alpha , beta , _game.WhoisAfter(player));
//                 player.DoMove(move.GetReverse()); // undo previously done move
//             }
//             
//             return 0;
//         }
//     }
// }
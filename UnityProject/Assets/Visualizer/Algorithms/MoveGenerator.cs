using System.Collections.Generic;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.Algorithms
{
    public static class MoveGenerator
    {
        // implements the move generator used in minimax and expectimax
        // thank god we only need to generate the moves of a single agent type

        public static void GenerateMoves ( Board board , Agent agent , out List<AgentMove> moves )
        {
            moves = new List<AgentMove>();
            var currentTile = agent.CurrentTile;
            
            // agent can move in 4 direction if not obstructed and not on the edge of the map

            var neighbors = board.GetReachableNeighbors(currentTile);
            
            //TODO: needs to check that no other agent is on that Tile, else collision
            
            // generate a move to each of the reachable neighbors
            foreach (var neighbor in neighbors)
            {
                moves.Add(new GoMove( currentTile , neighbor ));
            }
            
            // dirt placer should not stain already dirty tile, cleaner should not clean already clean tile

            var tileDirty = agent.CurrentTile.IsDirty;

            if ( !agent.CurrentBrain.IsGood() )
            {
                if (!tileDirty) // if clean, stain
                    moves.Add(new StainTileMove(currentTile));
            }
            else if ( tileDirty )
            {
                moves.Add(new CleanDirtMove(currentTile));
            }
        }
    }
}
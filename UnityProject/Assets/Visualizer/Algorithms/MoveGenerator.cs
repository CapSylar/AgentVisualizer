using System.Collections.Generic;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.Algorithms
{
    public static class MoveGenerator
    {
        // implements the move generator used in minimax and expectimax
        // thank god we only need to generate the moves of a single agent type

        //TODO: for now only supports evil agents
        public static void GenerateMoves ( Board board , Agent agent , out List<AgentMove> moves )
        {
            moves = new List<AgentMove>();
            var currentTile = agent.CurrentTile;
            
            // agent can move in 4 direction if not obstructed and not on the edge of the map
            // +1 move if the current Tile underneath is dirty , clean it

            var neighbors = board.GetReachableNeighbors(currentTile);
            // generate a move to each of the reachable neighbors
            foreach (var neighbor in neighbors)
            {
                moves.Add(new GoMove( currentTile , neighbor ));
            }
            
            if ( !agent.CurrentTile.IsDirty )
                moves.Add(new StainTileMove(currentTile));
        }
    }
}
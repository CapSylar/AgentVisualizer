using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.AgentBrains.EvilBrains
{
    public class BfsStainClosestTileFullVisibility : BaseBrain
    {
        private Board _currentBoard;
        private Agent _actor;
        
        public BfsStainClosestTileFullVisibility(Board board)
        {
            _currentBoard = board;
        }

        public override void Start(Agent actor)
        {
            _actor = actor;
            GenerateMove(); // call first time to set in motion
        }

        // gets called on each turn
        public override void Update()
        {
            GenerateMove();
        }

        public override bool IsGood()
        {
            return false;
        }

        private void GenerateMove()
        {
            // get the closest clean tile
            var found = Bfs.DoAvoidOccupiedBfs( _actor.CurrentGame , _actor.CurrentTile , tile => !tile.IsDirty , out var path  );
            
            if (found)
            {
                // generate commands
                if (path.Count < 2) // we are on the dirty tile
                {
                    Commands.Enqueue(new StainTileMove(_actor.CurrentTile));
                }
                else // start going to it
                {
                    Commands.Enqueue(new GoMove(path[0],path[1]));
                }
            }
        }
    }
}
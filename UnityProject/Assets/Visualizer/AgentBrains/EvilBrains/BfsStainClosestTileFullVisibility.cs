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
            //TODO: fix this ugly hack 
            if (Commands.Count > 0)
                return;
            
            // get the closest dirty tile and go to it
            var found = Bfs.DoBfs( _currentBoard , _actor.CurrentTile , tile => !tile.IsDirty , out List<Tile> path  );
            
            if (found)
            {
                // generate commands
                PathToMoveCommands( path , Commands );
                // if not path, then the closest clean tile is the one we are standing on
                Commands.Enqueue(new StainTileMove(path[path.Count-1]));
            }
        }
    }
}
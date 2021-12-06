using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentMoves;

namespace Visualizer.AgentBrains.GoodBrains
{
    // this brain does not assume a static environment like the rest
    // recalculates the nearest tile at each turn
    public class BfsCleanNearestTileDynamic : BaseBrain
    {
        private Board _currentBoard;
        private Agent _actor;
        
        // internal state

        private Tile _currentDest = null;

        public BfsCleanNearestTileDynamic(Board board)
        {
            _currentBoard = board;
        }
        public override void Start(Agent actor)
        {
            _actor = actor;
            GenerateMove(); // call it the first time to set in motion
        }
        
        private void GenerateMove()
        {
            // first get the closest Tile
            var dirtyTiles = _currentBoard.GetAllDirtyTiles();
            if (dirtyTiles.Count > 0)
            {
                var closestDirty = TspNearestNeighborFullVisibility.GetNearestDirty(_currentBoard,
                    _currentBoard.GetAllDirtyTiles(), _actor.CurrentTile );

                // check if we need to change destination
                if (_currentDest == null || _currentDest != closestDirty)
                {
                    Commands.Clear();
                    // plot a new path
                    Bfs.DoBfs(_currentBoard, _actor.CurrentTile, tile => tile.IsDirty, out List<Tile> path);
                    PathToMoveCommands( path , Commands );
                    Commands.Enqueue(new CleanDirtMove(closestDirty));
                }
            }
        }
        
        // gets called at each turn
        public override void Update()
        {
            GenerateMove();
        }
    }
}
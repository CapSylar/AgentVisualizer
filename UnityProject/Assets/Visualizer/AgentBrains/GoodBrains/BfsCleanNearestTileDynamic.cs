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
            // get the closest dirty tile and go to it
            var found = Bfs.DoAvoidOccupiedBfs( _actor.CurrentGame , _actor.CurrentTile , tile => tile.IsDirty , out var path  );
            
            if (found)
            {
                // generate commands
                if (path.Count < 2) // we are on the dirty tile
                {
                    Commands.Enqueue(new CleanDirtMove(_actor.CurrentTile));
                }
                else // start going to it
                {
                    Commands.Enqueue(new GoMove(path[0],path[1]));
                }
            }
        }
        
        // gets called at each turn
        public override void Update()
        {
            GenerateMove();
        }

        public override string ToString()
        {
            return "Brain:{BFS Clean Nearest tile Dynamic}";
        }
    }
}
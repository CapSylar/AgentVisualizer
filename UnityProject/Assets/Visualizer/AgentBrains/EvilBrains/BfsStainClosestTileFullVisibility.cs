using System.Collections.Generic;
using Visualizer.Algorithms;
using Visualizer.GameLogic;
using Visualizer.GameLogic.AgentActions;

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
            _actor.HookToEventOnActionDone(GenerateMove);
            GenerateMove(); // can first time to set in motion
        }
        
        private void GenerateMove()
        {
            //TODO: fix this ugly hack 
            if (Commands.Count > 0)
                return;
            
            // get the closest dirty tile and go to it
            var found = Bfs.DoBfs( _currentBoard , _actor.CurrentTile , tile => !tile.IsDirty , out List<Tile> path  );
            
            path.RemoveAt(0);

            if (found)
            {
                // generate commands
                foreach (var tile in path)
                {
                    Commands.Enqueue(new GoAction(tile));
                }
                
                // if not path, then the closest clean tile is the one we are standing on
                Commands.Enqueue(new StainTileAction( path.Count > 0 ?
                    path[path.Count-1] : _actor.CurrentTile ));
            }
        }

        public override void Reset()
        {
            _actor.UnHookEventOnActionDone(GenerateMove);
            base.Reset();
        }
    }
}
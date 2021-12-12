using Visualizer.Algorithms;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains.EvilBrains
{
    public class MinimizerMinimaxFullVisibility : BaseBrain
    {
        private Board _currentBoard;
        private Agent _actor;

        public MinimizerMinimaxFullVisibility(Board board)
        {
            _currentBoard = board;
        }

        public override void Start(Agent actor)
        {
            _actor = actor;
        }
        
        // called once per turn 
        public override void Update()
        {
            var bestMove = GameSearch.MinimaxSearch(_actor.CurrentGame, _actor);
            
            Commands.Enqueue(bestMove);
            base.Update();
        }

        public override bool IsGood()
        {
            return false;
        }

        public override string ToString()
        {
            return "Brain:{Minimizer Minimax}";
        }
    }
}
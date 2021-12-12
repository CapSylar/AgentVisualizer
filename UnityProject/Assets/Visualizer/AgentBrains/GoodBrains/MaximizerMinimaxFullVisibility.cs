using Visualizer.Algorithms;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains.GoodBrains
{
    public class MaximizerMinimaxFullVisibility : BaseBrain
    {
        private Agent _actor;
        private Board _currentBoard;

        public MaximizerMinimaxFullVisibility(Board board)
        {
            _currentBoard = board;
        }
        
        public override void Start(Agent actor)
        {
            _actor = actor;
        }
        
        public override void Update()
        {
            var bestMove = GameSearch.MinimaxSearch(_actor.CurrentGame, _actor);
            
            Commands.Enqueue(bestMove);
            base.Update();
        }

        public override string ToString()
        {
            return "Brain:{Maximizer Minimax}";
        }
    }
}
using Visualizer.Algorithms;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains.EvilBrains
{
    // Minimizer Minimax which can effectively counter a Nearest Neighbour cleaner
    
    public class MinimizerMiniMaxCounterNN : BaseBrain
    {
        private Board _currentBoard;
        private Agent _actor;

        public MinimizerMiniMaxCounterNN(Board board)
        {
            _currentBoard = board;
        }

        public override void Start(Agent actor)
        {
            _actor = actor;
        }
        
        
        //called once per turn
        public override void Update()
        {
            var bestMove = GameSearch.MinimaxSearchCounterNN( 5 , _actor.CurrentGame, _actor);

            Commands.Enqueue(bestMove);
            base.Update();
        }

        public override bool IsGood()
        {
            return false;
        }

        public override string ToString()
        {
            return "Brain:{Minimizer countering NN Minimax}";
        }
    }
}
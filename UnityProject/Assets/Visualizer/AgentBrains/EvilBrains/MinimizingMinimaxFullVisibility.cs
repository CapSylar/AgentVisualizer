using UnityEngine;
using Visualizer.Algorithms;
using Visualizer.GameLogic;

namespace Visualizer.AgentBrains.EvilBrains
{
    public class MinimizingMinimaxFullVisibility : BaseBrain
    {
        private Board _currentBoard;
        private Agent _actor;

        public MinimizingMinimaxFullVisibility(Board board)
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
        }

        public override bool IsGood()
        {
            return false;
        }
    }
}
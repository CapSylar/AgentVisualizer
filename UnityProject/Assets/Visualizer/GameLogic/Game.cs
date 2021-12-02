using System.Collections.Generic;

namespace Visualizer.GameLogic
{
    // Game represents a game => { players , board , rules... }
    public class Game
    {
        private List<Agent> _players ;
        private Board _board ;

        public Game ( Board board , List<Agent> players )
        {
            _players = players;
            _board = board;
            
            // Start all the agents
            foreach (var agent in _players)
            {
                agent.Start();
            }
        }

        public Game ( Board board, params Agent[] players)
        {
            _players = new List<Agent>(players);
            _board = board;
            
            foreach (var agent in _players)
            {
                agent.Start();
            }
        }

        // does one "round" of playing, like in a game of cards, every agent runs once as if it was his turn to move
        public void Update()
        {
            // give agent a turn to move

            foreach (var agent in _players)
            {
                agent.Update(); // do a move
            }
        }

        public void Reset()
        {
            // reset the game, board and players

            foreach (var agent in _players)
            {
                agent.Reset();
            }
            
            //TODO: implement this
            //board.Reset()
        }
    }
}
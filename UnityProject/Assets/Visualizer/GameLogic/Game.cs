using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations;

namespace Visualizer.GameLogic
{
    // Game represents a game => { players , board , rules... }
    public class Game
    {
        public List<Agent> Players { get; }
        
        public Board Board { get; }
        
        // internal state
        
        private int _nextTurn = 0; // points to player to play next
        private Agent _lastPlayer;

        public Game ( Board board , IEnumerable<Agent> players )
        {
            Players = new List<Agent>(players);
            Board = board;

            _lastPlayer = Players[0]; // just to init, does not matter
            
            // Start all the agents
            foreach (var agent in Players)
            {
                agent.Start();
            }
        }

        public Game ( Board board, params Agent[] players ) : this ( board , players.ToList() ) { }

        // the next player in the queue plays his round
        public void PlayTurn()
        {
            // first make sure that the last player that player is done with his move
            if (_lastPlayer.IsDone())
            {
                var player = NextAgent();
                player.Update(); // player plays his move
                _lastPlayer = player;
            }
        }

        public Agent NextAgent()
        {
            var toReturn = Players[_nextTurn];
            _nextTurn = ++_nextTurn % Players.Count;

            return toReturn;
        }
        public Agent PeekNextAgent () // who will play the next turn ? 
        {
            return Players[_nextTurn];
        }

        public Agent PeekPreviousAgent()
        {
            return Players[_nextTurn--];
        }
        

        public void Reset()
        {
            // reset the game, board and players
            foreach (var agent in Players)
            {
                agent.Reset();
            }
            
            //TODO: implement this
            //board.Reset()
        }
    }
}
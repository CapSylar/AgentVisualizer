using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
                agent.Start( this );
            }
            
            
            Debug.Log(ToString());
        }

        public Game ( Board board, params Agent[] players ) : this ( board , players.ToList() ) { }

        // the next player in the queue plays his round
        public void PlayTurn()
        {
            // first make sure that the last player that player is done with his move
            if (_lastPlayer.IsDone())
            {
                var player = GetNextAgent();
                player.Update(); // player plays his move
                _lastPlayer = player;
            }
        }

        private Agent GetNextAgent()
        {
            var toReturn = Players[_nextTurn];
            _nextTurn = ++_nextTurn % Players.Count;

            return toReturn;
        }
        
        public Agent WhoisAfter ( Agent player ) // who will play the next turn after Agent ? 
        {
            //TODO: IndexOf O(N) not good!! fix!
            var index = Players.IndexOf(player);
            
            return Players[ ++index % Players.Count ];
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


        public override string ToString()
        {
            // prints the board state for debug
            
            var debugMessage = Players.Aggregate("", (current, player) => current + player + "\n" );
            
            return "Game:{ \n" +
                   debugMessage +
                   Board+ "\n" +
                   "}";
        }
    }
}
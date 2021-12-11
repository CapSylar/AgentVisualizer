using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Visualizer.GameLogic.Conditions;

namespace Visualizer.GameLogic
{
    // Game represents a game => { players , board , rules... }
    public class Game
    {
        public List<Agent> Players { get; }
        
        public Board Board { get; }
        
        // internal state

        public  int RoundsPlayed { get; private set; } // never to be set explicitly
        
        private int _turnsPlayed;
        public int TurnsPlayed
        {
            get => _turnsPlayed;
            private set
            {
                _turnsPlayed = value;
                RoundsPlayed = _turnsPlayed / Players.Count;
            }
        }

        private int _nextTurn = 0; // points to player to play next
        private Agent _lastPlayer;

        private StoppingCondition _currentCondition;

        public Game ( Board board , IEnumerable<Agent> players , StoppingCondition condition = null )
        {
            Players = new List<Agent>(players);
            Board = board;

            _lastPlayer = Players[Players.Count-1]; // just to init, doesn't really matter
            
            // Start all the agents
            foreach (var agent in Players)
            {
                agent.Start( this );
            }

            RoundsPlayed = 0;
            TurnsPlayed = 0;

            _currentCondition = condition;
        }
        
        // plays a whole round, each player gets a turn
        public void PlayRound()
        {
            // TODO:for now only check for game end conditions here
            if (HasEnded()) // could be removed, since game could run past this point
                return;

            for (var i = 0; i < Players.Count; ++i)
            {
                PlayTurn();
            }
        }
        
        // the next player in the queue plays his round
        public void PlayTurn()
        {
            // first make sure that the last player that player is done with his move
            // some moves take time to complete, since they could animations associated with them
            if (HasEnded())  // could be removed, since game could run past this point
                return;
            
            if (_lastPlayer.IsDone())
            {
                ++TurnsPlayed;

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

            RoundsPlayed = 0;

            //TODO: implement this
            //board.Reset()
        }

        // returns true if the game has ended
        // a game ends according to some condition
        public bool HasEnded()
        {
            return (_currentCondition != null &&
                    _currentCondition.HasEnded(this)); // has the game met the stopping criterion
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
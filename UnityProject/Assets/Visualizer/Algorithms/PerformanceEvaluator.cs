using System;
using Visualizer.GameLogic;

namespace Visualizer.Algorithms
{
    // get the final game state, and decides on the winner
    // if we have several dirt makers and cleaners, get a winner from both teams
    
    public static class PerformanceEvaluator
    {
        // returns the best agent, cleaner or dirt maker
        public static Agent GetBestAgent(Game game)
        {
            Agent bestPlayer = null;
            var bestPlayerScore = Int32.MinValue;

            foreach (var player in game.Players)
            {
                var score = EvaluateAgent(player); // evaluate player

                if (score > bestPlayerScore)
                {
                    bestPlayerScore = score;
                    bestPlayer = player;
                }
            }
            
            return bestPlayer;
        }

        // returns the best from the both teams, if any
        //TODO: return type ayayayayaya
        public static Tuple<Tuple<Agent,int>, Tuple<Agent,int>> GetBestTeamWise(Game game)
        {
            // method assumes the game contains 2 teams
            var goodScore = int.MinValue;
            var evilScore = int.MinValue;

            Agent bestGoodAgent = null;
            Agent bestEvilAgent = null;

            foreach (var player in game.Players)
            {
                var score = EvaluateAgent(player);
                
                if (player.CurrentBrain.IsGood() ) // good player
                {
                    if (score > goodScore)
                    {
                        goodScore = score;
                        bestGoodAgent = player;
                    }
                }
                else if ( score > evilScore )// evil player
                {
                    evilScore = score;
                    bestEvilAgent = player;
                }
            }
            
            return Tuple.Create(Tuple.Create(bestGoodAgent,goodScore), Tuple.Create(bestEvilAgent,evilScore));
        }

        private static int EvaluateAgent(Agent agent)
        {
            // gives a score to the agent based on its performance
            var score = 0;
            var alpha = 0.90;
            
            //TODO: for now only take into account the number of cleaned or stained

            var actionScore = agent.CurrentBrain.IsGood() ?  agent.Cleaned : agent.Stained;
            
            score = 10 * (int) (alpha * (actionScore) + (1 - alpha) * ( -agent.Steps ) );
            return score;
        }
    }
}
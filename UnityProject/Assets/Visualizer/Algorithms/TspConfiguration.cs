using System;
using System.Collections.Generic;

namespace Visualizer.Algorithms
{
    public class TspConfiguration
    {
        // essentially TspConfiguration is just a wrapper class for a List used in the TSP implementation
        // it wraps around the DS that keeps track of all the Tiles in the current Path
        // and provides facilities to calculate the Route distance, to reshuffle, and to get a neighboring config

        private Random rng;
        private List<Tile> _route;

        public List<Tile> Route
        {
            get => _route;
            set => _route = value;
        }
        
        public TspConfiguration( List<Tile> tiles )
        {
            _route = new List<Tile>(tiles);
            rng = new Random(); // used for shuffle...
        }

        public void Shuffle()
        {
            Shuffle(0,Route.Count);
        }

        public void Shuffle( int start , int end ) // end is exclusive
        {   
            //Fisher-Yates shuffle
            var n = end - start;
            while (n > 1)
            {
                --n;
                var k = rng.Next(start,end); // tile of agent stays at position 0 no matter what

                (_route[k], _route[n]) = (_route[n], _route[k]); // swap
            }
        }

        public TspConfiguration GetSimilarConfiguration()
        {
            // same as _route, but has two random cities swapped in order
            List<Tile> newConfig = new List<Tile>(_route);

            var city1 = rng.Next(_route.Count);
            var city2 = rng.Next(_route.Count);
            
            (newConfig[city1], newConfig[city2]) = (newConfig[city2] , newConfig[city1]); // swap 'em!
            return new TspConfiguration(newConfig);
        }

        // TODO: mappings is slow and ugly as shit, fix API
        public int GetRouteLength( int[,] distances , List<Tile> mappings )
        {
            var sum = 0;
            // get the adjacent cities on the route using the distances matrix
            for (int i = 0; i < _route.Count-1 ; ++i)
            {
                // get distance from i to i+1 city
                sum += distances[mappings.IndexOf(_route[i]), mappings.IndexOf(_route[i+1])];
            }

            return sum;
        }

        public int GetRouteCityCount()
        {
            return Route.Count;
        }
        
    }
}
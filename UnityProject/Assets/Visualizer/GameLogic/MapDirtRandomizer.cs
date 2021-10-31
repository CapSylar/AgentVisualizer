using UnityEngine;
using Random = System.Random;

namespace Visualizer.GameLogic
{
    public static class MapDirtRandomizer
    {
        // ratio = ratio of dirty tiles to clean tiles
        // ratio assumes ratio is between 0 and 1, no checks done
        public static void Randomize( Map map , double ratio )
        {
            // just generate randomly, without a pattern
            // can pass a map that is already populated, doesn't matter for Randomize()

            var sizeX = map.Grid.GetLength(0);
            var sizeZ = map.Grid.GetLength(1);

            int numOfDirts = (int) (sizeX * sizeZ * ratio); // scales with map

            Random rnd = new Random();
            
            for (int i = 0; i < numOfDirts; ++i)
            {
                // get a random tile
                var theChoseOne = map.GetTile((int) (rnd.NextDouble() * sizeX), (int)(rnd.NextDouble() * sizeZ));
                map.SetTileDirt(theChoseOne , true );
            }
        }
    }
}
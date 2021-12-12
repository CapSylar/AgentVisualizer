using System;

namespace Visualizer.GameLogic
{
    public enum TILE_EDGE // assuming z is looking up and x to the right and we are looking down in 2D
    {
        UP = 0,
        RIGHT = 1,
        DOWN = 2 ,
        LEFT = 3
    }

    static class TileEdgeExtension
    {
        public static TILE_EDGE GetOpposite( this TILE_EDGE edge )
        {
            return (TILE_EDGE) (((int) edge + 2) % 4); // get opposite direction of edge
        }

        public static TILE_EDGE GetRandom(this TILE_EDGE edge , System.Random rnd )
        {
            return (TILE_EDGE) rnd.Next(0,4); // get a random direction
        }

        public static TILE_EDGE GetNext(this TILE_EDGE edge)
        {
            return (TILE_EDGE) (((int) edge + 1) % 4); // next direction in clockwise motion
        }

        public static TILE_EDGE GetPrevious(this TILE_EDGE edge)
        {
            return (TILE_EDGE) (((int) edge + 3) % 4); // previous direction in clockwise motion

        }
    }
}
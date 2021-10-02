using System;

namespace Visualizer
{
    [Serializable()]
    public class TileState
    {
        // owns all the data for a tile and can be serialized
        public bool _isDirty;
        public bool[] hasWallOnEdge = new bool[4]; // UP, DOWN , RIGHT , lEFT
    }
}

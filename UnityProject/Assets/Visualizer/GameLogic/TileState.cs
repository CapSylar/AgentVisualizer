using System;
using UnityEngine.Serialization;

namespace Visualizer
{
    [Serializable()]
    public class TileState
    {
        // owns all the data for a tile and can be serialized
        public bool isDirty;
        public bool[] hasWallOnEdge = new bool[4]; // UP, DOWN , RIGHT , lEFT
    }
}

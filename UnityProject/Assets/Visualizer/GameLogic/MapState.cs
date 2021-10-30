using System;
using System.IO.MemoryMappedFiles;

namespace Visualizer.GameLogic
{
    [Serializable()]
    public class MapState
    {
        // contains all the Data needed to load/save a map
        
        public TileState[,] stateGrid;

        public MapState( Map map )
        {
            // create grid 
            var grid = map.Grid;
            stateGrid = new TileState[grid.GetLength(0), grid.GetLength(1)];
            
            // get all the tile states
            for ( int i = 0 ; i < grid.GetLength(0) ; ++i )
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                stateGrid[i, j] = grid[i, j].GetTileStateCopy();
            }
        }
        
    }
}
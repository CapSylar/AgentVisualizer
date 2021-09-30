using UnityEngine;

namespace Visualizer
{
    public class Map
    {
        public Tile[,] _grid ;

        public Map( GameObject tilePrefab , int sizeX, int sizeZ)
        {
            _grid = new Tile[sizeX,sizeZ];

            for (int i = 0; i < _grid.GetLength(0); ++i)
            {
                for (int j = 0; j < _grid.GetLength(1); ++j)
                {
                    _grid[i,j] = Tile.CreateTile( tilePrefab , i , j );
                }
            }
        }
        
    }
}

using UnityEngine;

namespace Visualizer
{
    public class Map
    {
        public Tile[,] _grid ;

        private GameObject _mapGameObject;

        public Map( GameObject tilePrefab , GameObject mapGameObject , int sizeX, int sizeZ)
        {
            _grid = new Tile[sizeX,sizeZ];
            _mapGameObject = mapGameObject;

            for (int i = 0; i < _grid.GetLength(0); ++i)
            {
                for (int j = 0; j < _grid.GetLength(1); ++j)
                {
                    _grid[i,j] = Tile.CreateTile( tilePrefab , _mapGameObject.transform , i , j );
                    _grid[i,j].gameObject.transform.SetParent(_mapGameObject.transform, false);
                }
            }
        }

        public Tile PointToTile( Vector3 point )
        {
            // find out on which tile this point lies 
            var xIndex = (int) point.x / 10; // TODO: remove magic numbers !
            var zIndex = (int) point.z / 10;
            
            // Debug.Log("x: " + xIndex + " z: " + zIndex );
            
            return _grid[xIndex , zIndex];
        }
        
    }
}

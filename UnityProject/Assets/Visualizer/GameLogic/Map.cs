using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Visualizer
{
    [Serializable()]
    public class Map
    {
        public Tile[,] _grid ;
        
        [field:NonSerialized()]
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

        public Map(GameObject tilePrefab, GameObject mapGameObject, TileState[,] stateGrid)
        {
            // create the tile grid from the grid of states
            _grid = new Tile[stateGrid.GetLength(0), stateGrid.GetLength(1)];

            for (int i = 0; i < _grid.GetLength(0); ++i)
            for (int j = 0; j < _grid.GetLength(1); ++j)
            {
                // create Tile with loaded state
                _grid[i, j] = Tile.CreateTile(tilePrefab, _mapGameObject.transform, i, j , stateGrid[i,j]);
                _grid[i, j].gameObject.transform.SetParent(_mapGameObject.transform, false);
            }
        }

        public void placeWall( int tileX , int tileY , TILE_EDGE edge )
        {
            var referenceTile = _grid[tileX, tileY];
            referenceTile.setWall(edge, true);
            // update the adjacent tile, it has the wall in the opposite direction

            var opposite = edge.getOpposite(); 
            //TODO: can this be written in a better way?
            switch (edge)
            {
                case TILE_EDGE.UP:
                    _grid[tileX, tileY + 1].setWall(opposite , true);
                    break;
                case TILE_EDGE.DOWN:
                    _grid[tileX, tileY - 1].setWall(opposite , true);
                    break;
                case TILE_EDGE.RIGHT:
                    _grid[tileX+1, tileY].setWall(opposite , true);
                    break;
                case TILE_EDGE.LEFT:
                    _grid[tileX-1, tileY].setWall(opposite , true);
                    break;
            }
        }

        public void refreshMap()
        {
            //TODO: implement this
        }

        public Tile PointToTile( Vector3 point )
        {
            // find out on which tile this point lies 
            var xIndex = (int) point.x / 10; // TODO: remove magic numbers !
            var zIndex = (int) point.z / 10;
            
            // Debug.Log("x: " + xIndex + " z: " + zIndex );
            
            return _grid[xIndex , zIndex];
        }

        
        public void SaveMap( string filepath )
        {
            // save the map
            Stream saveFileStream = File.Create(filepath);
            BinaryFormatter serializer = new BinaryFormatter();
            
            // save grid of states
            TileState[,] stategrid = new TileState[_grid.GetLength(0),_grid.GetLength(1)];
            
            for ( int i = 0 ; i < _grid.GetLength(0) ; ++i )
            for (int j = 0; j < _grid.GetLength(1); ++j)
                stategrid[i, j] = _grid[i, j].getTileState();
            
            serializer.Serialize(saveFileStream, stategrid); // serialize it
            saveFileStream.Close();
        }

        public static Map LoadMap( string filePath , GameObject tilePrefab , GameObject mapGameObject )
        {
            Map toReturn = null ; 
            // load map from file
            if (File.Exists(filePath))
            {
                Stream openFileStream = File.OpenRead(filePath);
                BinaryFormatter deserializer = new BinaryFormatter();
                TileState[,] stateGrid =  (TileState[,])deserializer.Deserialize(openFileStream);
                toReturn = new Map( tilePrefab , mapGameObject , stateGrid);
                openFileStream.Close();
            }

            return toReturn;
        }

    }
}

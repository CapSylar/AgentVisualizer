using System;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Visualizer
{
    [Serializable()]
    public class Map
    {
        public Tile[,] Grid ;
        public int sizeX , sizeZ; // not actual units, just the number if tiles in each direction
        
        [field:NonSerialized()]
        private GameObject _mapGameObject;
        
        public Map( GameObject tilePrefab , GameObject mapGameObject , int sizeX, int sizeZ)
        {
            Grid = new Tile[sizeX,sizeZ];
            _mapGameObject = mapGameObject;
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;

            for (int i = 0; i < Grid.GetLength(0); ++i)
            {
                for (int j = 0; j < Grid.GetLength(1); ++j)
                {
                    Grid[i,j] = Tile.CreateTile( tilePrefab , _mapGameObject.transform , i , j );
                    Grid[i,j].gameObject.transform.SetParent(_mapGameObject.transform, false);
                }
            }
        }

        public Map(GameObject tilePrefab, GameObject mapGameObject, TileState[,] stateGrid)
        {
            // create the tile grid from the grid of states
            Grid = new Tile[stateGrid.GetLength(0), stateGrid.GetLength(1)];
            _mapGameObject = mapGameObject;
            this.sizeX = stateGrid.GetLength(0);
            this.sizeZ = stateGrid.GetLength(1);

            for (int i = 0; i < Grid.GetLength(0); ++i)
            for (int j = 0; j < Grid.GetLength(1); ++j)
            {
                // create Tile with loaded state
                Grid[i, j] = Tile.CreateTile(tilePrefab, _mapGameObject.transform, i, j , stateGrid[i,j]);
                Grid[i, j].gameObject.transform.SetParent(_mapGameObject.transform, false);
            }
            
            Refresh(); // draw map graphics
        }

        public void placeWall( int tileX , int tileY , TILE_EDGE edge )
        {
            var referenceTile = Grid[tileX, tileY];
            referenceTile.setWall(edge, true);
            // update the adjacent tile, it has the wall in the opposite direction

            var opposite = edge.getOpposite(); 
            //TODO: can this be written in a better way?
            switch (edge)
            {
                case TILE_EDGE.UP:
                    Grid[tileX, tileY + 1].setWall(opposite , true);
                    break;
                case TILE_EDGE.DOWN:
                    Grid[tileX, tileY - 1].setWall(opposite , true);
                    break;
                case TILE_EDGE.RIGHT:
                    Grid[tileX+1, tileY].setWall(opposite , true);
                    break;
                case TILE_EDGE.LEFT:
                    Grid[tileX-1, tileY].setWall(opposite , true);
                    break;
            }
        }
        
        public Tile PointToTile( Vector3 point )
        {
            // find out on which tile this point lies 
            var xIndex = (int) point.x / 10; // TODO: remove magic numbers !
            var zIndex = (int) point.z / 10;
            
            // Debug.Log("x: " + xIndex + " z: " + zIndex );
            
            return Grid[xIndex , zIndex];
        }

        public void SetTileDirtState(Tile tile , bool isDirty )
        {
            tile.IsDirty = isDirty;
        }

        public void setTileWall(Tile tile, TILE_EDGE direction , bool state )
        {
            // each tile is only responsible for the upper and right walls
            // if we want a lower wall on the current tile we have to assign the upper on the tile below
            tile.setWall(direction , state);
            
            // walls are between two tiles, set the other tile that wasn't directly selected
            switch (direction)
            {
                case TILE_EDGE.UP:
                    getUp(tile).setWall(direction.getOpposite(),state);
                    break;
                case TILE_EDGE.DOWN:
                    getDown(tile).setWall(direction.getOpposite(),state);
                    break;
                case TILE_EDGE.LEFT:
                    getLeft(tile).setWall(direction.getOpposite(), state);
                    break;
                case TILE_EDGE.RIGHT :
                    getRight(tile).setWall(direction.getOpposite(), state);
                    break;
            }
        }

        public Tile getLeft(Tile tile)
        {
            return ( tile.x > 0 ) ? Grid[tile.x-1,tile.z] : null;
        }

        public Tile getRight ( Tile tile )
        {
            return ( tile.x < sizeX-1 ) ? Grid[tile.x+1,tile.z] : null;
        }

        public Tile getUp(Tile tile)
        {
            return (tile.z < sizeZ-1) ? Grid[tile.x,tile.z+1] : null;
        }

        public Tile getDown(Tile tile)
        {
            return (tile.z > 0) ? Grid[tile.x,tile.z-1] : null;
        }

        public Vector3 getClosestEdgeWorldPos( Vector3 point )
        {
            // assume the point is on the Map
            var tile = PointToTile(point);
            return tile.GetClosestEdgeWorldPos(point);
        }

        public bool isEdgeOnMapBorder( Vector3 edge )
        {
            // if edge has a zero in X or Z or Max value of X or Z then it's on the border
            return (edge.x == 0 || edge.z == 0 || edge.x == (sizeX * 10) || edge.z == (sizeZ * 10)); 
        }
        
        public void SaveMap( string filepath )
        {
            // save the map
            Stream saveFileStream = File.Create(filepath);
            BinaryFormatter serializer = new BinaryFormatter();
            
            // save grid of states
            TileState[,] stategrid = new TileState[Grid.GetLength(0),Grid.GetLength(1)];
            
            for ( int i = 0 ; i < Grid.GetLength(0) ; ++i )
            for (int j = 0; j < Grid.GetLength(1); ++j)
                stategrid[i, j] = Grid[i, j].getTileState();
            
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
        
        // just for testing
        public void Refresh()
        {
            // refresh every tile
            for (int i = 0; i < Grid.GetLength(0); ++i)
            {
                for (int j = 0; j < Grid.GetLength(1); ++j)
                {
                    Grid[i,j].Refresh(); // for the lolz
                }
            }
        }
    }
}

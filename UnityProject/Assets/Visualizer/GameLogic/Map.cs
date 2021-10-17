using System;
using System.Collections.Generic;
using UnityEngine;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    public class Map
    {
        // the floor, the actual grid of tiles which also contain the position of the walls
        public Tile[,] Grid ;
        // reference to the Agent
        public Agent agent = null;
        
        public int sizeX , sizeZ; // not actual units, just the number of tiles in each direction

        public Map(int sizeX, int sizeZ)
        {
            Grid = new Tile[sizeX,sizeZ];
            this.sizeX = sizeX;
            this.sizeZ = sizeZ;

            var mapGameObject = PrefabContainer.Instance.mapReference;

            for (var i = 0; i < Grid.GetLength(0); ++i)
            {
                for (var j = 0; j < Grid.GetLength(1); ++j)
                {
                    Grid[i,j] = Tile.CreateTile(mapGameObject.transform , i , j );
                    Grid[i,j].gameObject.transform.SetParent(mapGameObject.transform, false);
                }
            }
        }

        public Map(TileState[,] stateGrid)
        {
            // create the tile grid from the grid of states
            Grid = new Tile[stateGrid.GetLength(0), stateGrid.GetLength(1)];
            this.sizeX = stateGrid.GetLength(0);
            this.sizeZ = stateGrid.GetLength(1);
            
            var mapGameObject = PrefabContainer.Instance.mapReference;

            for (var i = 0; i < Grid.GetLength(0); ++i)
            for (var j = 0; j < Grid.GetLength(1); ++j)
            {
                // create Tile with loaded state
                Grid[i, j] = Tile.CreateTile(mapGameObject.transform, i, j , stateGrid[i,j]);
                Grid[i, j].gameObject.transform.SetParent(mapGameObject.transform, false);
            }
            
            Refresh(); // draw map graphics
        }

        public void PlaceWall( int tileX , int tileY , TILE_EDGE edge )
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

        public void  SetActiveAgent( Agent agent )
        {
            this.agent = agent; 
        }
        
        public Tile PointToTile( Vector3 point )
        {
            // find out on which tile this point lies 
            var xIndex = (int) point.x / 10; // TODO: remove magic numbers !
            var zIndex = (int) point.z / 10;
            
            // Debug.Log("x: " + xIndex + " z: " + zIndex );
            
            return Grid[xIndex , zIndex];
        }

        public int Manhattan( Tile tile1 , Tile tile2 )
        {
            return Math.Abs(tile1.GridX - tile2.GridX) + Math.Abs(tile1.GridZ - tile2.GridZ);
        }

        public List<Tile> GetAllDirtyTiles()
        {
            //TODO: speed this up, we could keep track of all the dirty tiles
            var list = new List<Tile>();

            for (var i = 0; i < Grid.GetLength(0); ++i)
            for (var j = 0; j < Grid.GetLength(1); ++j)
            {
                if ( Grid[i,j].IsDirty )
                    list.Add(Grid[i,j]);
            }

            return list;
        }

        public List<Tile> GetReachableNeighbors( Tile tile )
        {
            // get the neighbors such that no walls exist in between
            var neighbors = new List<Tile>();

            for (var direction = 0; direction < 4; ++direction) // iterate over all directions
            {
                Tile temp;
                // if does not have a wall in this direction and indeed has a neighbor ( not on the border ) 
                if (!tile.hasWall((TILE_EDGE) direction) && (temp = GetNeighbor(tile, (TILE_EDGE) direction)) != null)
                {
                    neighbors.Add(temp);
                }
            }
            
            return neighbors;
        }

        public void SetTileDirtState(Tile tile , bool isDirty )
        {
            tile.IsDirty = isDirty;
        }

        public void SetTileWall(Tile tile, TILE_EDGE direction , bool state )
        {
            // each tile is only responsible for the upper and right walls
            // if we want a lower wall on the current tile we have to assign the upper on the tile below
            tile.setWall(direction , state);
            
            // walls are between two tiles, set the other tile that wasn't directly selected
            GetNeighbor(tile , direction).setWall(direction.getOpposite(), state );
        }

        public Tile GetTile( int gridX , int gridZ )
        {
            if (gridX >= 0 && gridX < sizeX && gridZ >= 0 && gridZ < sizeZ)
                return Grid[gridX, gridZ];
            return null;
        }

        public Tile GetLeft(Tile tile)
        {
            return ( tile.GridX > 0 ) ? Grid[tile.GridX-1,tile.GridZ] : null;
        }

        public Tile GetRight ( Tile tile )
        {
            return ( tile.GridX < sizeX-1 ) ? Grid[tile.GridX+1,tile.GridZ] : null;
        }

        public Tile GetUp(Tile tile)
        {
            return (tile.GridZ < sizeZ-1) ? Grid[tile.GridX,tile.GridZ+1] : null;
        }

        public Tile GetDown(Tile tile)
        {
            return (tile.GridZ > 0) ? Grid[tile.GridX,tile.GridZ-1] : null;
        }

        // gets the neighbor in the specified direction, if it does not exist, returns null
        public Tile GetNeighbor ( Tile tile , TILE_EDGE direction)
        {
            Tile neighbor = null;
            
            switch (direction)
            {
                case TILE_EDGE.UP:
                    neighbor = GetUp(tile);
                    break;
                case TILE_EDGE.RIGHT:
                    neighbor = GetRight(tile);
                    break;
                case TILE_EDGE.DOWN:
                    neighbor = GetDown(tile);
                    break;
                case TILE_EDGE.LEFT:
                    neighbor = GetLeft(tile);
                    break;
            }

            return neighbor;
        }

        public Vector3 GetClosestEdgeWorldPos( Vector3 point )
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

        public void Destroy() // destroy the current Map, cleans up and destroys all GameObjects related to it
        {
            // destroy all tiles in grid
            for (var i = 0; i < Grid.GetLength(0); ++i)
            {
                for (var j = 0; j < Grid.GetLength(1); ++j)
                {
                    Grid[i,j].Destroy();
                }
            }
        }
    }
}

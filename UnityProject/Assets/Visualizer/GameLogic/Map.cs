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

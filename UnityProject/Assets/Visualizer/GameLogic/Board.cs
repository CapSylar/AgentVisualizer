using System;
using System.Collections.Generic;
using UnityEngine.Rendering;
using Visualizer.Algorithms;

namespace Visualizer.GameLogic
{
    // Board represents the state of the Game Board, where the dirt is, where the walls are...
    [Serializable()]
    public class Board
    {   
        // the floor, the actual grid of tiles which also contain the position of the walls
        public Tile[,] Grid;
        
        public int sizeX , sizeZ; // not actual units, just the number of tiles in each direction
        
        public int NumOfTiles
        {
            get => sizeX * sizeZ;
        }

        [NonSerialized()] protected int _dirtyTiles;

        public virtual int DirtyTiles
        {
            get => _dirtyTiles;
            protected set {  _dirtyTiles = value; }
        }

        // createGrid was added to make it easier to write the constructor of both
        // Board and GraphicalBoard 
        public Board(int sizeX, int sizeZ, bool populateGrid = true)
        {
            Grid = new Tile[sizeX, sizeZ];

            if (populateGrid)
            {
                for (var i = 0; i < sizeX; ++i)
                    for (var j = 0; j < sizeZ; ++j)
                        Grid[i, j] = new Tile(i, j);
            }

            this.sizeX = sizeX;
            this.sizeZ = sizeZ;
        }

        public Board(Board board) : this( board.sizeX , board.sizeZ)
        {
            for (var i = 0; i < sizeX; ++i)
            {
                for (var j = 0; j < sizeZ; ++j)
                {
                    Grid[i, j] = board.Grid[i, j].GetClone();
                }
            }
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
        
        public List<Tile> GetAllTiles()
        {
            //TODO: speed this up, we could keep track of all tiles
            var list = new List<Tile>();

            for (var i = 0; i < Grid.GetLength(0); ++i)
            for (var j = 0; j < Grid.GetLength(1); ++j)
            {
                //if ( Grid[i,j].IsDirty )
                list.Add(Grid[j,i]);
            }

            return list;
        }
        
        public Tile GetLeft(Tile graphicalTile)
        {
            return ( graphicalTile.GridX > 0 ) ? Grid[graphicalTile.GridX-1,graphicalTile.GridZ] : null;
        }

        public Tile GetRight ( Tile graphicalTile )
        {
            return ( graphicalTile.GridX < sizeX-1 ) ? Grid[graphicalTile.GridX+1,graphicalTile.GridZ] : null;
        }

        public Tile GetUp(Tile graphicalTile)
        {
            return (graphicalTile.GridZ < sizeZ-1) ? Grid[graphicalTile.GridX,graphicalTile.GridZ+1] : null;
        }

        public Tile GetDown(Tile graphicalTile)
        {
            return (graphicalTile.GridZ > 0) ? Grid[graphicalTile.GridX,graphicalTile.GridZ-1] : null;
        }
        
        public Tile GetNeighbor ( Tile graphicalTile , TILE_EDGE direction)
        {
            Tile neighbor = null;
            
            switch (direction)
            {
                case TILE_EDGE.UP:
                    neighbor = GetUp(graphicalTile);
                    break;
                case TILE_EDGE.RIGHT:
                    neighbor = GetRight(graphicalTile);
                    break;
                case TILE_EDGE.DOWN:
                    neighbor = GetDown(graphicalTile);
                    break;
                case TILE_EDGE.LEFT:
                    neighbor = GetLeft(graphicalTile);
                    break;
            }

            return neighbor;
        }
        
        public List<Tile> GetReachableNeighbors( Tile graphicalTile )
        {
            // get the neighbors such that no walls exist in between
            var neighbors = new List<Tile>();

            for (var direction = 0; direction < 4; ++direction) // iterate over all directions
            {
                Tile temp;
                // if does not have a wall in this direction and indeed has a neighbor ( not on the border ) 
                if (!graphicalTile.HasWall((TILE_EDGE) direction) && (temp = GetNeighbor(graphicalTile, (TILE_EDGE) direction)) != null)
                {
                    neighbors.Add(temp);
                }
            }
            
            return neighbors;
        }
        
        public bool IsTileWallOnEdge( Tile graphicalTile , TILE_EDGE direction )
        {
            return (GetNeighbor(graphicalTile, direction) == null);
        }
        
        public virtual void SetTileWall(Tile tile , TILE_EDGE direction , bool state )
        {
            // each tile is only responsible for the upper and right walls
            // if we want a lower wall on the current tile we have to assign the upper on the tile below
            tile.SetWall(direction , state);
            
            // walls are between two tiles, set the other tile that wasn't directly selected
            GetNeighbor(tile , direction).SetWall(direction.GetOpposite(), state );
        }
        
        // destroy all walls
        public virtual void RemoveAllWalls()
        {
            // calling remove all walls on Tiles is ok as long as we do it on all of them at once,
            // doing it on individual tiles creates inconsistencies in the Map
            
            DoOnAllGridEntries((i, j) => Grid[i,j].RemoveAllWalls());
        }
        
        protected void DoOnAllGridEntries(Func<int, int, Tile> lambda )
        {
            for (var i = 0; i < Grid.GetLength(0); ++i)
            for (var j = 0; j < Grid.GetLength(1); ++j)
            {
                // create Tile with loaded state
                Grid[i, j] = lambda(i, j);
            }
        }
        
        public int ManhattanDistance( Tile tile1 , Tile tile2 )
        {
            return Math.Abs(tile1.GridX - tile2.GridX) + Math.Abs(tile1.GridZ - tile2.GridZ);
        }

        public int BfsDistance( Tile tile1 , Tile tile2 ) // real path distance taking walls into account
        {
            //TODO: make a more efficient version of DoBfs just for this
            Bfs.DoBfs( this , tile1 , tile2 , out var path );
            return path.Count-1; // since current Tile is in the list
        }
        
        public Tile GetTile( int gridX , int gridZ )
        {
            if (gridX >= 0 && gridX < sizeX && gridZ >= 0 && gridZ < sizeZ)
                return Grid[gridX, gridZ];
            return null;
        }

        public Tile SetTileDirt(Tile tile, bool isDirty)
        {
            if (tile.IsDirty != isDirty) // if state really changed
            {
                tile.IsDirty = isDirty;
                DirtyTiles += isDirty ? 1 : -1;
            }
            
            return tile;
        }
        
        // set all tiles to clean
        public void MopTheFloor()
        {
            DoOnAllGridEntries((i, j) => SetTileDirt(Grid[i, j] , false));
        }
        
    }
}
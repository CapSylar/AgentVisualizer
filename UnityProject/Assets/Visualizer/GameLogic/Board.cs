using System;
using System.Collections.Generic;
using System.Linq;
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

        public int CleanTiles => NumOfTiles - DirtyTiles;

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
        
        // just gets the neighbors
        public List<Tile> GetReachableNeighbors( Tile tile )
        {
            // get the neighbors such that no walls exist in between
            var neighbors = new List<Tile>();

            for (var direction = 0; direction < 4; ++direction) // iterate over all directions
            {
                Tile temp;
                // if does not have a wall in this direction and indeed has a neighbor ( not on the border ) 
                if (!tile.HasWall((TILE_EDGE) direction) && (temp = GetNeighbor(tile, (TILE_EDGE) direction)) != null)
                {
                    neighbors.Add(temp);
                }
            }
            
            return neighbors;
        }

        //TODO: does not really belong here, and needs refactoring
        // gets the neighboring tiles that are reachable and that are free, meaning no other agent is there 
        public List<Tile> GetReachableFreeNeighbors(Game game, Tile tile)
        {
            // get the neighbors such that no walls exist in between
            var neighbors = new List<Tile>();

            for (var direction = 0; direction < 4; ++direction) // iterate over all directions
            {
                Tile temp;
                // if does not have a wall in this direction and indeed has a neighbor ( not on the border ) 
                if (!tile.HasWall((TILE_EDGE) direction) &&
                    (temp = GetNeighbor(tile, (TILE_EDGE) direction)) != null &&
                     !IsTileOccupied(game , temp))
                {
                    neighbors.Add(temp);
                }
            }
            
            return neighbors;
        }

        //TODO: does not really belong here
        // is the tile occupied by an agent ?
        public bool IsTileOccupied( Game game , Tile tile )
        {
            //TODO: too slow, runs in O(N)

            foreach (var player in game.Players)
            {
                if (player.CurrentTile.GridX == tile.GridX && player.CurrentTile.GridZ == tile.GridZ)
                {
                    return true; 
                }
            }
            return false;
            
            // return game.Players.Any(player => player.CurrentTile == tile);
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
            Bfs.DoNormalBfs( this , tile1 , tile2 , out var path );
            return path.Count-1; // since current Tile is in the list
        }
        
        public Tile GetTile( int gridX , int gridZ )
        {
            if (gridX >= 0 && gridX < sizeX && gridZ >= 0 && gridZ < sizeZ)
                return Grid[gridX, gridZ];
            return null;
        }

        public int DirtyTilesInSquare(int side, Tile tile )
        {
            return DirtyTilesInSquare(side, tile.GridX, tile.GridZ);
        }

        public int DirtyTilesInSquare(int side , int gridX , int gridZ )
        {
            // side represents the side length of the square to search in

            var count = 0;

            var minX = gridX - side > 0 ? gridX - side : 0;
            var maxX = gridX + side > sizeX - 1 ? sizeX : gridX + side ;
            var minZ = gridZ - side > 0 ? gridZ - side : 0;
            var maxZ = gridZ + side >sizeZ - 1 ? sizeZ : gridZ + side ;
            
            for ( var x = minX ; x < maxX ; ++x )
                for ( var z = minZ ; z < maxZ ; ++z )
                    if (Grid[x, z].IsDirty)
                        ++count;
            
            return count;
        }

        public int CleanTilesInSquare(int side, Tile tile)
        {
            return CleanTilesInSquare(side, tile.GridX, tile.GridZ);
        }

        public int CleanTilesInSquare (int side, int gridX, int gridZ)
        {
            var tilesInSquare = (2 * side + 1) * (2 * side + 1);
            return tilesInSquare - DirtyTilesInSquare(side, gridX, gridZ);
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
        
        public override string ToString()
        {
            return $"Board:{{ NumTiles: {NumOfTiles}, DirtyTilesCount: {DirtyTiles} }}";
        }
    }
}
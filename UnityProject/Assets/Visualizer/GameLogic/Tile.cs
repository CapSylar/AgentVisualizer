using System;
using UnityEngine;

namespace Visualizer.GameLogic
{
    // the base implementation of the Tile

    [Serializable()]
    public class Tile
    {
        protected bool isDirty;
        public bool[] hasWallOnEdge;  // always 4 entries : UP, DOWN , RIGHT , lEFT

        public int GridX { get; protected set; }
        public int GridZ { get; protected set; }

        public virtual bool IsDirty
        {
            get => isDirty;
            set => isDirty = value;
        }

        public Tile( int gridX , int gridZ  )
        {
            GridX = gridX;
            GridZ = gridZ;
            
            isDirty = false;
            hasWallOnEdge = new bool [4];
        }
        
        public Tile( Tile tile )
        {
            SetState(tile);
        }
        
        public TILE_EDGE OrientationOf( Tile otherTile )
        {
            // get the relative position of otherTile with respect to the current Tile 
            
            if (otherTile.GridX == this.GridX) // must be UP or down
            {
                return (GridZ - otherTile.GridZ) > 0 ? TILE_EDGE.DOWN : TILE_EDGE.UP;
            }
             
            // must be LEFT or RIGHT
            return (GridX - otherTile.GridX) > 0 ? TILE_EDGE.LEFT : TILE_EDGE.RIGHT;
        }
        
        
        public virtual Tile SetState(Tile tile)
        {
            GridX = tile.GridX;
            GridZ = tile.GridZ;
            
            isDirty = tile.isDirty;
            hasWallOnEdge = ( bool [] ) tile.hasWallOnEdge.Clone();

            return this; // in case calling function needs it
        }
        
        public bool HasWall( TILE_EDGE edge )
        {
            return hasWallOnEdge[(int) edge]; 
        }
        
        public virtual void SetWall ( TILE_EDGE edge , bool present )
        {
            // place a wall on that edge
            hasWallOnEdge[(int)edge] = present;
        }
        
        public Tile RemoveAllWalls() // Warning: should not be called directly, can create inconsistencies in the map
        {
            hasWallOnEdge = new bool[4]; // all to zero

            return this;
        }

        public virtual Vector3 GetWorldPosition()
        {
            return Vector3.zero; // TODO:Bit of a hack, implemented just for GraphicalTile to override it
        }
        
        public Tile GetClone()
        {
            return ( Tile ) MemberwiseClone();
        }

        public bool IsEqual(Tile other)
        {
            return GridX == other.GridX && GridZ == other.GridZ;
        }
    }
}
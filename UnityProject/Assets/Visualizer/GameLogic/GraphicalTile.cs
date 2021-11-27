using System;
using UnityEngine;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    // Graphical Wrapper around the base Tile, contains Graphics specific code
    public class GraphicalTile : Tile
    {
        private GameObject _tile;
        private GameObject _wallPrefab = PrefabContainer.Instance.wallPrefab;
        private GameObject _upperWall;
        private GameObject _rightWall;

        private const int PlaneSize = 10;

        //TODO: maybe there exists a cleaner way to do it
        // private TileState _data;
        
        // state that is not saved with Tile
        private bool _isMarked = false; // show tile in mark color is true

        public GraphicalTile( Transform parent , int gridX , int gridZ  ) : base( gridX , gridZ )
        {
            InitGraphics( parent , GridX , GridZ );
            
            //TODO: broke saving, fix it !!!!
            
            // state ??= new TileState();
            // _data = state; // assign state
        }

        public GraphicalTile(Transform parent, Tile tile) : base(tile)
        {
            InitGraphics( parent , GridX , GridZ );
        }

        private void InitGraphics( Transform parent , int x , int z )
        {
            _tile = GameObject.Instantiate(PrefabContainer.Instance.tilePrefab , parent );
            _tile.transform.SetParent( parent , false );
            _tile.transform.localPosition = new Vector3(x * PlaneSize, 0, z * PlaneSize);
        }
        
        // public static GraphicalTile CreateTile(Transform parent, int x, int y, TileState state = null)
        // {
        //     state ??= new TileState();
        //     
        //     component.Init(x,y , state);
        //     return component;
        // }
        //
        // private void Init(int x, int z, TileState state)
        // {
        //     gameObject.transform.localPosition = new Vector3(x*PlaneSize, 0, z*PlaneSize);
        //
        // }
        
        // Graphical is costly to build, added the options
        // to change it in place 

        public new GraphicalTile SetState( Tile tile )
        {
            // change the "underneath" tile
            base.SetState(tile);
            return this;
        }

        public override  Vector3 GetWorldPosition()
        {
            return _tile.transform.position;
        }
        
        public override bool IsDirty
        {
            get => isDirty;
            set // should not be set manually
            {
                isDirty = value;
                Refresh(); // refresh tile
            }
        }

        public override  void SetWall ( TILE_EDGE edge , bool present )
        {
            // place a wall on that edge
            base.SetWall( edge , present );
            Refresh();
        }
        
        public GraphicalTile RemoveAllWalls() // Warning: should not be called directly, can create inconsistencies in the map
        {
            base.RemoveAllWalls();
            Refresh();

            return this;
        }

        // public TileState GetTileState()
        // {
        //     return _data;
        // }

        // public TileState GetTileStateCopy()
        // {
        //     return _data.getClone();
        // }

        public Vector3 GetTileWorldPos()
        {
            return _tile.transform.position;
        }

        public TILE_EDGE GetClosestEdge(Vector3 pointOnTile)
        {
            // both pointOnTile and the return are in global coordinates
            
            // transform point to origin
            Vector3 toOrigin = pointOnTile - _tile.transform.position;

            // get min of the 4 perpendiculars to the edges
            // TODO: very ugly, find better way to write this section
            
            var distanceUp = Math.Abs(5 - toOrigin.z);
            var distanceDown = Math.Abs(-5 - toOrigin.z);

            var distanceRight = Math.Abs(5 - toOrigin.x);
            var distanceLeft = Math.Abs(-5 - toOrigin.x);

            var min = Math.Min(distanceUp, Math.Min(distanceDown, Math.Min(distanceRight, distanceLeft)));
            
            // return closest edge
            if (min == distanceUp)
            {
                return TILE_EDGE.UP;
            }
            if ( min == distanceDown )
            {
                return TILE_EDGE.DOWN;
            }
            if (min == distanceRight)
            {
                return TILE_EDGE.RIGHT;
            }

            return TILE_EDGE.LEFT;
        }

        public Vector3 GetClosestEdgeWorldPos(Vector3 pointOnTile)
        {
            // first find out where the closest edge is
            var closest = GetClosestEdge(pointOnTile);

            return _tile.transform.position + new Vector3(
                closest == TILE_EDGE.RIGHT ? 5 : (closest == TILE_EDGE.LEFT) ? -5 : 0,
                0,
                closest == TILE_EDGE.UP ? 5 : (closest == TILE_EDGE.DOWN) ? -5 : 0);
        }


    
        //TODO: remove magic numbers
        public void Refresh()
        {
            // Refresh the Tile graphics ( Dirt + Tiles )
            // Tile is only responsible for checking its right and upper walls
            
            // create the up wall
            if (HasWall(TILE_EDGE.UP) && _upperWall == null )
            {
                _upperWall = GameObject.Instantiate(_wallPrefab , _tile.transform);
                _upperWall.transform.localPosition = new Vector3(0, 0, 5);
            }
            
            // remove the up wall
            if (!HasWall(TILE_EDGE.UP) && _upperWall != null )
            {
                GameObject.Destroy(_upperWall);
            }
            
            // create the right wall
            if (HasWall(TILE_EDGE.RIGHT) && _rightWall == null)
            {
                _rightWall = GameObject.Instantiate(_wallPrefab, _tile.transform);
                _rightWall.transform.localPosition = new Vector3(5, 0, 0);
                _rightWall.transform.rotation = Quaternion.Euler(0,90,0);
            }
            
            // remove the right wall
            if (!HasWall(TILE_EDGE.RIGHT) && _rightWall != null)
            {
                GameObject.Destroy(_rightWall);
            }
            
            // // is it has any dirt assigned
            var rend = _tile.GetComponent<Renderer>();
            var material = rend.material;
            // control the second detail albedo map to show or hide the dirt
            //TODO: maybe this is not so efficient ? 
            material.EnableKeyword("_DETAIL_MULX2");
            material.SetTexture("_DetailAlbedoMap" , IsDirty ? PrefabContainer.Instance.dirtTexture : null );
            
            // control the second detail albedo map to show or hide the dirt
            material.color = _isMarked ? Color.yellow : Color.white ;
        }
        
        // cleanup, destroy gameObjects...
        public void Destroy()
        {
            // like in the case of creation, the tile is also responsible for destroying the up and right walls
            
            if ( _upperWall != null ) // destroy the up wall
                GameObject.Destroy(_upperWall);
            if ( _rightWall != null ) // destroy the right wall
                GameObject.Destroy(_upperWall);
            
           GameObject.Destroy(_tile); // byebye!
        }

        // eases Algorithm debugging
        public GraphicalTile SetMark( bool isOn ) // not part of Tile state
        {
            _isMarked = isOn;
            Refresh(); // refresh graphics 

            return this;
        }
       
    }
}
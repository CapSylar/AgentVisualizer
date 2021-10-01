using System;
using System.Resources;
using UnityEngine;

namespace Visualizer
{

    public enum TILE_EDGE // assuming z is looking up and x to the right and we are looking down in 2D
    {
        UP = 0,
        DOWN,
        RIGHT,
        LEFT
    }
    
    public class Tile : MonoBehaviour
    {
        private GameObject _tilePrefab;
        private const int PlaneSize = 10;
        
        private bool _isDirty;
        private bool _hasWallDown, _hasWallUp , _hasWallLeft, _hasWallRight ;

        public void Init(int x, int z, bool isDirty = false)
        {
            gameObject.transform.localPosition = new Vector3(x*PlaneSize, 0, z*PlaneSize);
            IsDirty = isDirty;
        }

        public void Update()
        {
            // do something here
        }
        
        public static Tile CreateTile(GameObject prefab , Transform parent ,  int x , int y , bool isDirty = false )
        {
            var plane = Instantiate(prefab , parent );
            var component = plane.AddComponent<Tile>();
            
            component.Init(x,y,isDirty);
            return component;
        }
        
        public bool IsDirty
        {
            get
            {
                return _isDirty;
            }
            set
            {
                _isDirty = value;
            }
        }

        
        public TILE_EDGE GetClosestEdge(Vector3 pointOnTile)
        {
            // both pointOnTile and the return are in global coordinates
            
            // transform point to origin
            Vector3 toOrigin = pointOnTile - gameObject.transform.position;

            // get min of the 4 perpendiculars to the edges
            // TODO: very ugly, find better way to write this section
            
            float distanceUp = Math.Abs(5 - toOrigin.z);
            float distanceDown = Math.Abs(-5 - toOrigin.z);

            float distanceRight = Math.Abs(5 - toOrigin.x);
            float distanceLeft = Math.Abs(-5 - toOrigin.x);

            float min = Math.Min(distanceUp, Math.Min(distanceDown, Math.Min(distanceRight, distanceLeft)));
            
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

            return gameObject.transform.position + new Vector3(
                closest == TILE_EDGE.RIGHT ? 5 : (closest == TILE_EDGE.LEFT) ? -5 : 0,
                0,
                closest == TILE_EDGE.UP ? 5 : (closest == TILE_EDGE.DOWN) ? -5 : 0);
        }
        
        
    }
}



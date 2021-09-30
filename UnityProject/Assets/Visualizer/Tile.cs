using UnityEngine;

namespace Visualizer
{
    public class Tile : MonoBehaviour
    {
        private GameObject _tilePrefab;
        private const int PlaneSize = 10;
        
        private bool _isDirty;
        private bool _hasWallDown, _hasWallUp , _hasWallLeft, _hasWallRight ;

        public void Init(int x, int z, bool isDirty = false)
        {
            gameObject.transform.position = new Vector3(x*PlaneSize, 0, z*PlaneSize);
            IsDirty = isDirty;
            Debug.Log("init says hello!");
        }

        public void Update()
        {
            
        }
        
        public static Tile CreateTile(GameObject prefab , int x , int y , bool isDirty = false )
        {
            var plane = Instantiate(prefab);
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
        
    }
}



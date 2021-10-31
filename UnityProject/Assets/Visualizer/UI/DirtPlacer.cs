using UnityEditor;
using UnityEngine;
using Visualizer.GameLogic;

namespace Visualizer.UI
{
    public class DirtPlacer : ItemPlacer
    {
        private GameObject _preview;
        
        // placer state
        private Transform _previewTransform;
        private Tile _currentTile , _lastTile ;

        public DirtPlacer()
        {
            _preview = GameObject.Instantiate(PrefabContainer.Instance.dirtyPlanePrefab);
            _previewTransform = _preview.transform;
        }

        public void Destroy()
        {
            GameObject.Destroy(_preview);
        }

        public void Update(Vector3 worldPos)
        {
            _currentTile = GameStateManager.Instance.currentMap.PointToTile(worldPos);
            
            //TODO: this could be a performance hazard, keep in mind!!
            
            // we essentially display the preview dirty tile on top of the tile already existing, hiding the latter
            // I guess its better than changing the material every time
            var trans = _currentTile.gameObject.transform.position;
            _previewTransform.position = new Vector3(trans.x, 0.01f, trans.z); // 0.01f to prevent Z fighting
        }

        public void PlaceItem()
        {
            _currentTile.IsDirty = true;
        }

        public void RemoveItem()
        {
            _currentTile.IsDirty = false;
        }
    }
}
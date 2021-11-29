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
        private GraphicalTile _currentGraphicalTile , _lastGraphicalTile ;

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
            _currentGraphicalTile = GameStateManager.Instance.CurrentBoard.PointToTile(worldPos);
            
            //TODO: this could be a performance hazard, keep in mind!!
            
            // we essentially display the preview dirty tile on top of the tile already existing, hiding the latter
            // I guess its better than changing the material every time
            var trans = _currentGraphicalTile.GetWorldPosition();
            _previewTransform.position = new Vector3(trans.x, 0.01f, trans.z); // 0.01f to prevent Z fighting
        }

        public void PlaceItem()
        {
            GameStateManager.Instance.CurrentBoard.SetTileDirt( _currentGraphicalTile , true );
        }

        public void RemoveItem()
        {
            GameStateManager.Instance.CurrentBoard.SetTileDirt( _currentGraphicalTile , false );

        }
    }
}
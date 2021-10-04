using UnityEngine;

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
            _preview = GameObject.Instantiate(GameState.Instance._dirtyPlanePrefab);
            _previewTransform = _preview.transform;
        }

        public void Update(Vector3 worldPos)
        {
            _currentTile = GameState.Instance.currentMap.PointToTile(worldPos);
            
            //TODO: this could be a performance hazard, keep in mind!!
            
            // we essentially display the preview dirty tile on top of the tile already existing, hiding the latter
            // I guess its better than changing the material every time
            var trans = _currentTile.gameObject.transform.position;
            _previewTransform.position = new Vector3(trans.x, 0.01f, trans.z); // 0.01f to prevent Z fighting
        }

        public void PlaceItem()
        {
            GameState.Instance.currentMap.SetTileDirtState(_currentTile , true);
        }

        public void RemoveItem()
        {
            GameState.Instance.currentMap.SetTileDirtState(_currentTile , false);

        }
    }
}
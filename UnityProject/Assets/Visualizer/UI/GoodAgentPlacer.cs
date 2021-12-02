using System.Net.Sockets;
using UnityEngine;
using Visualizer.GameLogic;

namespace Visualizer.UI
{
    public class GoodAgentPlacer : ItemPlacer
    {
        private GameObject _preview;
        
        // placer state
        private Transform _previewTransform;
        private GraphicalTile _currentTile;

        public GoodAgentPlacer()
        {
            _preview = GameObject.Instantiate(PrefabContainer.Instance.agentPrefab);
            _previewTransform = _preview.transform;
        }
        
        public void Destroy()
        {
            GameObject.Destroy(_preview);
        }
        
        public void Update(Vector3 worldPos)
        {
            // worldPos of mouse pointer of map
            _currentTile = GameStateManager.Instance.CurrentBoard.PointToTile(worldPos);
            var trans = _currentTile.GetWorldPosition();
            _previewTransform.position = new Vector3(trans.x, 0.01f, trans.z); // 0.01f to prevent Z fighting
        }

        public void PlaceItem()
        {
            GameStateManager.Instance.SetCurrentAgent(_currentTile.GridX , _currentTile.GridZ );
        }

        public void RemoveItem()
        {
            GameStateManager.Instance.RemoveAgent( _currentTile.GridX , _currentTile.GridZ , true );
        }
    }
}
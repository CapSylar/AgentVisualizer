using System.Net.Sockets;
using UnityEngine;
using Visualizer.GameLogic;

namespace Visualizer.UI
{
    public class AgentEnemyPlacer : ItemPlacer
    {
        private GameObject _preview;
        
        // placer state
        private Transform _previewTransform;
        private GraphicalTile _currentGraphicalTile;

        public AgentEnemyPlacer()
        {
            _preview = GameObject.Instantiate(PrefabContainer.Instance.agentEnemyPrefab);
            _previewTransform = _preview.transform;
        }
        
        public void Destroy()
        {
            GameObject.Destroy(_preview);
        }
        
        public void Update(Vector3 worldPos)
        {
            // worldPos of mouse pointer of map
            _currentGraphicalTile = GameStateManager.Instance.CurrentBoard.PointToTile(worldPos);
            var trans = _currentGraphicalTile.GetWorldPosition();
            _previewTransform.position = new Vector3(trans.x, 0.01f, trans.z); // 0.01f to prevent Z fighting
        }

        public void PlaceItem()
        {
            //TODO: change this
            // GameStateManager.Instance.SetCurrentAgent(_currentTile.GridX , _currentTile.GridZ );
        }

        public void RemoveItem()
        {
            // GameStateManager.Instance.RemoveCurrentAgent();
        }
    }
}
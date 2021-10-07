using System.Net.Sockets;
using UnityEngine;

namespace Visualizer.UI
{
    public class AgentPlacer : ItemPlacer
    {
        private GameObject _preview;
        
        // placer state
        private Transform _previewTransform;
        private Tile _currentTile;

        public AgentPlacer()
        {
            _preview = GameObject.Instantiate(Epoch.Instance._agentPrefab);
            _previewTransform = _preview.transform;
        }
        
        public void Update(Vector3 worldPos)
        {
            // worldPos of mouse pointer of map
            _currentTile = Epoch.Instance.currentMap.PointToTile(worldPos);
            var trans = _currentTile.gameObject.transform.position;
            _previewTransform.position = new Vector3(trans.x, 0.01f, trans.z); // 0.01f to prevent Z fighting
        }

        public void PlaceItem()
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem()
        {
            throw new System.NotImplementedException();
        }
    }
}
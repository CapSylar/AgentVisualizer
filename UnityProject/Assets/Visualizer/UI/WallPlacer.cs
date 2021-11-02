using UnityEngine;
using Visualizer.GameLogic;
using Visualizer.UI;

namespace Visualizer
{
    public class WallPlacer : ItemPlacer
    {
        private GameObject _preview;

        // placer state
        private bool _isLastValid; // indicated position is valid for placing an item or removing one
        private Tile _currentTile;
        private Vector3 _placementPos;
        private TILE_EDGE _placementDirection;

        public WallPlacer()
        {
            _preview = GameObject.Instantiate(PrefabContainer.Instance.wallPrefabPreview);
        }
        
        public void Destroy()
        {
            GameObject.Destroy(_preview);
        }

        public void Update( Vector3 worldPos ) // worldPos of mouse pointer on Map
        {
            // first check if it is placeable there
            if (IsPlaceable(worldPos))
            {
                _isLastValid = true;
                _preview.gameObject.SetActive(true);
                PositionPreview(); // position the gameObject in the game correctly
            }
            else
            {
                _isLastValid = false;
                _preview.gameObject.SetActive(false);
            }
        }

        private void PositionPreview()
        {
            // position the preview object correctly if needed at _placementPos
            var trans = _preview.transform;
            trans.position = _placementPos;
            // rotate wall in the correct direction
            if (_placementDirection == TILE_EDGE.UP || _placementDirection == TILE_EDGE.DOWN)
                trans.rotation = Quaternion.Euler(0, 0, 0);
            else
                trans.rotation = Quaternion.Euler(0,90,0);
        }

        private bool IsPlaceable( Vector3 worldPoint )
        {
            // returns true if the object can be placed in this position
            // find out which tile, 
            
            //TODO: maybe we should abstract this away, just talk to map not to tiles directly?
            var tile = GameStateManager.Instance.currentMap.PointToTile(worldPoint);
            var edgePos = tile.GetClosestEdgeWorldPos(worldPoint);

            if (GameStateManager.Instance.currentMap.isEdgeOnMapBorder(edgePos) ||
                    Vector3.Distance(worldPoint, edgePos) > 3 ) // invalid, can't place walls on borders, or mouse pointer too far from closest edge
                return false;
            
            // save them
            _currentTile = tile;
            _placementPos = edgePos;
            _placementDirection = _currentTile.GetClosestEdge(_placementPos); // get the direction

            return true;
        }

        public void PlaceItem()
        {
            if ( _isLastValid )
                GameStateManager.Instance.currentMap.SetTileWall( _currentTile , _placementDirection , true );
        }

        public void RemoveItem()
        {
            if (_isLastValid )
                GameStateManager.Instance.currentMap.SetTileWall( _currentTile, _placementDirection , false );
                
        }
    } 
}


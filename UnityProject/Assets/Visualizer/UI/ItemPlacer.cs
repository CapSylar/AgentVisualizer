using UnityEngine;

namespace Visualizer.UI
{
    public interface ItemPlacer
    {
        // called by the MapEditor at every frame with the mouse position projected onto the map
        // the Placer is responsible for handling the effects and graphics on screen
        public void Update(Vector3 worldPos); // worldPos of mouse pointer on Map
        
        // when the user presses the left key to place the item , the MapEditor calls this method
        // it is up to the placer to determine whether the location is valid
        // it is also up to the placer to inform the map of the changes to be made
        public void PlaceItem();
        
        // when the user presses the right left key to remove the item, it is up to the placer to first check
        // if the item is even placed at that location , and if yes remove it by informing the map
        public void RemoveItem();
    }
}
using UnityEngine;

namespace Visualizer.UI
{
    public class ButtonListener : MonoBehaviour
    {
        public void OnLoadMapButtonPressed()
        {
            Debug.Log("load map button pressed!");
        }
        
        public void OnSaveMapButtonPressed()
        {
            Debug.Log("Save map button pressed!");
        }
        
        public void OnEditMapButtonPressed()
        {
            Debug.Log("Edit map button pressed!");
        }
    }
}

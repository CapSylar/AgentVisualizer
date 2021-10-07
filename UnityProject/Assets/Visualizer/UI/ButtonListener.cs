using UnityEngine;
using Visualizer.GameLogic;

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
            GameState state = new GameState(Epoch.Instance.currentMap);
            state.Save("Assets/Visualizer/Maps/map000.map");
        }
        
        public void OnEditMapButtonPressed()
        {
            Debug.Log("Edit map button pressed!");
        }
    }
}

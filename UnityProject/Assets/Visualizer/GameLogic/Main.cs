using System.Data.Common;
using TMPro;
using UnityEngine;
using Visualizer.AgentBrains;

namespace Visualizer.GameLogic
{
    public class Main : MonoBehaviour
    {
        // Starts the game, and handles main UI ( not editor UI )
    
        public GameObject mainUI;
        public GameObject mapEditorUI;
        public GameObject dropDownMenu;
        public MapEditor mapEditorComponent;
    
        private GameStateManager Manager;
    
        void Start()
        {
            // create a GameStateManager to keep track of practically everything
            Manager = new GameStateManager();
        
            // populate the drop down menu with available brains
            var dropDown = dropDownMenu.GetComponent<TMP_Dropdown>();
            dropDown.options.Clear(); // just to be sure
            foreach (var brainName in BrainCatalog.GetAllBrainNames())
            {
                dropDown.options.Add(new TMP_Dropdown.OptionData(brainName));
            }
            
            dropDown.RefreshShownValue();
            // hook listener
            dropDown.onValueChanged.AddListener(delegate { DropDownItemSelected(dropDown); });
        }
        

        void Update()
        {
        
        }

        public bool isPlaying = false;
        public void OnPlayPressed()
        {
            // user wants to start the Agent
            // send the request to the Game state instance, it will manage it from there
            if (isPlaying) // button serves as pause button
            {
                GameStateManager.Instance.PauseGame();
                isPlaying = true;
            }
            else
            {
                GameStateManager.Instance.StartGame();
                isPlaying = false;
            }
        }

        public void OnResetPressed()
        {
            // user wants to reset everything
            // send the request to the Game state instance, it will manage it from there
            GameStateManager.Instance.ResetGame();
        }
    
        public void OnChangeMapPressed()
        {
            // user wants to go to the Map Editor UI
            mapEditorUI.SetActive(true);
            mainUI.SetActive(false);
        }

        public void OnGoMainUI()
        {
            // user wants to quit the Map Editor UI
            mapEditorComponent.OnExitEditor(); // tell the object to clean up
            mapEditorUI.SetActive(false);
            mainUI.SetActive(true);
        }
        
        // user wants to select a brain to use
        void DropDownItemSelected(TMP_Dropdown dropDown)
        {
            Manager.setCurrentBrain(BrainCatalog.NameToBrain(
                dropDown.options[dropDown.value].text));
            
            dropDown.RefreshShownValue();
        }
        
    }
}

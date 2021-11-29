using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visualizer.UI;

namespace Visualizer.GameLogic
{
    public class Main : MonoBehaviour
    {
        private enum MAIN_STATE // assuming z is looking up and x to the right and we are looking down in 2D
        {
            NOT_RUNNING = 0, // was never running 
            RUNNING, // is running right now
            PAUSED, // is pause, but can be resumed
        }
        
        // Starts the game, and handles main UI ( not editor UI )
    
        public GameObject mainUI;
        public GameObject mapEditorUI;
        public MapEditor mapEditorComponent;
        public GameObject telemetryDock;

        // References to UI elements
        public TMP_Dropdown dropDownMenu;
        public Button changeMapButton;
        public Button resetButton;
        
        // References to UI elements in TelemetryDock
        
        public Text stepsLabel, turnsLabel , dirtLeftLabel;
        
        // Reference to UI element PopUpWindow 
        public GameObject PopUpWindow;
        public GameObject UserInputSection;
        public Button DoneButton;
            
        private GameStateManager Manager;
        private GlobalTelemetryHandler _currentHandler;

        private MAIN_STATE _currentState;
        
        void Start()
        {
            // create a GameStateManager to keep track of practically everything
            Manager = new GameStateManager();
            
            // create a Global telemetry Handler 
            _currentHandler = new GlobalTelemetryHandler( this );
            
            // assign the popUpWindow reference and section to the PopUpHandler
            PopUpHandler.PopUpWindow = PopUpWindow;
            PopUpHandler.UserInputSection = UserInputSection;
            PopUpHandler.DoneButton = DoneButton;

            // populate the drop down menu with available brains
            dropDownMenu.options.Clear(); // just to be sure
            foreach (var brainName in BrainCatalog.GetAllBrainNames())
            {
                dropDownMenu.options.Add(new TMP_Dropdown.OptionData(brainName));
            }
            
            dropDownMenu.RefreshShownValue(); 
            DropDownItemSelected(dropDownMenu); //important, to set brain in state
            // hook listener
            dropDownMenu.onValueChanged.AddListener(delegate { DropDownItemSelected(dropDownMenu); });
            
            // set the proper state
            OnResetPressed();
        }
        

        void Update()
        {
            // update the current game state
            Manager.Update();
        }

        public void OnPlayPressed()
        {
            // user wants to start the Agent
            // send the request to the Game state instance, it will manage it from there
            if (_currentState == MAIN_STATE.RUNNING) // button serves as pause button
            {
                GameStateManager.Instance.PauseGame();
                _currentState = MAIN_STATE.PAUSED;
                resetButton.interactable = true;
            }
            else // button serves as run
            {
                GameStateManager.Instance.StartGame();
                _currentState = MAIN_STATE.RUNNING;
                resetButton.interactable = false; // shouldn't be able to press reset while running 
            }
            
            // set UI to non interactable
            dropDownMenu.interactable = false;
            changeMapButton.interactable = false;
        }

        public void OnResetPressed()
        {
            // user wants to reset everything
            // send the request to the Game state instance, it will manage it from there
            GameStateManager.Instance.ResetGame();
            _currentState = MAIN_STATE.NOT_RUNNING;
            
            // set UI to interactable
            dropDownMenu.interactable = true;
            changeMapButton.interactable = true;
            resetButton.interactable = true;
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
            Manager.SetCurrentBrain(BrainCatalog.NameToBrain(
                dropDown.options[dropDown.value].text));
            
            dropDown.RefreshShownValue();
        }
        
        // User wants to change the agent speed

        public void OnSpeedSliderValueChanged( float value )
        {
            GameStateManager.Instance.SetSpeed( (int)(value * 9 + 1)  );
        }
        
    }
}

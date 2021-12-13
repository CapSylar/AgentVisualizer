using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Visualizer.UI;
using Visualizer.UI.Catalogs;

namespace Visualizer.GameLogic
{
    public class Main : MonoBehaviour
    {
        // Starts the game, and handles main UI ( not editor UI )
    
        public GameObject mainUI;
        public GameObject mapEditorUI;
        public GameObject scoreBoardUI;
        public MapEditor mapEditorComponent;
        public GameObject telemetryDock;

        // References to UI elements
        public TMP_Dropdown evilAgentAlgoDropDownMenu;
        public TMP_Dropdown goodAgentAlgoDropDownMenu;
        public TMP_Dropdown stoppingConditionDropDownMenu;
        public TMP_Dropdown boardEvaluatorDropDownMenu;
        public Button changeMapButton;
        public Button resetButton;
        public Button stopButton;
        public Button playPauseButton;
        
        // References to UI elements in TelemetryDock
        
        public Text stepsLabel, turnsLabel , dirtLeftLabel;
        
        // Reference to UI element PopUpWindow 
        public GameObject PopUpWindow;
        public GameObject UserInputSection;
        public Button DoneButton;
            
        private GameStateManager _stateManager;
        private GlobalTelemetryHandler _currentHandler;
        
        void Start()
        {
            // create a GameStateManager to keep track of practically everything ( not really everything )
            _stateManager = new GameStateManager();
            
            // create a Global telemetry Handler 
            _currentHandler = new GlobalTelemetryHandler( this );
            
            // assign the popUpWindow reference and section to the PopUpHandler
            PopUpHandler.PopUpWindow = PopUpWindow;
            PopUpHandler.UserInputSection = UserInputSection;
            PopUpHandler.DoneButton = DoneButton;

            PopulateEvaluatorChooserDropDown();
            PopulateAlgorithmChooserDropdowns();
            PopulateStoppingConditionDropDown();

            // set the proper state
            OnResetPressed();
        }

        private void PopulateEvaluatorChooserDropDown()
        {
            boardEvaluatorDropDownMenu.options.Clear();
            
            // populate it with the board evaluators available
            foreach (var evaluatorName in BoardEvaluatorCatalog.GetAllBoardEvaluators())
            {
                boardEvaluatorDropDownMenu.options.Add(new TMP_Dropdown.OptionData(evaluatorName));
            }
            
            boardEvaluatorDropDownMenu.RefreshShownValue();
            BoardEvaluatorItemSelected();
            
            boardEvaluatorDropDownMenu.onValueChanged.AddListener(delegate { BoardEvaluatorItemSelected(); });

        }


        private void PopulateStoppingConditionDropDown()
        {
            stoppingConditionDropDownMenu.options.Clear();
            
            // populate the dropdown with stopping conditions
            foreach (var conditionName in StoppingConditionCatalog.GetAllStoppingConditions())
            {
                stoppingConditionDropDownMenu.options.Add(new TMP_Dropdown.OptionData(conditionName));
            }
            
            stoppingConditionDropDownMenu.RefreshShownValue();
            StoppingConditionItemSelected(); // to set initial state
            
            // hook listener
            stoppingConditionDropDownMenu.onValueChanged.AddListener(delegate { StoppingConditionItemSelected(); });
        }

        private void PopulateAlgorithmChooserDropdowns()
        {
            // evil and good drop down menus
            
            // populate the good agent drop down menu
            goodAgentAlgoDropDownMenu.options.Clear(); // just to be sure
            foreach (var brainName in BrainCatalog.GetAllGoodBrainNames())
            {
                goodAgentAlgoDropDownMenu.options.Add(new TMP_Dropdown.OptionData(brainName));
            }
            
            // populate the evil agent drop down menu
            evilAgentAlgoDropDownMenu.options.Clear();
            foreach (var brainName in BrainCatalog.GetAllEvilBrainNames())
            {
                evilAgentAlgoDropDownMenu.options.Add(new TMP_Dropdown.OptionData(brainName));
            }

            goodAgentAlgoDropDownMenu.RefreshShownValue();
            evilAgentAlgoDropDownMenu.RefreshShownValue();
            
            AlgorithmDropDownItemSelected(goodAgentAlgoDropDownMenu , true ); //important, to set brain in state
            AlgorithmDropDownItemSelected(evilAgentAlgoDropDownMenu , false );
            
            // hook listener
            goodAgentAlgoDropDownMenu.onValueChanged.AddListener(delegate { AlgorithmDropDownItemSelected(goodAgentAlgoDropDownMenu , true ); });
            evilAgentAlgoDropDownMenu.onValueChanged.AddListener(delegate { AlgorithmDropDownItemSelected(evilAgentAlgoDropDownMenu , false ); });
        }
        
        void Update()
        {
            _stateManager.Update();
        }

        public void OnPlayPressed()
        {
            // user wants to start the Agent
            // send the request to the Game state instance, it will manage it from there
            if (GameStateManager.Instance.State == GameStateManager.GameState.RUNNING) // button serves as pause button
            {
                GameStateManager.Instance.PauseGame();
                resetButton.interactable = true;
            }
            else // button serves as run
            {
                GameStateManager.Instance.StartGame();
                resetButton.interactable = false; // shouldn't be able to press reset while running 
            }
            
            // update UI state
            goodAgentAlgoDropDownMenu.interactable = false;
            evilAgentAlgoDropDownMenu.interactable = false;
            changeMapButton.interactable = false;
            stopButton.interactable = true;
        }

        public void OnResetPressed()
        {
            // user wants to reset everything
            // send the request to the Game state instance, it will manage it from there
            GameStateManager.Instance.ResetGame();
            
            // set UI to interactable
            playPauseButton.interactable = true;
            goodAgentAlgoDropDownMenu.interactable = true;
            evilAgentAlgoDropDownMenu.interactable = true;
            changeMapButton.interactable = true;
            resetButton.interactable = true;
            stopButton.interactable = false;
        }

        public void OnStopPressed()
        {
            // user wants to stop
            GameStateManager.Instance.StopGame();
            
            resetButton.interactable = true;
            
            //TODO: refactor these interactions
            // set UI to non interactable
            playPauseButton.interactable = false;
            goodAgentAlgoDropDownMenu.interactable = false;
            evilAgentAlgoDropDownMenu.interactable = false;
            changeMapButton.interactable = false;
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
        private void AlgorithmDropDownItemSelected(TMP_Dropdown dropDown , bool isGood )
        {
            var text = dropDown.options[dropDown.value].text;
            
            // picks from good brain list and bad brain list according to flag isGood 
            _stateManager.SetCurrentBrain( isGood ? BrainCatalog.GetGoodBrain(text) : BrainCatalog.GetEvilBrain(text) , isGood );
            dropDown.RefreshShownValue();
        }

        private void StoppingConditionItemSelected()
        {
            var conditionName = stoppingConditionDropDownMenu.options[stoppingConditionDropDownMenu.value].text;

            _stateManager.SetCurrentStoppingCondition(StoppingConditionCatalog.GetCondition(conditionName));
            stoppingConditionDropDownMenu.RefreshShownValue();
        }

        private void BoardEvaluatorItemSelected()
        {
            var evaluatorName = boardEvaluatorDropDownMenu.options[boardEvaluatorDropDownMenu.value].text;
            
            _stateManager.SetCurrentBoardEvaluator(BoardEvaluatorCatalog.GetBoardEvaluator(evaluatorName));
            boardEvaluatorDropDownMenu.RefreshShownValue();
        }
        
        // User wants to change the agent speed
        public void OnSpeedSliderValueChanged( float value )
        {
            GameStateManager.Instance.SetSpeed( (int)(value * 9 + 1)  );
        }
    }
}

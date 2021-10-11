using UnityEngine;
using Visualizer;

public class Main : MonoBehaviour
{
    // Starts the game, and handles main UI ( not editor UI )
    
    public GameObject mainUI;
    public GameObject mapEditorUI;
    
    private GameStateManager Manager;
    
    void Start()
    {
        // create a GameStateManager to keep track of practically everything
        Manager = new GameStateManager();
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
        mapEditorUI.SetActive(false);
        mainUI.SetActive(true);
    }
}

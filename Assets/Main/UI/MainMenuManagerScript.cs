using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManagerScript : MonoBehaviour
{

    public Button startButton; // Reference to the Start button
    public Button optionsButton; // Reference to the Options button
    public Button exitButton; // Reference to the Exit button

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(OnStartGameButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        exitButton.onClick.AddListener(OnQuitGameButtonClicked);
    }

    private void OnStartGameButtonClicked()
    {
        // Logic to open options menu
        Debug.Log("Play button clicked");
        SceneManager.LoadScene("SampleScene"); // Load the game scene
    }

    private void OnOptionsButtonClicked() // do stuff later idk add audio and stuff maybe remap the buttons and sensitivities if we want to 
    {
        // Logic to open options menu
        Debug.Log("Options button clicked");
    }

    private void OnQuitGameButtonClicked() //Maybe later we will want to do something else here
    {
        // Logic to quit the game
        Debug.Log("Quit button clicked"); 
        Application.Quit(); // This will quit the application
    }
}

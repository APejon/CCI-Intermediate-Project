using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Reference to the main menu UI panel
    public GameObject mainMenuPanel;

    // Reference to the Start button
    public Button startButton;
    // options
    // credits
    // Website
    // exit button

    // Timer variable
    public GameObject introScreen;
    // Game hud (main gameplay)
    // credits
    // options



    // Start is called before the first frame update
    void Start()
    {
        // Make sure the menu is visible at the start
        mainMenuPanel.SetActive(true);

        // Add listener to the Start button
        //startButton.onClick.AddListener(OnStartButtonClicked);



        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Check for Enter key press
        if (Input.GetKeyDown(KeyCode.Return)) // Return is "Enter" key
        {
            btnStartGame();
        }

        // Optional: if you want to see the time passing before the button is clicked, update the timer here

    }

    // Method to hide the main menu and start the game
    public void btnStartGame()
    {
        // Set timer to 1 when the game starts

        Time.timeScale = 1f;

        // Hide the main menu
        mainMenuPanel.SetActive(false);
        // introScreen.StartIntro = true;

    }


    // Update the timer display on the screen

}
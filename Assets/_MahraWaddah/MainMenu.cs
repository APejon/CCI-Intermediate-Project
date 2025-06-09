using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour


{

    // Reference to the main menu UI panel
    public GameObject mainMenuPanel;

    // Reference to the Start button
    public GameObject creditsPanel;
    public GameObject introScreen;
    public GameObject OptionsScreen;

    public void OptionsClicked(){
        Debug.Log("Options");
        
    }

    public GameObject Gamehud()
    {
        GameObject x = new GameObject();
        return x;
    }

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


    public void btnExitbutton()
    {
        // This will quit the application
        Application.Quit();

        // This is only for the editor (won’t affect the actual build)
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
public void btncredits  ()
    {
        creditsPanel.SetActive(true);
    }

    public void TurnOffAllScreens()
    {
        mainMenuPanel.SetActive(false);
        OptionsScreen.SetActive(false);
        creditsPanel.SetActive(false);
    }




    public void btnOption()
    {
      
      TurnOffAllScreens();
      OptionsScreen.SetActive(true);
    }



}
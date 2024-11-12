using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject frontPage;
    public Button continueButton;
    public GameObject settingsMenu;
    public GameObject statisticsMenu;
    public TMP_Text timePlayedText;
    public TMP_Text deathsText;
    public TMP_Text enemiesKilledText;
    public TMP_Text gallonsSpilledText;
    public GameObject controlsMenu;
    public static PauseMenuManager Instance;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("PauseMenuManager created");
            DontDestroyOnLoad(gameObject);

            // Turn off pause menu initially
            pauseMenu.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate PauseMenuManager destroyed");
        }

    }

    void Start()
    {
        // Listen for continue button to be pressed
        continueButton.onClick.AddListener(OnContinueButtonClick);
    }

    void OnContinueButtonClick()
    {
        GameManager.Instance.UpdateGameState(GameState.Unpaused);
    }

    public KeyCode GetPauseKey()
    {
        if (PlayerPrefs.HasKey("pauseKey"))
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pauseKey"));
        }

        return GameManager.Instance.defaultKeyDict["pauseKey"];
    }

    public void TogglePauseMenu()
    {
        // Subscribe to state change event
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    private void GameManagerOnGameStateChanged(GameState state)
    {
        if (state == GameState.Paused)
        {
            // Pause game
            Time.timeScale = 0;

            // Make sure settings/stats menus are hidden at first
            settingsMenu.SetActive(false);
            statisticsMenu.SetActive(false);
            controlsMenu.SetActive(false);

            // Show pause menu
            pauseMenu.SetActive(true);
            frontPage.SetActive(true);

            // Lock the cursor to the game window and make it visible
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            // Unpause game
            Time.timeScale = 1;

            // Hide pause menu
            pauseMenu.SetActive(false);

            // Lock and hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void UpdateStatisticsMenu()
    {
        // Calculate hours, minutes, and seconds
        int hours = Mathf.FloorToInt(GameManager.Instance.timePlayed / 3600);
        int minutes = Mathf.FloorToInt((GameManager.Instance.timePlayed % 3600) / 60);
        int seconds = Mathf.FloorToInt(GameManager.Instance.timePlayed % 60);

        timePlayedText.text = "Time Played <color=#FFFFFF>" + hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") + "</color>";
        enemiesKilledText.text = "Enemies Killed <color=#FFFFFF>" + GameManager.Instance.enemiesKilled.ToString("F0") + "</color>";
        deathsText.text = "Deaths <color=#FFFFFF>" + GameManager.Instance.deaths.ToString("F0") + "</color>";
        gallonsSpilledText.text = "Gallons of Blood Spilled <color=#FFFFFF>" + GameManager.Instance.gallonsSpilled.ToString("F2") + " Gals</color>";

    }

    public void SetActive(bool active)
    {
        if (active)
        {
            pauseMenu.SetActive(true);
            // In case the player left these open, make sure they're closed when pausing again
            settingsMenu.SetActive(false);
            statisticsMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(false);
        }
    }

    public void QuitToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main Menu");
    }

}

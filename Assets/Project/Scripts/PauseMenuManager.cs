using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(GetPauseKey()))
        {
            TogglePauseMenu();
        }
    }

    KeyCode GetPauseKey()
    {
        if (PlayerPrefs.HasKey("pauseKey"))
        {
            return (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pauseKey"));
        }

        return UpdateControls.Instance.defaultKeyDict["pauseKey"];
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);

        if (isPaused)
        {
            // Pause game
            Time.timeScale = 0;

            // Lock the cursor to the game window and make it visible
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            // Unpause game
            Time.timeScale = 1;

            // Lock and hide the cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void QuitToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("Main Menu");
    }

}

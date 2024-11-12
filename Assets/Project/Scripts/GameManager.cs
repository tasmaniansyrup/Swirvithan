using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Controls")]
    public TMP_InputField forwardInputField;
    public TMP_InputField backwardInputField;
    public TMP_InputField leftInputField;
    public TMP_InputField rightInputField;
    public TMP_InputField pauseInputField;
    public PauseMenuManager pauseMenuManager;

    [Header("Player info")]
    public float currHealth;
    public float currGas;
    public float currStamina;
    public float maxHealth;
    public float maxGas;
    public float maxStamina;

    [Header("Player Stats")]
    public Dictionary<string, float> statsDict;
    public float enemiesKilled;
    public float gallonsSpilled;
    public float timePlayed;
    public float deaths;
    public Dictionary<TMP_InputField, string> inputFieldsDict;
    public Dictionary<string, KeyCode> defaultKeyDict;
    public static bool InstanceExists => Instance != null;
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("GameManager instance created on " + SceneManager.GetActiveScene().name);

            // Set state to unpaused initially
            State = GameState.Unpaused;

            InitializeControls();
            InitializeStats();
            loadStats();
            
            // Activate pause menu long enough for its instance to be created (it will turn itself off after)
            pauseMenuManager.SetActive(true);
        }
        else 
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameManager Destroyed");    
        }
    }

    void Start()
    {
        maxHealth = 100f;
        maxGas = 100f;
        maxStamina = 100f;
        currHealth = 100f;
        currGas = 100f;
        currStamina = 100f;

        if (PlayerPrefs.HasKey("currHealth"))
        {
            currHealth = PlayerPrefs.GetFloat("currHealth");
        }

        if (PlayerPrefs.HasKey("currGas"))
        {
            currGas = PlayerPrefs.GetFloat("currGas");
        }
    }

    void Update()
    {
        if (State != GameState.Paused && Input.GetKeyDown(pauseMenuManager.GetPauseKey()) && SceneManager.GetActiveScene().name != "Main Menu")
        {
            UpdateGameState(GameState.Paused);
        }
        else if (State == GameState.Paused && Input.GetKeyDown(pauseMenuManager.GetPauseKey()))
        {
            // If we're already paused and hit the pause button again, unpause
            UpdateGameState(GameState.Unpaused);
        }

        // Increase playtime only if the player is unpaused and off the main menu
        if (State != GameState.Paused && SceneManager.GetActiveScene().name != "Main Menu")
        {
            timePlayed += Time.deltaTime;
        }
    }

    private void SaveStats()
    {
        // Loop through all stats and save them to player prefs
        foreach (KeyValuePair<string, float> stat in statsDict)
        {
            PlayerPrefs.SetFloat(stat.Key, stat.Value);
        }

        PlayerPrefs.SetFloat("currHealth", currHealth);
        PlayerPrefs.SetFloat("currGas", currGas);

        PlayerPrefs.Save();
    }

    public void InitializeStats()
    {
        statsDict = new Dictionary<string, float>
        {
            {"enemiesKilled", enemiesKilled},
            {"gallonsSpilled", gallonsSpilled},
            {"timePlayed", timePlayed},
            {"deaths", deaths}
        };
    }

    public void loadStats()
    {
        var keys = new List<string>(statsDict.Keys);

        foreach (string key in keys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                statsDict[key] = PlayerPrefs.GetFloat(key);
            }
            else
            {
                // No stats stored, set all to 0
                statsDict[key] = 0f;
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveStats();
    }

    public void InitializeControls()
    {
        // To add a new key to the controls
        // 1. create new input field variable above
        // 2. Add a grabber in GrabInputFields
        // 3. add key to both dictionaries below
        // 4. we're laughing

        // create our input field -> key name dictionary
        inputFieldsDict = new Dictionary<TMP_InputField, string>
        {
            {forwardInputField, "forwardKey"},
            {backwardInputField, "backwardKey"},
            {leftInputField, "leftKey"},
            {rightInputField, "rightKey"},
            {pauseInputField, "pauseKey"}
        };

        // create our key name -> default key dictionary
        defaultKeyDict = new Dictionary<string, KeyCode>
        {
            {"forwardKey", KeyCode.W},
            {"backwardKey", KeyCode.S},
            {"leftKey", KeyCode.A},
            {"rightKey", KeyCode.D},
            {"pauseKey", KeyCode.Escape}
        };
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.Paused:
                HandlePaused();
                break;
            case GameState.Unpaused:
                HandlePaused();
                break;
            // add other states in the future
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    // State functions
    private void HandlePaused()
    {
        pauseMenuManager.TogglePauseMenu();
    }
}

public enum GameState
{
    Paused,
    Unpaused
}
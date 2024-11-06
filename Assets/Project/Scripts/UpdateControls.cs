using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Reflection;
using UnityEngine.SceneManagement;

public class UpdateControls : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [Header("Script References")]
    public PlayerMovement playerMovement;

    [Header("Input Fields")]
    public TMP_InputField forwardInputField;
    public TMP_InputField backwardInputField;
    public TMP_InputField leftInputField;
    public TMP_InputField rightInputField;
    public TMP_InputField pauseInputField;
    private TMP_InputField selectedField;

    private Dictionary<TMP_InputField, string> inputFieldsDict;
    public Dictionary<string, KeyCode> defaultKeyDict;
    private bool isFocused = false;
    private bool isFirstInput;
    public static UpdateControls Instance { get; private set; }
    public static bool InstanceExists => Instance != null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("GameManager instance created: " + SceneManager.GetActiveScene().name);

            InitializeControls();
        }
        else 
        {
            // Destroy(gameObject);
            // Debug.Log("Duplicate Destroyed");
        }
    }

    void InitializeControls()
    {
        // To add a new key to the controls
        // 1. create new input field variable above
        // 2. add key to both dictionaries below
        // 3. we're laughing

        if (forwardInputField == null) Debug.LogError("Forward Input Field is not assigned.");
        if (backwardInputField == null) Debug.LogError("Backward Input Field is not assigned.");
        if (leftInputField == null) Debug.LogError("Left Input Field is not assigned.");
        if (rightInputField == null) Debug.LogError("Right Input Field is not assigned.");

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

        // Update the text of the input fields to reflect keybinds
        UpdateInputFieldText();    
    }
    void Update() 
    {
        if (selectedField != null && isFocused)
        {
            if (Input.anyKeyDown)
            {
                // Loop through keycodes to find which one was pressed
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    // Skip the first input which is the left click into the text box
                    if (isFirstInput)
                    {
                        isFirstInput = false;
                        return;
                    }
                    
                    // Found the keycode pressed
                    if (Input.GetKeyDown(keyCode))
                    {
                        selectedField.text = keyCode.ToString();  // Update the text of the selected InputField
                        SaveControls(selectedField, keyCode);      // Save the keybinding
                        selectedField = null;                     // Deselect after setting the key
                        EventSystem.current.SetSelectedGameObject(null);  // Unfocus the InputField
                    }
                }
            }
        }
    }

    public void SaveControls(TMP_InputField inputField, KeyCode keyCode)
    {
        // Access the key name associated with given inputField and set it to the new key code
        foreach (KeyValuePair<TMP_InputField, string> iField in inputFieldsDict)
        {
            // we need to check if the given keyCode is already set to another bind elsewhere
            // loop through all player prefs and check if the keyCode is set anywhere 
            if (PlayerPrefs.HasKey(iField.Value) && PlayerPrefs.GetString(iField.Value) == keyCode.ToString())
            {
                // We are setting a new key to a bind already in use, reset that old bind to default and log
                PlayerPrefs.DeleteKey(iField.Value);
                Debug.Log(keyCode.ToString() + " was already bound to " + iField.Value + ". " + iField.Value + " bind reset to default");
            }
        }

        // Set new keybind
        PlayerPrefs.SetString(inputFieldsDict[inputField], keyCode.ToString());
        Debug.Log(inputFieldsDict[inputField] + " set to " + keyCode.ToString());
    }

    private void UpdateInputFieldText()
    {
        // Loop through all keybinds to see if any have been set
        foreach (KeyValuePair<TMP_InputField, string> iField in inputFieldsDict)
        {
            if (PlayerPrefs.HasKey(iField.Value))
            {
                // update text of field if a keybind is found
                iField.Key.text = PlayerPrefs.GetString(iField.Value);
            }
            else {
                iField.Key.text = "";
            }
        }
    }

    public void LoadControls()
    {        
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("No PlayerMovement instance found!");
            return;
        }

        foreach (KeyValuePair<TMP_InputField, string> iField in inputFieldsDict)
        {
            //Use reflection to dynamically obtained the class information for the given variable
            Debug.Log("Key name: " + iField.Value);
            var fieldInfo = playerMovement.GetType().GetField(iField.Value);
            if (fieldInfo == null)
            {
                Debug.Log("Key doesn't exist");
                continue;  // skip the rest of this iteration
            }

            if (PlayerPrefs.HasKey(iField.Value))
            {
                // update keybind in playerMovement
                // fieldInfo is apart of the reflection namespace
                // this code essentially turns fieldInfo into the variable to be changed
                if (fieldInfo.FieldType == typeof(KeyCode))
                    {
                        fieldInfo.SetValue(playerMovement, (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(iField.Value)));
                    }
                }
            else {
                // Grab the name of the key and then grab the default key based on that name
                fieldInfo.SetValue(playerMovement, defaultKeyDict[iField.Value]);
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("selected");
        isFirstInput = true;
        isFocused = true;
        selectedField = eventData.selectedObject.GetComponent<TMP_InputField>();  // Store the selected InputField
        selectedField.caretWidth = 0;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("deselected");
        isFocused = false;
        selectedField = null;
    }

    public void ResetKeybinds()
    {
        // reset all saved binds
        PlayerPrefs.DeleteAll();
        UpdateInputFieldText();
        Debug.Log("Keybinds Successfully Reset");
        
        // button was staying highlighted for some reason so I turned it off using brute force
        EventSystem.current.SetSelectedGameObject(null);
    }
}

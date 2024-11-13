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

    private TMP_InputField selectedField;
    private Dictionary<TMP_InputField, string> inputFieldsDict;
    private Dictionary<string, KeyCode> defaultKeyDict;

    private bool isFocused = false;
    private bool isFirstInput;

    void Awake()
    {
        inputFieldsDict = GameManager.Instance.inputFieldsDict;
        defaultKeyDict = GameManager.Instance.defaultKeyDict;
    }

    void Start()
    {
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
        PlayerPrefs.Save();
        Debug.Log(inputFieldsDict[inputField] + " set to " + keyCode.ToString());
    }

    public void UpdateInputFieldText()
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
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            return;
        }

        playerMovement = FindObjectOfType<PlayerMovement>();

        foreach (KeyValuePair<TMP_InputField, string> iField in inputFieldsDict)
        {
            //Use reflection to dynamically obtained the class information for the given variable
            var fieldInfo = playerMovement.GetType().GetField(iField.Value);
            if (fieldInfo == null)
            {
                Debug.Log("Key doesn't exist in playermovement script");
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

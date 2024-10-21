using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UpdateControls : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public PlayerMovement playerMovement;
    public TMP_InputField forwardInputField;
    public TMP_InputField backwardInputField;
    public TMP_InputField leftInputField;
    public TMP_InputField rightInputField;
    private TMP_InputField selectedField;
    private bool isFocused = false;
    private bool isFirstInput;

    void Update() 
    {
        if (selectedField != null && isFocused)
        {
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (isFirstInput)
                    {
                        isFirstInput = false;
                        return;
                    }

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
        if (inputField == forwardInputField)
        {
            PlayerPrefs.SetString("ForwardKey", keyCode.ToString());
        }
        else if (inputField == backwardInputField)
        {
            PlayerPrefs.SetString("BackwardKey", keyCode.ToString());
        }
        else if (inputField == leftInputField)
        {
            PlayerPrefs.SetString("LeftKey", keyCode.ToString());
        }
        else if (inputField == rightInputField)
        {
            PlayerPrefs.SetString("RightKey", keyCode.ToString());
        }
    }

    public void LoadControls()
    {        
        if (PlayerPrefs.HasKey("ForwardKey"))
        {
            playerMovement.forwardKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ForwardKey"));
        }
        else {
            playerMovement.forwardKey = KeyCode.W;
        }

        if (PlayerPrefs.HasKey("BackwardKey"))
        {
            playerMovement.backwardKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("BackwardKey"));
        }
        else
        {
            playerMovement.backwardKey = KeyCode.S;
        }

        if (PlayerPrefs.HasKey("LeftKey"))
        {
            playerMovement.leftKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftKey"));
        }
        else
        {
            playerMovement.leftKey = KeyCode.A;
        }

        if (PlayerPrefs.HasKey("RightKey"))
        {
            playerMovement.rightKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightKey"));
        }
        else
        {
            playerMovement.rightKey = KeyCode.D;
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
}

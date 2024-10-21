using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpdateControls : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public PlayerMovement playerMovement;
    public InputField forwardInputField;
    public InputField backwardInputField;
    public InputField leftInputField;
    public InputField rightInputField;
    public InputField selectedField;
    public bool isFocused = false;

    void updateControls() 
    {
        if (selectedField != null && Input.anyKeyDown)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    selectedField.text = keyCode.ToString();  // Update the text of the selected InputField
                    SaveControls(selectedField, keyCode);      // Save the keybinding
                    selectedField = null;                     // Deselect after setting the key
                    EventSystem.current.SetSelectedGameObject(null);  // Unfocus the InputField
                    break;
                }
            }
        }
    }

    public void SaveControls(InputField inputField, KeyCode keyCode)
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
        PlayerPrefs.DeleteAll();  // DELETE THIS
        
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
        selectedField = eventData.selectedObject.GetComponent<InputField>();  // Store the selected InputField
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selectedField = null;
    }
}

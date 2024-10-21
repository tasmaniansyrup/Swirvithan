using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResizeInputField : MonoBehaviour
{    
    private TMP_InputField inputField;
    private RectTransform rectTransform;
    void Start()
    {
        inputField = GetComponent<TMP_InputField>(); // Change to InputField if needed
        rectTransform = GetComponent<RectTransform>();
        

        // Subscribe to the onValueChanged event to resize the Input Field
        inputField.onValueChanged.AddListener(OnTextChanged);
    }
    private void OnTextChanged(string text)
    {
        // Set the width of the Input Field based on the text length
        float width = CalculateWidth(text);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    private float CalculateWidth(string text)
    {
        // Adjust this calculation based on your font settings and padding
        float padding = 20f; // Add padding to the width
        float characterWidth = 10f; // Approximate width per character; adjust as needed
        return text.Length * characterWidth + padding;
    }
}

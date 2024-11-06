using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ImageOnButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject image;
    public float offsetX;
    private Button button;
    private TMP_Text buttonText;
    public int normalFontSize = 24;
    public int hoverFontSize = 26;
    void Start()
    {
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<TMP_Text>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show the image and position it next to the button
        image.SetActive(true);
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        image.transform.position = buttonRect.position + new Vector3(offsetX, 7.5f, 0);
        
        // Change font size
        if (buttonText != null)
        {
            buttonText.fontSize = hoverFontSize;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide the image when not hovering
        image.SetActive(false);
        if (buttonText != null)
        {
            buttonText.fontSize = normalFontSize;
        }
    }

    public void RemoveChainsaw()
    {
        // Hide the image when not hovering
        image.SetActive(false);
        if (buttonText != null)
        {
            buttonText.fontSize = normalFontSize;
        }
    }

}

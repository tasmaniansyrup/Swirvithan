using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScaleSprite : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
     // Scale factor for when the sprite is clicked
    public Vector3 scaleIncrease = new Vector3(1.5f, 1.5f, 1.5f);
    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Scale the sprite up
        transform.localScale = originalScale + scaleIncrease;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset the sprite scale back to the original size
        transform.localScale = originalScale;
    }
}

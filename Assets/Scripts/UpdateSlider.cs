using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomHandle : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    // Reference to the slider
    public Slider slider;

    // This will be called when the user clicks (pointer down) on the handle image
    public void OnPointerDown(PointerEventData eventData)
    {
        // Forward the pointer down event to the slider to engage dragging
        if (slider != null)
        {
            slider.OnPointerDown(eventData);
        }
    }

    // This will be called when the user drags the handle image
    public void OnDrag(PointerEventData eventData)
    {
        // Forward the drag event to the slider to update its value
        if (slider != null)
        {
            slider.OnDrag(eventData);
        }
    }
}

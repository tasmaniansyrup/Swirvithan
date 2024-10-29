using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class promptHandler : MonoBehaviour
{

    public float interactionDistance;
    public Camera playerCamera;
    public LayerMask interactLayer;
    public GameObject interactPrompt;
    public KeyCode interactKey;
    private RaycastHit playerCursor;
    public bool canInteract;


    // Start is called before the first frame update
    void Start()
    {
        interactPrompt.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        canInteract = Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out playerCursor, interactionDistance, interactLayer);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward, Color.red, 1f);

        if(canInteract){
            interactPrompt.SetActive(true);
            if(Input.GetKey(interactKey))
            {
                doorController dc = playerCursor.collider.GetComponent<doorController>();
                dc.openDoor();
                Debug.Log("Fartmxdownaclonsd");
            }
        } 
        else {
            interactPrompt.SetActive(false);
        }
    }
}
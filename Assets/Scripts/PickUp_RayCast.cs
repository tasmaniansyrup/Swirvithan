using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PickUp_RayCast : MonoBehaviour
{

    public WeaponController wc;
    [SerializeField]
    private LayerMask spiritOrbs;
    [SerializeField]
    private LayerMask conversationLayer;

    [SerializeField]
    private Transform playerCameraTransform;

    [SerializeField]
    private TextMeshProUGUI PromptUserUI;
    // [SerializeField]
    // private GameObject BunnyTalk;
    [SerializeField]
    private GameObject Conversation;
    [SerializeField]
    private GameObject gameFinish;

    [SerializeField]
    [Min(1)]
    private float hitRange = 5;

    private RaycastHit hit;

    public Text scoreText;

    public ParticleToPoint ptp;

    public ParticleSystem spiritParticles;
    public LayerMask door;
    public LayerMask bed;

    public Animator doorAnim;

    //int num_rabbits = 0;

    private void Update()
    {
        //Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward*hitRange, Color.red);
        if(hit.collider == null)
        {
            PromptUserUI.enabled = false;
            // BunnyTalk.SetActive(false);
        }
        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, spiritOrbs))
        {
            PromptUserUI.text = "Press E to collect";
            PromptUserUI.enabled = true;
            if(Input.GetKey(KeyCode.E))
            {
                //Destroy(hit.transform.gameObject);
                //num_rabbits += 1;
                hit.collider.enabled = false;
                spiritParticles = hit.transform.gameObject.GetComponentInParent<ParticleSystem>();

                ptp.enabled = true;
                ptp.particleSystem = spiritParticles;


                //Debug.Log(spiritParticles.name);
                PromptUserUI.enabled = false;
                wc.specialAttackPercentage += 5;
                wc.specialAttackBar.fillAmount = wc.specialAttackPercentage/100;
                //scoreText.text = "Rabbits: " + num_rabbits.ToString();
                //Debug.Log(num_rabbits);
            }
        }
        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, door)) {
            if(Input.GetKeyDown(KeyCode.E)) {
                if(!doorAnim.GetBool("IsOpen")) {
                    doorAnim.SetBool("IsOpen", true);
                }
                else {
                    doorAnim.SetBool("IsOpen", false);
                }
            }
        }
        if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, bed)) {
            PromptUserUI.text = "Press E to sleep";
            PromptUserUI.enabled = true;
            if(Input.GetKeyDown(KeyCode.E)) {
                
            }
        }
        // if(Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, conversationLayer))
        // {
        //     BunnyTalk.SetActive(true);
        //     if(Input.GetKey(KeyCode.E) && num_rabbits < 15)
        //     {
        //         Conversation.SetActive(true);
        //     }
        //     if(Input.GetKey(KeyCode.E) && num_rabbits == 15)
        //     {
        //         gameFinish.SetActive(true);
        //     }
        // }
        // if(!Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, conversationLayer))
        // {
        //     Conversation.SetActive(false);
        //     gameFinish.SetActive(false);
        // }

    }

    
}

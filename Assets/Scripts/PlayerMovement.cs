using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    public float walkSpeed;
    public bool isWalking;
    public bool isRunning;
    public float regenTimer;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    bool grounded;

    [Header("Step Controls")]

    public GameObject stepRayHeight;
    public GameObject stepRayGround;
    public float stepMaxHeight;
    public float stepForce;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private float angle;
    private bool exitingSlope;

    public float expectedSlopeDistance;
    public float realSlopeDistance;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 limitedVel;
    Vector3 moveDirection;
    public Animator animator;
    public Image staminaBar;
    public float staminaAmount;
    public bool isRegening;

    Rigidbody rb;
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    private void Start()
    {
        //animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;

        readyToJump = true;

        //stepRayHeight.transform.position = new Vector3(stepRayHeight.transform.position.x, -1 + stepMaxHeight, stepRayHeight.transform.position.z);
    }

    private void Awake() {
        stepRayHeight.transform.position = new Vector3(stepRayHeight.transform.position.x, stepRayHeight.transform.position.y -1 + stepMaxHeight, stepRayHeight.transform.position.z);
    }


    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Vector3 v1 = new Vector3(transform.position.x, transform.position.y - .63f, transform.position.z);
        Gizmos.DrawSphere(v1, 0.6f);
    }

    private void FixedUpdate() {
        //Debug.Log(rb.velocity.magnitude);
        stepForce = 8 * moveSpeed/6f;
        
        
        MovePlayer();
        SpeedControl();
        stepController();

        if(grounded) {
            expectedSlopeDistance = (.3f - .3f * Mathf.Sin(90f - angle))/Mathf.Sin(90f - angle);
            realSlopeDistance = groundCheck.transform.position.y - slopeHit.transform.position.y;
        }

        if(!Physics.Raycast(stepRayGround.transform.position, moveDirection, 0.4f, whatIsGround) && !(Physics.Raycast(groundCheck.position, moveDirection.normalized, 1f, whatIsGround)) && realSlopeDistance <= expectedSlopeDistance && grounded) {
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, slopeHit.normal);
        }


        // Debug.DrawRay(groundCheck.position, moveDirection.normalized, Color.red, 1f);
        // if(Physics.Raycast(groundCheck.position, moveDirection.normalized, out slopeHit, 1f, whatIsGround)) {
        //     Debug.Log("big boogs");
        //     rb.velocity = Vector3.ProjectOnPlane(rb.velocity, slopeHit.normal);
        // }

        // Debug.Log(Physics.CheckSphere(v2, 0.62f, whatIsGround));

        // if(!Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround) && !OnSlope()) {
        //     if(Physics.Raycast(groundCheck.position, Vector3.down, 1f, whatIsGround)) {
        //         rb.AddForce(Vector3.down * 10f, ForceMode.Impulse);
        //     }
        // }

        // if(rb.velocity.y is > 0 && readyToJump && !OnSlope() && !grounded) {
        //     Vector3 newVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //     rb.velocity = newVel;
        // }

        // handle drag
        if(grounded){
            rb.drag = groundDrag;
        }
        else{
            rb.drag = 0;
        }
    }

    private void Update()
    {

        MyInput();
        StateHandler();

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        // Animations
        if(animator != null) {
            if(!grounded) 
            {
                animator.SetInteger("Speed", 3);
                
            }
            else if(moveDirection == Vector3.zero)
            {
                animator.SetInteger("Speed", 0);
                //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60, .03f);
                
            }
            else if(!Input.GetKey(KeyCode.LeftShift) || staminaAmount < 0.01)
            {
                animator.SetInteger("Speed", 1);
                //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60, .03f);
                
            }
            else if (staminaAmount > 0.01)
            {
                animator.SetInteger("Speed", 2);
                //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 65, .1f);
            }
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKeyDown(crouchKey)) {
            Debug.Log("You're short");
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 7.5f, ForceMode.Impulse);
            //transform.position = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);
        }

        if(Input.GetKeyUp(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

    }

    private void StateHandler() {

        if(Input.GetKey(crouchKey)) {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        if(grounded && Input.GetKey(sprintKey) && staminaAmount >= 0.01)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            isRunning = true;
        }
        else if(grounded && !Input.GetKey(sprintKey) || staminaAmount < 0.01)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            isRunning = false;
        }
        else {
            state = MovementState.air;
            isRunning = false;
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(OnSlope() && !exitingSlope && state != MovementState.crouching) {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if(rb.velocity.y > 0) {
                rb.AddForce(Vector3.down * 40f, ForceMode.Force);
            }
        }

        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {

        if(OnSlope() && !exitingSlope) {
            if(rb.velocity.magnitude > moveSpeed) {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        else {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            IEnumerator regenStamina = staminaRegen();

            // limit vel
            if(flatVel.magnitude > moveSpeed)
            {
                limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
            // if(Input.GetKey(sprintKey) && staminaAmount >= 0.01 && flatVel.magnitude > 1)
            // {
            //     isRunning = true;
            //     isRegening = false;
            //     staminaAmount -= 30 * Time.deltaTime;
            //     staminaBar.fillAmount = staminaAmount/100f;
            //     regenTimer = 1.4f;
            // }
            // else if(staminaAmount < 100f && !Input.GetKey(sprintKey) && !isRunning){
            //     //StartCoroutine(regenStamina);
            // }
            // else {
            //     isRegening = false;
            // }
            // if(regenTimer <= 0 && staminaAmount < 100) {
            //     staminaAmount += 20 * Time.deltaTime;
            //     staminaBar.fillAmount = staminaAmount/100f;
            // }
            // if(regenTimer > 0) {
            //     regenTimer -= Time.deltaTime;
            // }
        }

    }

    IEnumerator staminaRegen() {
        yield return new WaitForSeconds(2f);
        if(!isRunning) {
            isRegening = true;
        }
    }

    private void Jump()
    {

        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        exitingSlope = false;

        readyToJump = true;
    }

    private bool OnSlope() {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, whatIsGround)) {
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        
        return false;
    }
    
    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void stepController() {
        
        RaycastHit hitLower;

        Vector3 playerDir = new Vector3(rb.velocity.normalized.x, 0f, rb.velocity.normalized.z);
        Debug.DrawRay(stepRayGround.transform.position, playerDir, Color.red, .4f);
        if(Physics.Raycast(stepRayGround.transform.position, moveDirection, out hitLower, 0.4f + 1.5f/moveSpeed, whatIsGround)) {
            RaycastHit hitUpper;
            float stepAngle = Vector3.Angle(Vector3.up, hitLower.normal);
            if(!Physics.Raycast(stepRayHeight.transform.position, moveDirection, out hitUpper, 0.5f + 1.5f/moveSpeed, whatIsGround) && stepAngle > 60) {
                Debug.Log("Stepped up");
                rb.position += new Vector3(0f, stepForce * Time.fixedDeltaTime, 0f);
            }
        }
    }
}

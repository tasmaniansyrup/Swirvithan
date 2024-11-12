using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Handles player movement like jump, crouch, sprint, slope movement and stamina
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
    private UpdateControls updateControls;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode backwardKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public bool grounded;

    [Header("Step Controls // gonna delete later maybe")]

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

    public WeaponController wc;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Start()
    {
        Debug.Log("Loading Controls");
        // Load controls
        updateControls = FindObjectOfType<UpdateControls>();
        updateControls.LoadControls();

        //animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;

        readyToJump = true;

        //stepRayHeight.transform.position = new Vector3(stepRayHeight.transform.position.x, -1 + stepMaxHeight, stepRayHeight.transform.position.z);
    }
    

    /**
     *  This function allows for drawing shapes and vectors in unity viewable in the scene
     *  Only place you can do this ^
     */
    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Vector3 v1 = new Vector3(transform.position.x, transform.position.y - .63f, transform.position.z);
        Gizmos.DrawSphere(v1, 0.6f);
    }

    /**
     * This runs 30 times a second
     * Best to use FixedUpdate for physics based updating and detection
     * * NEVER USE FOR PLAYER INPUT
     */
    private void FixedUpdate() {        
        
        // Updates player rb
        MovePlayer();
        SpeedControl();
        rb.AddForce(Vector3.down * 7.5f, ForceMode.Force);        

        // Calculates how far away the ground should be from player based on slope
        if(grounded) {
            expectedSlopeDistance = .3f / Mathf.Cos(angle * Mathf.PI/180f) - .3f;
            realSlopeDistance = slopeHit.distance - 1f;
        }

        // Uses expected ground distance to check if player is ground and projects player velocity orthogonal to the slope/plane
        if(realSlopeDistance <= expectedSlopeDistance && grounded && state != MovementState.crouching) {
            rb.velocity = Vector3.ProjectOnPlane(rb.velocity, slopeHit.normal);
        }

        /**
         *  If player is moving up, but isn't on the ground and didn't jump, stop vertical momentum
         *  Used to handle players exiting slopes
         */
        if(rb.velocity.y > 0 && readyToJump && !OnSlope() && !grounded) {
            Vector3 newVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.velocity = newVel;
        }

        // handle drag
        if(grounded){
            rb.drag = groundDrag;
        }
        else{
            rb.drag = 0;
        }
    }

    /**
     *  Is called everyframe - dependent on fps when running game;
     *  Used mostly for player input and interaction to provide fastest things
     */
    private void Update()
    {
        // take player input :o
        MyInput();
        StateHandler();

        // update grounded if player is within .1f of ground
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        // Animations 
        // * Keep // camera.fieldOfView for future adjustment 
        if(animator != null) {
            // Air/Jump animation
            if(!grounded && !wc.isAttacking) 
            {
                animator.speed = 0.2f;

                
            } // Idle animation
            else if(moveDirection == Vector3.zero)
            {
                animator.SetInteger("Speed", 0);
                animator.speed = 1f;
                //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60, .03f);
                
            } // Walk animation
            else if(!Input.GetKey(KeyCode.LeftShift) || staminaAmount < 0.01)
            {
                animator.SetInteger("Speed", 1);
                animator.speed = 1f;
                //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 60, .03f);
                
            } // Sprint animation
            else if (staminaAmount > 0.01)
            {
                animator.SetInteger("Speed", 2);
                animator.speed = 1f;
                //camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, 65, .1f);
            }
        }
    }


    /**
     * Get's player input and assigns to variables
     */
    private void MyInput()
    {
        // These would be WASD and the arrow keys
        // horizontalInput = Input.GetAxisRaw("Horizontal");
        // verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = 0;
        verticalInput = 0;

        if (Input.GetKey(leftKey))
        {
            horizontalInput -= 1f;  // move left
        }
        if (Input.GetKey(rightKey))
        {
            horizontalInput += 1f;  // move right
        }
        if (Input.GetKey(backwardKey))
        {
            verticalInput -= 1f;  // move backward
        }
        if (Input.GetKey(forwardKey))
        {
            verticalInput += 1f;  // move forward
        }
        

        // If player can jump and is ground, will jump player when jumpKey (space) is pressed
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Scales player to half size (vertically) and adds force down to keep player grounded
        if(Input.GetKeyDown(crouchKey)) {
            Debug.Log("You're short");
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 7.5f, ForceMode.Impulse);
        }

        // Scale player double size (height)
        if(Input.GetKeyUp(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

    }

    // Handles what state player is in
    // Kinda useless imo
    private void StateHandler() {

        // Player is crouching
        if(Input.GetKey(crouchKey)) {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        // Player is sprinting
        else if(grounded && Input.GetKey(sprintKey) && staminaAmount >= 0.01)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            isRunning = true;
        } // Player is walking
        else if(grounded && !Input.GetKey(sprintKey) || staminaAmount < 0.01)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            isRunning = false;
        }
        else { // Player is in air
            state = MovementState.air;
            isRunning = false;
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // If on slope we want to have the player move along the the slope angle
        if(OnSlope() && !exitingSlope && state != MovementState.crouching) {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            // Artificial gravity on slope
            // ? Change in future idk
            if(true) {
                rb.AddForce(-slopeHit.normal * 40f, ForceMode.Force);
            }
        }
        // If not on slope, adds force in direction player is looking/pressing
        else if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // disable gravity on slope
        rb.useGravity = !OnSlope();
    }


    // Controls the speed of character rigidbody
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

            // * Keep this code, its working and used for stamina regen/degen.
            // * Commented out for development/testing
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

    // Time until stamina begins to regen after sprinting
    IEnumerator staminaRegen() {
        yield return new WaitForSeconds(2f);
        if(!isRunning) {
            isRegening = true;
        }
    }

    // Adds force upward when called
    private void Jump()
    {

        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Called when player has landed
    private void ResetJump()
    {
        exitingSlope = false;

        readyToJump = true;
    }

    // Returns if player is on slope as well as updates slopeHit
    private bool OnSlope() {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f, whatIsGround)) {
            angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        
        return false;
    }
    
    // Return vector orthogonal to slope
    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MovementScript : MonoBehaviour
{
    [Header("References")]
    public Transform orientor;
    public TMP_Text spedometer;
    public LayerMask groundMask;
    [Header("Movement")]
    public float speed; // regular speed
    public float boostSpeed; // speed while sprinting
    public float jumpHeight;
    public float coyoteTime = 0.15f;
    [Header("Physics")]
    public float gravity;
    public float airControl;
    public float drag;
    public float staticFriction;
    public float kineticFriction;
    [Header("Player Attributes")]
    public float playerHeight;
    [Header("Speed Caps")]
    public float speedCap;
    public float boostCap;
    public float masterSpeedCap;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool isInputting;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isMoving;
    private bool isBoosting;
    private int jumpCounter = 2;
    private float coyoteCounter;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update() 
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        spedometer.text = horizontalVelocity.magnitude.ToString("F1");
    }

    void FixedUpdate()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        isMoving = horizontalVelocity.sqrMagnitude > 0.01;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        //Player Height * 0.5 gets to the center of the bottom, and then + 0.2 gets just bellow the feel to penetrate whats bellow.
        if (isGrounded && !wasGrounded) jumpCounter = 2;

        ApplyGravity();
        ApplyFriction();
        ApplyAirDrag();
        MovePlayer();
        ClampSpeed();

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;

        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }

        wasGrounded = isGrounded;
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            Vector3 gravityForce = Physics.gravity * gravity * rb.mass;
            rb.AddForce(gravityForce, ForceMode.Force);
        }
    }

    private void ApplyFriction()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (!isInputting && isGrounded)
        {
            rb.linearVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, staticFriction) + Vector3.up * rb.linearVelocity.y;
        }
        else if (isGrounded)
        {
            rb.linearVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, kineticFriction) + Vector3.up * rb.linearVelocity.y;
        }
    }

    private void ApplyAirDrag()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (isInputting)
        {
            Vector3 dragDir = -horizontalVelocity.normalized;
            float dragAmount = drag * horizontalVelocity.sqrMagnitude * rb.mass;
            if (!isGrounded)
            {
                rb.AddForce(dragDir * dragAmount, ForceMode.Force);
            }
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientor.forward * moveInput.y + orientor.right * moveInput.x; //For Future note, transform.forward represents the blue arrow, and transform.right represents the red arrow. The y axis or the green arrow is transform.up and we aren't touching that now.

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }
        
        float projection = Vector3.Dot(moveDirection, rb.linearVelocity);
        float activeCap = isBoosting ? boostCap : speedCap;
        float activeSpeed = isBoosting ? boostSpeed : speed;
        float headroom = activeCap - projection;
        float force = Mathf.Min (activeSpeed, headroom);

        if (headroom > 0f)
        {
            if (!isGrounded)
            {
                rb.AddForce(moveDirection * force * airControl, ForceMode.VelocityChange);
            }
            else
            {
                rb.AddForce(moveDirection * force, ForceMode.VelocityChange);
            }
            
        }
    }
    private void ClampSpeed()
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (horizontalVelocity.magnitude > masterSpeedCap)
        {
            horizontalVelocity = horizontalVelocity.normalized * masterSpeedCap;
            rb.linearVelocity = horizontalVelocity + (Vector3.up * rb.linearVelocity.y);
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        isInputting = moveInput != Vector2.zero;
    }

    public void OnJump(InputValue value)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float totalGravity = Physics.gravity.magnitude * gravity;
        float jumpVelocity = Mathf.Sqrt(2 * totalGravity * jumpHeight);
        if ((isGrounded|| coyoteCounter >0 || jumpCounter == 1) && value.isPressed)
        {
            rb.linearVelocity = Vector3.up * jumpVelocity + horizontalVelocity;
            jumpCounter--;
            coyoteCounter = 0f;
        }
    }
    public void OnSprint(InputValue value)
    {
        isBoosting = value.isPressed;
    }
}

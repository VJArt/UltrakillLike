using System;
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
    public float airControl;
    [Header("Player Attributes")]
    public float playerHeight;
    [Header("Speed Caps")]
    public float speedCap;
    public float boostCap;
    public float masterSpeedCap;
    
    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool isBoosting;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    void Update() 
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        spedometer.text = horizontalVelocity.magnitude.ToString("F1");
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        //Player Height * 0.5 gets to the center of the bottom, and then + 0.2 gets just bellow the feel to penetrate whats bellow.
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientor.forward * moveInput.y + orientor.right * moveInput.x; //For Future not, transform.forward represents the blue arrow, and transform.right represents the red arrow. The y axis or the green arrow is transform.up and we aren't touching that now.
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

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

        

        if (horizontalVelocity.magnitude > masterSpeedCap)
        {
            horizontalVelocity = horizontalVelocity.normalized * masterSpeedCap;
            rb.linearVelocity = horizontalVelocity + (Vector3.up * rb.linearVelocity.y);
        }
        
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnSprint(InputValue value)
    {
        isBoosting = value.isPressed;
    }
}

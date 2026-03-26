using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    [Header("Referances")]
    public Transform orientor;
    [Header("Movement")]
    public float speed; // regular speed
    public float sprSpeed; // speed while sprinting

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private bool isSprinting;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    void Update() 
    {
        Debug.Log(moveInput);
        Debug.Log(isSprinting);
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientor.forward * moveInput.y + orientor.right * moveInput.x; //For Future not, transform.forward represents the blue arrow, and transform.right represents the red arrow. The y axis or the green arrow is transform.up and we aren't touching that now.
       
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }
        
        float currentSpeed = isSprinting ? sprSpeed : speed;

        rb.AddForce(moveDirection * currentSpeed, ForceMode.VelocityChange); 
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }
}

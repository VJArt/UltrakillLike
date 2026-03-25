using UnityEngine;

public class MovementScript : MonoBehaviour
{

    private Rigidbody rb;
    private Vector3 verticalVelocity;
    private Vector3 horizontalVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        
    }
}

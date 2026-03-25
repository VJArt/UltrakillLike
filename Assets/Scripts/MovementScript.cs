using UnityEngine;

public class MovementScript : MonoBehaviour
{

    [Header("References")]
    private Rigidbody rb;
    public Transform orientation;

    

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

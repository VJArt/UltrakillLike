using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCameraControllerScript : MonoBehaviour
{
    [Header("Look Settings")]
    [Tooltip("New Input System uses raw deltas. Start low (e.g., 0.05 - 0.1)")]
    public float sensitivity = 0.1f;
    public float xRotation = 0f;

    private InputAction lookAction;

    void Awake()
    {
        lookAction = new InputAction("Look", binding: "<Mouse>/delta");
    }

    void OnEnable()
    {
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (transform.parent != null)
        {
            transform.parent.Rotate(Vector3.up * mouseX);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 8f; 
    public float gravity = -9.81f; 

    [Header("Look Settings")]
    public float mouseSensitivity = 0.1f;
    public float mouseSmoothTime = 0.03f; 
    public Transform cameraTransform;

    [Header("Tilt Settings (Camera Sway)")]
    public float tiltMultiplier = 2f;      // How sensitive the tilt is to mouse movement
    public float maxTilt = 4f;             // Maximum angle the camera can tilt
    public float tiltSmoothTime = 0.1f;    // How smoothly the camera tilts and returns to center

    private CharacterController characterController;
    
    // Rotation tracking
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    
    // Smoothing variables for looking
    private Vector2 currentMouseDelta;
    private Vector2 currentMouseVelocity;

    // Tilt variables
    private float currentTilt = 0f;
    private float currentTiltVelocity = 0f;

    // Movement tracking
    private Vector3 velocity; 
    private Vector3 currentMovement; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        horizontalRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        HandleMovement();
    }

    void LateUpdate()
    {
        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null) return;
        
        // Read raw mouse delta
        Vector2 targetMouseDelta = Mouse.current.delta.ReadValue();

        // Smooth the mouse input for looking around
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseVelocity, mouseSmoothTime);

        float mouseX = currentMouseDelta.x * mouseSensitivity;
        float mouseY = currentMouseDelta.y * mouseSensitivity;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // --- CAMERA TILT LOGIC ---
        // Calculate target tilt based on the raw horizontal mouse movement
        float targetTilt = -targetMouseDelta.x * tiltMultiplier;
        targetTilt = Mathf.Clamp(targetTilt, -maxTilt, maxTilt);

        // Smoothly transition the current tilt towards the target tilt
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref currentTiltVelocity, tiltSmoothTime);

        // Apply horizontal rotation to the player body
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);
        
        // Apply vertical rotation (looking up/down) AND the tilt (Z-axis roll) to the camera simultaneously
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, currentTilt);
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Keyboard.current == null) return;

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;

        Vector3 targetDirection = (transform.right * moveX + transform.forward * moveZ).normalized;
        Vector3 targetMovement = targetDirection * moveSpeed;

        currentMovement = Vector3.Lerp(currentMovement, targetMovement, acceleration * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMovement = currentMovement + velocity;
        characterController.Move(finalMovement * Time.deltaTime);
    }
}
using UnityEngine;
using UnityEngine.InputSystem; // Wymagany nowy namespace

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    [Header("Look Settings")]
    public float mouseSensitivity = 0.1f; // Nowy system podaje większe wartości delta, zmniejsz czułość
    public Transform cameraTransform;

    private CharacterController characterController;
    private float verticalRotation = 0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // Odczyt delty myszy z Nowego Input Systemu
        if (Mouse.current == null) return;
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * mouseSensitivity;
        float mouseY = mouseDelta.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null) return;

        // Odczyt klawiszy WSAD z Nowego Input Systemu
        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        
        characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }
}
using UnityEngine;

public class HeadBobber : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobbingSpeed = 12f;   // How fast the camera bobs (sin frequency)
    public float bobbingAmount = 0.05f; // How high/low the camera goes (sin amplitude)
    public float smoothSpeed = 10f;    // New variable to control how smooth the return to center is
    
    [Header("References")]
    public CharacterController controller; // Reference to the player's CharacterController

    private float defaultPosY = 0f;
    private float timer = 0f;
    private float targetY; // New private variable for calculating the next intended height

    void Start()
    {
        // Store the initial local Y position of the camera
        defaultPosY = transform.localPosition.y;
    }

    void Update()
    {
        // Default target height is the center, assuming idle state
        targetY = defaultPosY;

        // Check if player is moving on the ground (velocity on X or Z axis above threshold)
        if (controller.isGrounded && 
            (Mathf.Abs(controller.velocity.x) > 0.1f || Mathf.Abs(controller.velocity.z) > 0.1f))
        {
            // Player is walking, increment timer and calculate next target point along the sine wave
            timer += Time.deltaTime * bobbingSpeed;
            targetY = defaultPosY + Mathf.Sin(timer) * bobbingAmount;
        }
        else
        {
            // Player is idle or in the air. We don't change targetY here, it remains at defaultPosY.
            // We removed timer = 0f; to prevent the sine wave's phase from snapping if movement starts up quickly again.
        }

        // We now smooth the actual camera position from its current position towards the newly calculated target position.
        // This single line ensures smoothing between idle state (center) and active state (sine wave value), and back again.
        float finalY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * smoothSpeed);
        
        // Apply the final, smoothed vertical position
        transform.localPosition = new Vector3(transform.localPosition.x, finalY, transform.localPosition.z);
    }
}
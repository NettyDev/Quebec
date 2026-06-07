using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HandBobber : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("How much the hand sways based on camera movement.")]
    public float swayAmount = 25f;
    
    [Tooltip("Maximum allowed sway distance from the center.")]
    public float maxSwayAmount = 50f;

    [Tooltip("How smoothly the hand catches up to the camera. Lower is slower/heavier.")]
    public float swaySmoothness = 6f;

    private RectTransform rectTransform;
    private Vector2 basePosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Store the initial position so the UI element doesn't drift away
        if (rectTransform != null)
        {
            basePosition = rectTransform.anchoredPosition;
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Get raw mouse input (represents camera rotation speed this frame)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate target sway offset (invert the input to create a 'lagging behind' effect)
        float moveX = -mouseX * swayAmount;
        float moveY = -mouseY * swayAmount;

        // Clamp the offset to prevent the hand from flying too far off-screen on fast mouse flicks
        moveX = Mathf.Clamp(moveX, -maxSwayAmount, maxSwayAmount);
        moveY = Mathf.Clamp(moveY, -maxSwayAmount, maxSwayAmount);

        // Calculate the final target position relative to the starting base position
        Vector2 targetPosition = new Vector2(basePosition.x + moveX, basePosition.y + moveY);

        // Smoothly interpolate (Lerp) current position to the target position
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * swaySmoothness);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance to interact with objects (e.g., 3 meters)")]
    public float interactRange = 3f;
    
    [Tooltip("Layer mask for interactable objects to optimize physics performance")]
    public LayerMask interactableLayer;

    [Header("Floating UI Settings")]
    [Tooltip("The World Space Canvas or UI container")]
    public GameObject floatingUI;
    
    [Tooltip("TextMeshPro component to display the action name")]
    public TextMeshProUGUI floatingText; // Changed from Text to TextMeshProUGUI
    
    [Tooltip("Vertical offset above the item")]
    public float uiHeightOffset = 0.5f;

    // Stores the item the player is currently looking at
    private InteractableItem currentTarget;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        
        // Hide the UI at the start of the game
        if (floatingUI != null)
        {
            floatingUI.SetActive(false);
        }
    }

    void Update()
    {
        // Shoot a raycast from the center of the camera forward
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Check if the ray hits an object on the specified layer within the range
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            InteractableItem interactable = hit.collider.GetComponent<InteractableItem>();

            if (interactable != null)
            {
                // If we look at a new object, unhighlight the old one and highlight the new one
                if (interactable != currentTarget)
                {
                    if (currentTarget != null) currentTarget.Unhighlight();
                    
                    currentTarget = interactable;
                    currentTarget.Highlight();

                    // Update the text to match the specific item
                    if (floatingText != null)
                    {
                        floatingText.text = currentTarget.interactText;
                    }
                }

                // Always update UI position and rotation while looking at the item
                UpdateFloatingUI();

                // Check if player presses the interact key (F)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    currentTarget.Interact();
                    ClearTarget(); // Clear target as it will be destroyed
                }
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }
    }

    // Handles placing the UI above the item and rotating it towards the player
    private void UpdateFloatingUI()
    {
        if (floatingUI != null && currentTarget != null)
        {
            floatingUI.SetActive(true);
            
            // Position the UI above the item
            floatingUI.transform.position = currentTarget.transform.position + Vector3.up * uiHeightOffset;

            // Billboarding effect: match the camera's rotation so it always faces the player perfectly
            floatingUI.transform.rotation = mainCam.transform.rotation;
        }
    }

    // Helper method to safely remove highlights and hide UI
    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.Unhighlight();
            currentTarget = null;
        }

        // Hide the UI when not looking at anything interactable
        if (floatingUI != null)
        {
            floatingUI.SetActive(false);
        }
    }
}
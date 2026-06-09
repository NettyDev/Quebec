using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance to interact with objects (e.g., 3 meters)")]
    public float interactRange = 3f;
    
    [Tooltip("Layer mask for interactable objects to optimize physics performance")]
    public LayerMask interactableLayer;

    // Stores the item the player is currently looking at
    private InteractableItem currentTarget;

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
                }

                // Check if player presses the interact key (F)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    currentTarget.Interact();
                    currentTarget = null; // Clear target as it will be destroyed
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

    // Helper method to safely remove highlights
    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.Unhighlight();
            currentTarget = null;
        }
    }
}
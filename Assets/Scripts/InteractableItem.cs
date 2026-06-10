using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Text to display when looking at this item (e.g., 'Pick up Battery')")]
    public string interactText = "Podnies";

    [Header("Highlight Settings")]
    [Tooltip("How much the object should glow when looked at.")]
    public float emissionIntensity = 2.0f;
    
    [Tooltip("Color of the glow highlight")]
    public Color highlightColor = Color.green;
    
    private Material mat;
    private Color originalEmissionColor;
    private bool hasEmissionEnabledOriginally;

    [Header("Interaction Events")]
    [Tooltip("What happens when the player presses 'F'")]
    public UnityEvent onInteract;

    [Header("Audio Settings")]
    [Tooltip("Sound to play when the item is picked up or interacted with")]
    public AudioClip interactSound;

    void Start()
    {
        // Get the material of the 3D model to manipulate its color
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            mat = renderer.material;
            hasEmissionEnabledOriginally = mat.IsKeywordEnabled("_EMISSION");
            
            if (mat.HasProperty("_EmissionColor"))
            {
                originalEmissionColor = mat.GetColor("_EmissionColor");
            }
        }
    }

    public void Highlight()
    {
        if (mat != null && mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", highlightColor * emissionIntensity);
        }
    }

    public void Unhighlight()
    {
        if (mat != null && mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor("_EmissionColor", originalEmissionColor);
            
            if (!hasEmissionEnabledOriginally)
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    public void Interact()
    {
        // Play sound at the object's position
        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position);
        }

        // Trigger events
        onInteract.Invoke();

        // Remove the object
        Destroy(gameObject);
    }
}
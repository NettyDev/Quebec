using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour
{
    [Header("Highlight Settings")]
    [Tooltip("How much the object should glow when looked at. Increase this (e.g., 2 or 5) for a stronger glow.")]
    public float emissionIntensity = 2.0f;
    
    [Tooltip("Color of the glow highlight")]
    public Color highlightColor = Color.green;
    
    private Material mat;
    private Color originalEmissionColor;
    private bool hasEmissionEnabledOriginally;

    [Header("Interaction Events")]
    [Tooltip("What happens when the player presses 'F' (e.g., call BatteryRefill.UseBattery)")]
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
            // Note: We use .material (which creates an instance) so we don't change the global material
            mat = renderer.material;
            
            // Check if the material already uses emission
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
            // VERY IMPORTANT: Re-enable the emission keyword before trying to set the color!
            // This fixes the bug where the item only glows the very first time.
            mat.EnableKeyword("_EMISSION");
            
            // Set emission to the chosen color to simulate a glow/highlight outline
            mat.SetColor("_EmissionColor", highlightColor * emissionIntensity);
        }
    }

    public void Unhighlight()
    {
        if (mat != null && mat.HasProperty("_EmissionColor"))
        {
            // Revert to original emission
            mat.SetColor("_EmissionColor", originalEmissionColor);
            
            // If it didn't have emission originally, disable it for performance
            if (!hasEmissionEnabledOriginally)
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    public void Interact()
    {
        // Play the interaction sound at the object's position before destroying it
        // We use PlayClipAtPoint because the object will be destroyed immediately
        if (interactSound != null)
        {
            AudioSource.PlayClipAtPoint(interactSound, transform.position);
        }

        // Trigger all attached events (e.g., adding battery from the other script)
        onInteract.Invoke();

        // Remove the object from the map
        Destroy(gameObject);
    }
}
using UnityEngine;

public class BatteryRefill : MonoBehaviour
{
    [Header("Battery Settings")]
    [Tooltip("How much battery to restore (0-100)")]
    public float refillAmount = 35f;

    [Tooltip("Reference to the Flashlight script")]
    public FlashlightTool2D flashlight;

    // This method will be called by the InteractableItem script
    public void UseBattery()
    {
        // Debug log to confirm that the interaction event reached this script successfully
        Debug.Log("Akcja UseBattery wywołana! Próbuję dodać prąd...");
        
        if (flashlight != null)
        {
            flashlight.AddBattery(refillAmount);
            Debug.Log("Prąd dodany pomyślnie.");
        }
        else
        {
            Debug.LogWarning("FlashlightTool2D reference is missing on the BatteryRefill script!");
        }
    }
}
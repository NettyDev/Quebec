using UnityEngine;
using UnityEngine.UI; // Required for UI elements

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))] // Automatically adds AudioSource component
public class FlashlightTool2D : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;

    [Header("Sprite Settings")]
    [Tooltip("The Image component displaying the flashlight hand on screen")]
    public Image flashlightImage;
    [Tooltip("Sprite to show when the flashlight is turned ON")]
    public Sprite flashlightOnSprite;
    [Tooltip("Sprite to show when the flashlight is turned OFF")]
    public Sprite flashlightOffSprite;

    [Header("Flashlight Settings")]
    [Tooltip("Spotlight component attached to the camera or player")]
    public Light spotlight;
    [Tooltip("Sound played when turning on")]
    public AudioClip turnOnSound;
    [Tooltip("Sound played when turning off")]
    public AudioClip turnOffSound;

    [Header("Battery Settings")]
    [Tooltip("Current battery level (0-100)")]
    public float batteryLevel = 100f;
    [Tooltip("Battery drain rate per second")]
    public float drainRate = 5f;

    [Header("UI References")]
    [Tooltip("Parent container for the battery icon and text")]
    public GameObject batteryUIContainer;
    [Tooltip("Text element to display battery percentage")]
    public Text batteryText;

    private bool isLightOn = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // Ensure the light is off by default when starting
        if (spotlight != null)
        {
            spotlight.enabled = false;
        }

        // Ensure the battery UI is hidden at start
        ShowBatteryUI(false);
    }

    // Method called by InventoryManager
    public void SetToolActive(bool isActive)
    {
        // Enable or disable this specific GameObject in Unity
        gameObject.SetActive(isActive);

        if (isActive)
        {
            Debug.Log("Informacja: Latarka została aktywowana (Slot wybrany).");
            
            // Set the correct sprite when equipping (usually OFF unless we changed logic)
            if (flashlightImage != null && flashlightOffSprite != null)
            {
                flashlightImage.sprite = isLightOn ? flashlightOnSprite : flashlightOffSprite;
            }

            // Show battery UI only if the light is currently ON
            ShowBatteryUI(isLightOn);
            UpdateBatteryUI(); 
        }
        else
        {
            Debug.Log("Informacja: Latarka została zdezaktywowana (Zmieniono Slot).");
            
            // Turn off the light automatically when put away
            if (isLightOn)
            {
                TurnOffLight();
            }
            
            // Forcefully hide battery UI when switching to another slot
            ShowBatteryUI(false);
        }
    }

    void Update()
    {
        // Handle turning the flashlight on/off with Left Mouse Button
        if (Input.GetMouseButtonDown(0))
        {
            ToggleFlashlight();
        }

        // Handle battery drain
        if (isLightOn)
        {
            batteryLevel -= drainRate * Time.deltaTime;
            batteryLevel = Mathf.Clamp(batteryLevel, 0f, 100f);

            UpdateBatteryUI();

            // Force turn off if battery is dead
            if (batteryLevel <= 0f)
            {
                TurnOffLight();
            }
        }
    }

    private void ToggleFlashlight()
    {
        if (isLightOn)
        {
            TurnOffLight();
        }
        else
        {
            if (batteryLevel > 0f)
            {
                TurnOnLight();
            }
            else
            {
                Debug.Log("Bateria rozładowana! Nie można włączyć latarki.");
            }
        }
    }

    private void TurnOnLight()
    {
        isLightOn = true;
        if (spotlight != null) spotlight.enabled = true;
        if (audioSource != null && turnOnSound != null) audioSource.PlayOneShot(turnOnSound);
        
        // Change sprite to ON state
        if (flashlightImage != null && flashlightOnSprite != null)
        {
            flashlightImage.sprite = flashlightOnSprite;
        }

        // Show battery UI
        ShowBatteryUI(true);
    }

    private void TurnOffLight()
    {
        isLightOn = false;
        if (spotlight != null) spotlight.enabled = false;
        if (audioSource != null && turnOffSound != null) audioSource.PlayOneShot(turnOffSound);
        
        // Change sprite to OFF state
        if (flashlightImage != null && flashlightOffSprite != null)
        {
            flashlightImage.sprite = flashlightOffSprite;
        }

        // Hide battery UI
        ShowBatteryUI(false);
    }

    private void UpdateBatteryUI()
    {
        if (batteryText != null)
        {
            // Format to integer and add % sign
            batteryText.text = Mathf.CeilToInt(batteryLevel).ToString() + "%";
        }
    }

    // Helper method to safely toggle all battery-related UI elements
    private void ShowBatteryUI(bool show)
    {
        if (batteryUIContainer != null)
        {
            batteryUIContainer.SetActive(show);
        }
        
        // Explicitly toggle the text component's GameObject just in case it's not a child of the container
        if (batteryText != null)
        {
            batteryText.gameObject.SetActive(show);
        }
    }
}
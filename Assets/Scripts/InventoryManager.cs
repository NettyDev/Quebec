using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory State")]
    public bool hasLidar = false;
    public bool hasFlashlight = false;

    [Header("UI References")]
    public Image[] slotHighlights; 
    public Image[] slotIcons;      
    public Sprite lidarSprite;     
    public Sprite flashlightSprite; 

    [Tooltip("Assign UI elements with key icons (1, 2, 3)")]
    public Image[] keyPromptIcons; // Array for key prompt icons

    [Tooltip("Assign UI Text element to display the active tool name")]
    public Text toolNameText; // Text element for displaying item name

    [Header("Animation Settings")]
    [Tooltip("Assign the RectTransforms of Slot_1, Slot_2, Slot_3")]
    public RectTransform[] slotRects;
    [Tooltip("How many pixels to the right the active slot should move")]
    public float activeOffsetX = 30f;
    [Tooltip("Speed of the movement animation")]
    public float animationSpeed = 10f;

    [Header("2D Player Hands")]
    [Tooltip("Assign the object representing the EMPTY HAND")]
    public GameObject emptyHand2D;
    
    [Tooltip("Assign the Lidar object with the LidarTool2D script attached")]
    public LidarTool2D lidarTool;       
    
    [Tooltip("Assign the Flashlight object with the FlashlightTool2D script attached")]
    public FlashlightTool2D flashlightTool;  

    private int currentSlotIndex = 0;
    
    // Animation internal variables
    private float[] targetOffsets;
    private float[] currentOffsets;
    private Vector2[] basePositions;

    void Start()
    {
        // Initialize animation arrays
        if (slotRects != null && slotRects.Length > 0)
        {
            targetOffsets = new float[slotRects.Length];
            currentOffsets = new float[slotRects.Length];
            basePositions = new Vector2[slotRects.Length];

            for (int i = 0; i < slotRects.Length; i++)
            {
                if (slotRects[i] != null)
                {
                    basePositions[i] = slotRects[i].anchoredPosition;
                }
            }
        }

        UpdateInventoryUI();
        SelectSlot(0); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);

        // Handle smooth slot movement animation
        if (slotRects != null)
        {
            for (int i = 0; i < slotRects.Length; i++)
            {
                if (slotRects[i] != null)
                {
                    // Lerp towards the target offset
                    currentOffsets[i] = Mathf.Lerp(currentOffsets[i], targetOffsets[i], Time.deltaTime * animationSpeed);
                    
                    // Apply offset to the original base position
                    slotRects[i].anchoredPosition = new Vector2(basePositions[i].x + currentOffsets[i], slotRects[i].anchoredPosition.y);
                }
            }
        }
    }

    private void SelectSlot(int index)
    {
        currentSlotIndex = index;

        // Set target offsets for the animation
        if (targetOffsets != null)
        {
            for (int i = 0; i < targetOffsets.Length; i++)
            {
                targetOffsets[i] = (i == currentSlotIndex) ? activeOffsetX : 0f;
            }
        }

        // Update background highlights
        for (int i = 0; i < slotHighlights.Length; i++)
        {
            if (slotHighlights[i] != null)
            {
                slotHighlights[i].enabled = (i == currentSlotIndex);
            }
        }

        // Update key prompt icons opacity (40% when active, 100% when inactive)
        for (int i = 0; i < keyPromptIcons.Length; i++)
        {
            if (keyPromptIcons[i] != null)
            {
                Color keyColor = keyPromptIcons[i].color;
                keyColor.a = (i == currentSlotIndex) ? 0.4f : 1f; 
                keyPromptIcons[i].color = keyColor;
            }
        }

        // Update item icons opacity (100% when active, 50% when inactive)
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (slotIcons[i] != null)
            {
                Color iconColor = slotIcons[i].color;
                iconColor.a = (i == currentSlotIndex) ? 1f : 0.5f; 
                slotIcons[i].color = iconColor;
            }
        }

        ActivateEquippedItem();
    }

    private void ActivateEquippedItem()
    {
        // Hide all 2D hands first
        if (emptyHand2D != null) emptyHand2D.SetActive(false);
        if (lidarTool != null) lidarTool.SetToolActive(false);
        if (flashlightTool != null) flashlightTool.SetToolActive(false);

        // Clear the tool name text by default
        if (toolNameText != null) toolNameText.text = "";

        // Logic for Slot 1 (Lidar)
        if (currentSlotIndex == 0)
        {
            if (hasLidar)
            {
                if (lidarTool != null) lidarTool.SetToolActive(true);
                if (toolNameText != null) toolNameText.text = "Lidar"; // Display name
            }
            else
            {
                if (emptyHand2D != null) emptyHand2D.SetActive(true);
            }
        }
        // Logic for Slot 2 (Flashlight)
        else if (currentSlotIndex == 1)
        {
            if (hasFlashlight)
            {
                if (flashlightTool != null) flashlightTool.SetToolActive(true);
                if (toolNameText != null) toolNameText.text = "Latarka"; // Display name
            }
            else
            {
                if (emptyHand2D != null) emptyHand2D.SetActive(true);
            }
        }
        // Logic for Slot 3 (Empty Hand)
        else if (currentSlotIndex == 2)
        {
            if (emptyHand2D != null) emptyHand2D.SetActive(true);
        }
    }

    public void UpdateInventoryUI()
    {
        // Update Slot 1
        if (slotIcons.Length > 0 && slotIcons[0] != null)
        {
            slotIcons[0].sprite = lidarSprite;
            slotIcons[0].enabled = hasLidar;
        }

        // Update Slot 2
        if (slotIcons.Length > 1 && slotIcons[1] != null)
        {
            slotIcons[1].sprite = flashlightSprite;
            slotIcons[1].enabled = hasFlashlight;
        }

        ActivateEquippedItem(); 
    }
}
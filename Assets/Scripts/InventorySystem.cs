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

    [Header("2D Player Hands")]
    [Tooltip("Przypisz obiekt z komponentem SpriteRenderer/Image i Animatorem dla Lidaru")]
    public GameObject lidarHand2D;       
    [Tooltip("Przypisz obiekt z komponentem SpriteRenderer/Image i Animatorem dla Latarki")]
    public GameObject flashlightHand2D;  

    private int currentSlotIndex = 0;

    void Start()
    {
        UpdateInventoryUI();
        SelectSlot(0); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
    }

    private void SelectSlot(int index)
    {
        currentSlotIndex = index;

        for (int i = 0; i < slotHighlights.Length; i++)
        {
            if (slotHighlights[i] != null)
            {
                slotHighlights[i].enabled = (i == currentSlotIndex);
            }
        }

        ActivateEquippedItem();
    }

    private void ActivateEquippedItem()
    {
        // Ukrywamy wszystkie dłonie 2D
        if (lidarHand2D != null) lidarHand2D.SetActive(false);
        if (flashlightHand2D != null) flashlightHand2D.SetActive(false);

        // Slot 1 (Lidar)
        if (currentSlotIndex == 0 && hasLidar)
        {
            if (lidarHand2D != null) lidarHand2D.SetActive(true);
        }
        // Slot 2 (Latarka)
        else if (currentSlotIndex == 1 && hasFlashlight)
        {
            if (flashlightHand2D != null) flashlightHand2D.SetActive(true);
        }
    }

    public void UpdateInventoryUI()
    {
        if (slotIcons.Length > 0 && slotIcons[0] != null)
        {
            slotIcons[0].sprite = lidarSprite;
            slotIcons[0].enabled = hasLidar;
        }

        if (slotIcons.Length > 1 && slotIcons[1] != null)
        {
            slotIcons[1].sprite = flashlightSprite;
            slotIcons[1].enabled = hasFlashlight;
        }

        ActivateEquippedItem();
    }
}
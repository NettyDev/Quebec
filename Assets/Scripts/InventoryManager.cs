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

    [Tooltip("Przypisz obiekty UI z ikonami klawiszy (1, 2, 3)")]
    public Image[] keyPromptIcons; // Nowa tablica dla ikon klawiszy

    [Header("2D Player Hands")]
    [Tooltip("Przypisz obiekt reprezentujący PUSTĄ RĘKĘ")]
    public GameObject emptyHand2D;
    
    [Tooltip("Przypisz obiekt Lidaru z podpiętym skryptem LidarTool2D")]
    public LidarTool2D lidarTool;       
    
    [Tooltip("Przypisz obiekt Latarki z podpiętym skryptem FlashlightTool2D")]
    public FlashlightTool2D flashlightTool;  

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

        // Aktualizacja podświetlenia tła
        for (int i = 0; i < slotHighlights.Length; i++)
        {
            if (slotHighlights[i] != null)
            {
                slotHighlights[i].enabled = (i == currentSlotIndex);
            }
        }

        // Aktualizacja przezroczystości ikon klawiszy
        for (int i = 0; i < keyPromptIcons.Length; i++)
        {
            if (keyPromptIcons[i] != null)
            {
                // Pobieramy obecny kolor ikony
                Color keyColor = keyPromptIcons[i].color;
                
                // Zmieniamy kanał Alpha (a): 0.2f (20%) dla aktywnego slota, 1f (100%) dla nieaktywnych
                keyColor.a = (i == currentSlotIndex) ? 0.4f : 1f; 
                
                // Przypisujemy zmieniony kolor z powrotem do komponentu
                keyPromptIcons[i].color = keyColor;
            }
        }

        ActivateEquippedItem();
    }

    private void ActivateEquippedItem()
    {
        if (emptyHand2D != null) emptyHand2D.SetActive(false);
        if (lidarTool != null) lidarTool.SetToolActive(false);
        if (flashlightTool != null) flashlightTool.SetToolActive(false);

        if (currentSlotIndex == 0)
        {
            if (hasLidar)
            {
                if (lidarTool != null) lidarTool.SetToolActive(true);
            }
            else
            {
                if (emptyHand2D != null) emptyHand2D.SetActive(true);
            }
        }
        else if (currentSlotIndex == 1)
        {
            if (hasFlashlight)
            {
                if (flashlightTool != null) flashlightTool.SetToolActive(true);
            }
            else
            {
                if (emptyHand2D != null) emptyHand2D.SetActive(true);
            }
        }
        else if (currentSlotIndex == 2)
        {
            if (emptyHand2D != null) emptyHand2D.SetActive(true);
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
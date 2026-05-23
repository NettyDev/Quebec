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
    [Tooltip("Przypisz obiekt 2D reprezentujący PUSTĄ RĘKĘ")]
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

        // Aktualizacja podświetlenia (Highlight) - widoczne tylko dla aktywnego slota
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
        // 1. Zawsze na starcie ukrywamy pustą rękę i dezaktywujemy narzędzia
        if (emptyHand2D != null) emptyHand2D.SetActive(false);
        if (lidarTool != null) lidarTool.SetToolActive(false);
        if (flashlightTool != null) flashlightTool.SetToolActive(false);

        // 2. Slot 1 (Klawisz '1')
        if (currentSlotIndex == 0)
        {
            if (hasLidar)
            {
                // Przekazujemy informację do skryptu, że Lidar jest aktywny
                if (lidarTool != null) lidarTool.SetToolActive(true);
            }
            else
            {
                // Brak Lidaru = pokazujemy pustą rękę
                if (emptyHand2D != null) emptyHand2D.SetActive(true);
            }
        }
        // 3. Slot 2 (Klawisz '2')
        else if (currentSlotIndex == 1)
        {
            if (hasFlashlight)
            {
                // Przekazujemy informację do skryptu, że Latarka jest aktywna
                if (flashlightTool != null) flashlightTool.SetToolActive(true);
            }
            else
            {
                // Brak Latarki = pokazujemy pustą rękę
                if (emptyHand2D != null) emptyHand2D.SetActive(true);
            }
        }
        // 4. Slot 3 (Klawisz '3')
        else if (currentSlotIndex == 2)
        {
            // Slot 3 nigdy nie ma przypisanego przedmiotu = zawsze pokazujemy pustą rękę
            if (emptyHand2D != null) emptyHand2D.SetActive(true);
        }
    }

    // Ta metoda zarządza WYŁĄCZNIE ikonami w ekwipunku, zgodnie z posiadanymi zmiennymi
    public void UpdateInventoryUI()
    {
        // Slot 1: Lidar Icon
        if (slotIcons.Length > 0 && slotIcons[0] != null)
        {
            slotIcons[0].sprite = lidarSprite;
            slotIcons[0].enabled = hasLidar; // Ikona widoczna zawsze, jeśli hasLidar = true
        }

        // Slot 2: Flashlight Icon
        if (slotIcons.Length > 1 && slotIcons[1] != null)
        {
            slotIcons[1].sprite = flashlightSprite;
            slotIcons[1].enabled = hasFlashlight; // Ikona widoczna zawsze, jeśli hasFlashlight = true
        }

        ActivateEquippedItem(); // Odświeżamy stan w rękach w razie podniesienia przedmiotu
    }
}
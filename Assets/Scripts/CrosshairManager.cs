using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [Header("Elementy UI")]
    [SerializeField] private Image crosshairImage;

    [Header("Ustawienia Celownika")]
    [Tooltip("Domyślny sprite celownika")]
    [SerializeField] private Sprite defaultCursorSprite; 
    
    [Tooltip("Sprite celownika pojawiający się przy najechaniu na obiekt interaktywny")]
    [SerializeField] private Sprite interactCursorSprite; 
    
    [SerializeField] private Vector2 crosshairSize = new Vector2(16f, 16f);
    [SerializeField] private Color crosshairColor = Color.white;

    void Start()
    {
        // Upewnij się, że systemowy kursor jest zablokowany i ukryty dla widoku z pierwszej osoby
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshairImage == null)
        {
            crosshairImage = GetComponent<Image>();
        }

        SetupCrosshair();
    }

    private void SetupCrosshair()
    {
        if (crosshairImage == null) return;

        SetDefaultCrosshair();

        crosshairImage.color = crosshairColor;

        // Wymuś idealne wyśrodkowanie RectTransform z określoną wielkością
        RectTransform rectTransform = crosshairImage.rectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = crosshairSize;
    }

    // Publiczna metoda wywoływana, gdy gracz nie patrzy na żaden interaktywny obiekt
    public void SetDefaultCrosshair()
    {
        if (crosshairImage != null && defaultCursorSprite != null)
        {
            crosshairImage.sprite = defaultCursorSprite;
        }
    }

    // Publiczna metoda wywoływana, gdy gracz patrzy na interaktywny obiekt
    public void SetInteractCrosshair()
    {
        if (crosshairImage != null && interactCursorSprite != null)
        {
            crosshairImage.sprite = interactCursorSprite;
        }
    }
}
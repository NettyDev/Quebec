using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image crosshairImage;

    [Header("Crosshair Settings")]
    [SerializeField] private Sprite customCursorSprite;
    [SerializeField] private Vector2 crosshairSize = new Vector2(16f, 16f);
    [SerializeField] private Color crosshairColor = Color.white;

    void Start()
    {
        // Ensure the hardware cursor is locked and hidden for the first-person control
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

        // Assign the sprite if provided, otherwise it stays as the default UI square
        if (customCursorSprite != null)
        {
            crosshairImage.sprite = customCursorSprite;
        }

        crosshairImage.color = crosshairColor;

        // Force the RectTransform to be perfectly centered with a defined size
        RectTransform rectTransform = crosshairImage.rectTransform;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = crosshairSize;
    }
}
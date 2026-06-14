using UnityEngine;
using UnityEngine.UI;
using TMPro; // Wymagane dla TextMeshPro

public class PlayerInteractor : MonoBehaviour
{
    [Header("Ustawienia Interakcji")]
    [Tooltip("Maksymalny dystans interakcji z obiektami (np. 3 metry)")]
    public float interactRange = 3f;
    
    [Tooltip("Maska warstwy (Layer mask) dla interaktywnych obiektów, optymalizuje wydajność fizyki")]
    public LayerMask interactableLayer;

    [Header("Ustawienia Lewitującego UI")]
    [Tooltip("Kontener UI lub Canvas w przestrzeni świata (World Space)")]
    public GameObject floatingUI;
    
    [Tooltip("Komponent TextMeshPro wyświetlający nazwę akcji")]
    public TextMeshProUGUI floatingText; 
    
    [Tooltip("Pionowe przesunięcie UI nad obiektem")]
    public float uiHeightOffset = 0.5f;

    [Header("Zarządzanie Celownikiem")]
    [Tooltip("Referencja do menedżera celownika, aby zmieniać kursor podczas interakcji")]
    public CrosshairManager crosshairManager;

    // Przechowuje obiekt, na który gracz aktualnie patrzy
    private InteractableItem currentTarget;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        
        // Ukryj UI na początku gry
        if (floatingUI != null)
        {
            floatingUI.SetActive(false);
        }
    }

    void Update()
    {
        // Wypuść promień (raycast) ze środka kamery do przodu
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Sprawdź, czy promień uderza w obiekt na określonej warstwie w zasięgu
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            InteractableItem interactable = hit.collider.GetComponent<InteractableItem>();

            if (interactable != null)
            {
                // Jeśli patrzymy na nowy obiekt, usuń podświetlenie ze starego i podświetl nowy
                if (interactable != currentTarget)
                {
                    if (currentTarget != null) currentTarget.Unhighlight();
                    
                    currentTarget = interactable;
                    currentTarget.Highlight();

                    // Zmień kursor na wersję interaktywną
                    if (crosshairManager != null)
                    {
                        crosshairManager.SetInteractCrosshair();
                    }

                    // Zaktualizuj tekst, aby pasował do konkretnego przedmiotu
                    if (floatingText != null)
                    {
                        floatingText.text = currentTarget.interactText;
                    }
                }

                // Zawsze aktualizuj pozycję i rotację UI podczas patrzenia na przedmiot
                UpdateFloatingUI();

                // Sprawdź, czy gracz naciska klawisz interakcji (F)
                if (Input.GetKeyDown(KeyCode.F))
                {
                    currentTarget.Interact();
                    ClearTarget(); // Wyczyść cel, ponieważ zostanie on zniszczony
                }
            }
            else
            {
                ClearTarget();
            }
        }
        else
        {
            ClearTarget();
        }
    }

    // Obsługuje umieszczanie UI nad przedmiotem i obracanie go w stronę gracza
    private void UpdateFloatingUI()
    {
        if (floatingUI != null && currentTarget != null)
        {
            floatingUI.SetActive(true);
            
            // Umieść UI nad przedmiotem
            floatingUI.transform.position = currentTarget.transform.position + Vector3.up * uiHeightOffset;

            // Efekt Billboarding: dopasuj rotację kamery, aby UI zawsze było idealnie skierowane do gracza
            floatingUI.transform.rotation = mainCam.transform.rotation;
        }
    }

    // Metoda pomocnicza do bezpiecznego usuwania podświetlenia, ukrywania UI i resetowania kursora
    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.Unhighlight();
            currentTarget = null;
        }

        // Ukryj UI, gdy nie patrzysz na nic interaktywnego
        if (floatingUI != null)
        {
            floatingUI.SetActive(false);
        }

        // Przywróć domyślny kursor
        if (crosshairManager != null)
        {
            crosshairManager.SetDefaultCrosshair();
        }
    }
}
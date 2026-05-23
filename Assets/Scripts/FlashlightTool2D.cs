using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FlashlightTool2D : MonoBehaviour
{
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Ta metoda jest wywoływana z InventoryManager
    public void SetToolActive(bool isActive)
    {
        // Włączamy lub wyłączamy ten konkretny obiekt w Unity
        gameObject.SetActive(isActive);

        if (isActive)
        {
            Debug.Log("Informacja: Latarka została aktywowana (Slot wybrany).");
            // anim.SetTrigger("Equip");
        }
        else
        {
            Debug.Log("Informacja: Latarka została zdezaktywowana (Zmieniono Slot).");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ToggleFlashlight();
        }
    }

    private void ToggleFlashlight()
    {
        Debug.Log("Latarka w użyciu: Przełączenie światła...");
        // anim.SetTrigger("ToggleLight");
    }
}
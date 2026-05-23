using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LidarTool2D : MonoBehaviour
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
            Debug.Log("Informacja: Lidar został aktywowany (Slot wybrany).");
            // Możesz tu odpalić animację wyciągania: anim.SetTrigger("Equip");
        }
        else
        {
            Debug.Log("Informacja: Lidar został zdezaktywowany (Zmieniono Slot).");
            // Możesz tu zresetować stan działania narzędzia
        }
    }

    void Update()
    {
        // Logika skanowania działa tylko, gdy obiekt jest aktywny (gameObject.SetActive(true))
        if (Input.GetMouseButtonDown(0))
        {
            UseLidar();
        }
    }

    private void UseLidar()
    {
        Debug.Log("Lidar w użyciu: Skanowanie...");
        // anim.SetTrigger("Use");
    }
}
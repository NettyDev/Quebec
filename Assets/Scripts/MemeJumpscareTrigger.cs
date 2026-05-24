using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class MemeJumpscareTrigger : MonoBehaviour
{
    [Header("Ustawienia Jumpscare'a")]
    [Tooltip("Przeciągnij tutaj obiekt ScareImage z Twojego Canvasa")]
    public Image scareImage;
    
    [Tooltip("Wybierz plik dźwiękowy, który ma się odtworzyć")]
    public AudioClip scareSound;

    [Header("Czas trwania")]
    [Tooltip("Ile sekund obrazek ma być w pełni widoczny?")]
    public float displayTime = 2f;
    [Tooltip("Ile sekund ma trwać płynne znikanie?")]
    public float fadeOutTime = 1f;

    // Zabezpieczenie, aby wyzwolić tylko raz
    private bool hasTriggered = false;
    private AudioSource audioSource;

    void Start()
    {
        // Pobieramy AudioSource przypisane do tego samego obiektu
        audioSource = GetComponent<AudioSource>();
        // Upewniamy się, że dźwięk nie odtworzy się sam przy starcie gry
        audioSource.playOnAwake = false;

        // Upewniamy się, że obrazek na start jest przezroczysty
        if (scareImage != null)
        {
            Color startColor = scareImage.color;
            startColor.a = 0f;
            scareImage.color = startColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Sprawdzamy, czy w trigger wszedł gracz i czy event jeszcze się nie odbył
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true; // Blokujemy ponowne wyzwolenie
            StartCoroutine(PlayScareSequence());
        }
    }

    private IEnumerator PlayScareSequence()
    {
        if (scareImage == null)
        {
            Debug.LogWarning("Brak przypisanego obrazka (ScareImage)!");
            yield break;
        }

        // 1. Odtwórz dźwięk (jeśli jest przypisany)
        if (scareSound != null)
        {
            audioSource.PlayOneShot(scareSound);
        }

        // 2. Natychmiastowe pokazanie obrazka (Alpha = 100%)
        Color imgColor = scareImage.color;
        imgColor.a = 1f;
        scareImage.color = imgColor;

        // 3. Czekamy ustaloną ilość czasu (domyślnie 2 sekundy)
        yield return new WaitForSeconds(displayTime);

        // 4. Płynne znikanie przez ustalony czas (domyślnie 1 sekunda)
        float currentTime = 0f;
        while (currentTime < fadeOutTime)
        {
            currentTime += Time.deltaTime;
            // Mathf.Lerp płynnie przechodzi od 1 (w pełni widoczne) do 0 (niewidoczne)
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeOutTime);
            
            imgColor.a = alpha;
            scareImage.color = imgColor;
            
            // Czekamy do następnej klatki, aby kontynuować pętlę
            yield return null; 
        }

        // 5. Upewniamy się, że na koniec obrazek jest całkowicie przezroczysty (Alpha = 0%)
        imgColor.a = 0f;
        scareImage.color = imgColor;

        // Opcjonalnie: możemy zniszczyć ten trigger po użyciu, by nie zaśmiecał pamięci
        Destroy(gameObject);
    }
}
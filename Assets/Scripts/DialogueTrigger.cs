using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Content")]
    public List<DialogueLine> sequence;

    [Header("Trigger Settings")]
    public bool playOnlyOnce = true;
    private bool hasPlayed = false;

    // Metoda do wywoływania z zewnątrz (np. OnClick, system interakcji)
    public void TriggerDialogue()
    {
        if (playOnlyOnce && hasPlayed) return;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(sequence);
            
            if (playOnlyOnce)
            {
                hasPlayed = true;
            }
        }
    }

    // Opcjonalne wywołanie przez fizyczną kolizję ze strefą
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialogue();
        }
    }
}
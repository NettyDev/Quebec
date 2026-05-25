using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI References")]
    public TextMeshProUGUI subtitleTextUI;
    public GameObject subtitlePanel; 
    
    [Header("Settings")]
    public float defaultDelayIfNoAudio = 3f;

    private AudioSource audioSource;
    private Coroutine currentDialogueCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        if (subtitleTextUI != null) subtitleTextUI.text = "";
    }

    public void StartDialogue(List<DialogueLine> lines)
    {
        if (currentDialogueCoroutine != null)
        {
            StopCoroutine(currentDialogueCoroutine);
        }
        currentDialogueCoroutine = StartCoroutine(PlayDialogueSequence(lines));
    }

    private IEnumerator PlayDialogueSequence(List<DialogueLine> lines)
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(true);

        foreach (DialogueLine line in lines)
        {
            if (line.fragments == null || line.fragments.Count == 0) continue;

            if (line.audioClip != null)
            {
                audioSource.clip = line.audioClip;
                audioSource.Play();

                int currentFragmentIndex = 0;

                while (audioSource.isPlaying)
                {
                    float currentTime = audioSource.time;

                    if (currentFragmentIndex < line.fragments.Count && currentTime >= line.fragments[currentFragmentIndex].showTime)
                    {
                        subtitleTextUI.text = line.fragments[currentFragmentIndex].text;
                        currentFragmentIndex++;
                    }
                    yield return null;
                }
            }
            else
            {
                foreach (var fragment in line.fragments)
                {
                    subtitleTextUI.text = fragment.text;
                    yield return new WaitForSeconds(defaultDelayIfNoAudio / line.fragments.Count);
                }
            }
        }

        EndDialogue();
    }

    private void EndDialogue()
    {
        if (subtitleTextUI != null) subtitleTextUI.text = "";
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
        audioSource.Stop();
        currentDialogueCoroutine = null;
    }
}
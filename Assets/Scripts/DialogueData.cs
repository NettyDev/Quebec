using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubtitleFragment
{
    [TextArea(2, 5)]
    public string text;
    [Tooltip("Czas w sekundach od początku audio, kiedy ma pojawić się tekst.")]
    public float showTime; 
}

[System.Serializable]
public class DialogueLine
{
    public AudioClip audioClip;
    public List<SubtitleFragment> fragments;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;    
    public AudioSource[] audioSources;

    // Awake is called when the script instance is being loaded.
    protected void Awake() {
        instance = this;
    }

    public void PlayAudio(int index)
    {
        if (index >= 0 && index < audioSources.Length)
        {
            audioSources[index].Play();
        }
        else
        {
            Debug.LogWarning("Índice de AudioSource fuera de rango.");
        }
    }
}

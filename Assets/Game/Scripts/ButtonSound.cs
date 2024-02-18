using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    // Referencia al objeto AudioSource que reproducirá el sonido
    public AudioSource audioSource;
    // Sonido a reproducir cuando se presione el botón
    public AudioClip buttonSound;

    void Start()
    {
        // Obtener el componente AudioSource si no se ha asignado
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayButtonSound()
    {
        // Reproducir el sonido al presionar el botón
        audioSource.PlayOneShot(buttonSound);
    }
}

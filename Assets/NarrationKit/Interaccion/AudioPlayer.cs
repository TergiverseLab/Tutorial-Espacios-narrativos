using UnityEngine;

/// <summary>
/// Reproductor de audio simple. Permite reproducir sonidos sin interrumpirse entre sí.
/// Necesita un AudioSource en el mismo objeto.
///
/// Configuración:
/// 1. Añade un AudioSource al objeto del jugador
/// 2. Añade este script al mismo objeto
///
/// Otros scripts llaman a audioPlayer.PlayAudio(clip) para reproducir sonidos.
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Reproduce un clip de audio. Se pueden superponer varios sonidos.
    /// </summary>
    public void PlayAudio(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}

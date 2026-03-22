using UnityEngine;
using System.Collections;

/// <summary>
/// Zona de audio ambiental: cuando el jugador entra, el sonido aparece gradualmente (fade in).
/// Cuando sale, desaparece gradualmente (fade out).
///
/// Configuración:
/// 1. Crea un objeto vacío
/// 2. Añade un Collider (Box, Sphere...) y marca "Is Trigger". Hazlo grande para cubrir la zona.
/// 3. Añade un AudioSource con tu clip de audio. Desactiva "Play On Awake".
/// 4. Añade este script
/// 5. Arrastra el AudioSource al campo correspondiente
/// 6. Asegúrate de que el jugador tiene el tag "Player"
/// </summary>
public class AudioZoneManager : MonoBehaviour
{
    [Tooltip("Arrastra aquí el AudioSource de este objeto")]
    public AudioSource audioSource;

    [Tooltip("Duración del fade in/out en segundos")]
    public float fadeDuration = 2.0f;

    private float maxVolume;
    private Coroutine fadeActual;

    private void Start()
    {
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            maxVolume = audioSource.volume;
            audioSource.volume = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && audioSource != null)
        {
            // Cancelar fade anterior si existe
            if (fadeActual != null)
                StopCoroutine(fadeActual);

            audioSource.Play();
            fadeActual = StartCoroutine(Fade(0f, maxVolume));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && audioSource != null)
        {
            if (fadeActual != null)
                StopCoroutine(fadeActual);

            fadeActual = StartCoroutine(Fade(audioSource.volume, 0f));
        }
    }

    /// <summary>
    /// Transición gradual de volumen entre dos valores.
    /// </summary>
    private IEnumerator Fade(float from, float to)
    {
        float timer = 0f;
        audioSource.volume = from;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(from, to, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = to;

        // Si el volumen llega a 0, parar el audio para no gastar recursos
        if (to <= 0f)
            audioSource.Stop();

        fadeActual = null;
    }
}

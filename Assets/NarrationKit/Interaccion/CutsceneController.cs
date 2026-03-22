using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Controlador de cutscenes con subtítulos temporizados.
/// Usa el sistema Timeline de Unity para las animaciones/cámara
/// y muestra textos en momentos específicos.
///
/// Configuración:
/// 1. Crea una Timeline (Window → Sequencing → Timeline)
/// 2. Añade un PlayableDirector al objeto de la cutscene
/// 3. Añade este script al mismo objeto
/// 4. Configura los textos con sus tiempos de aparición
///
/// Para un walking sim simple puedes usarlo como intro o final.
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>

[System.Serializable]
public class Texts
{
    [TextArea]
    [Tooltip("Texto del subtítulo")]
    public string text;

    [Tooltip("Segundo en el que aparece este texto")]
    public float startTime;
}

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneController : MonoBehaviour
{
    [Tooltip("¿Reproducir automáticamente al iniciar la escena?")]
    public bool playOnAwake;

    [Tooltip("Subtítulos temporizados de la cutscene")]
    public Texts[] texts;

    private PlayableDirector cutscene;

    private void Awake()
    {
        cutscene = GetComponent<PlayableDirector>();
    }

    void Start()
    {
        if (playOnAwake)
        {
            Invoke("Play", 1f);
        }
    }

    /// <summary>
    /// Reproduce la cutscene y sus subtítulos.
    /// </summary>
    public void Play()
    {
        cutscene.Play();
        Invoke("Finish", (float)cutscene.duration);

        for (int i = 0; i < texts.Length; i++)
        {
            StartCoroutine(Subtitle(texts[i]));
        }
    }

    IEnumerator Subtitle(Texts text)
    {
        yield return new WaitForSeconds(text.startTime);
        UIManager.instance.SetCaptions(text.text);
    }

    void Finish()
    {
        UIManager.instance.SetCaptions("");
    }
}

using UnityEngine;
using TMPro;

/// <summary>
/// Gestiona la visualización de subtítulos en pantalla.
/// Arrastra un objeto TextMeshPro (UI) al campo subtitleText en el Inspector.
/// Otros scripts (como SubtitleTrigger) llaman a StartSubtitles() para lanzar una secuencia.
/// </summary>

[System.Serializable]
public class SubtitleData
{
    [TextArea] public string subtitle;  // Texto del subtítulo
    public float duration = 3f;         // Duración en segundos
}

public class SubtitleManager : MonoBehaviour
{
    [Tooltip("Arrastra aquí un elemento TextMeshPro de tu Canvas")]
    public TextMeshProUGUI subtitleText;

    [Tooltip("Duración del efecto de aparición gradual (fade in)")]
    public float fadeInDuration = 1.0f;

    private SubtitleData[] subtitles;
    private float fadeInTimer;
    private float displayTimer;
    private int currentSubtitleIndex = 0;
    private bool subtitlesStarted = false;
    private bool subtitlesPlaying = false;

    /// <summary>
    /// Otros scripts pueden consultar esta propiedad para saber si hay subtítulos en pantalla.
    /// </summary>
    public bool SubtitlesPlaying
    {
        get { return subtitlesPlaying; }
    }

    void Start()
    {
        if (subtitleText != null)
            subtitleText.text = "";
    }

    void Update()
    {
        if (!subtitlesStarted || subtitles == null || currentSubtitleIndex >= subtitles.Length)
            return;

        fadeInTimer += Time.deltaTime;
        displayTimer += Time.deltaTime;

        // Efecto de fade in mientras se muestra el subtítulo actual
        float alpha = Mathf.Clamp01(fadeInTimer / fadeInDuration);
        subtitleText.color = new Color(subtitleText.color.r, subtitleText.color.g, subtitleText.color.b, alpha);

        // Cuando se agota la duración, pasar al siguiente subtítulo
        if (displayTimer >= subtitles[currentSubtitleIndex].duration)
        {
            currentSubtitleIndex++;

            if (currentSubtitleIndex < subtitles.Length)
            {
                ShowSubtitle(subtitles[currentSubtitleIndex]);
            }
            else
            {
                // Se acabaron los subtítulos
                subtitleText.text = "";
                subtitlesStarted = false;
                subtitlesPlaying = false;
            }

            displayTimer = 0f;
            fadeInTimer = 0f;
        }
    }

    /// <summary>
    /// Inicia una secuencia de subtítulos. Llamado por SubtitleTrigger u otros scripts.
    /// </summary>
    public void StartSubtitles(SubtitleData[] newSubtitles)
    {
        if (newSubtitles != null && newSubtitles.Length > 0)
        {
            subtitles = newSubtitles;
            currentSubtitleIndex = 0;
            subtitlesStarted = true;
            subtitlesPlaying = true;
            fadeInTimer = 0f;
            displayTimer = 0f;
            ShowSubtitle(subtitles[0]);
        }
    }

    private void ShowSubtitle(SubtitleData subtitleData)
    {
        subtitleText.text = subtitleData.subtitle;
        // Empezar invisible para que el fade in funcione
        subtitleText.color = new Color(subtitleText.color.r, subtitleText.color.g, subtitleText.color.b, 0f);
    }
}

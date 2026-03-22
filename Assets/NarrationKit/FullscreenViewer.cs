using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// Visor a pantalla completa: muestra imágenes, textos largos o videos
/// cuando el jugador interactúa con un objeto.
///
/// Configuración:
/// 1. Crea un Canvas con un Panel oscuro semitransparente (el fondo)
/// 2. Dentro del Panel, pon:
///    - Un Image (para mostrar imágenes/documentos)
///    - Un TextMeshPro (para textos largos)
///    - Un RawImage (para videos)
///    - Un TextMeshPro pequeño con "Click derecho para cerrar"
/// 3. Arrastra cada elemento a los campos de este script
/// 4. Este script va en el mismo Canvas o en un objeto gestor
/// 5. Otros scripts llaman a ShowImage(), ShowText() o ShowVideo() para activarlo
///
/// Se integra con el sistema de Interactables: en el UnityEvent OnInteract
/// de un Interactable, llama a FullscreenViewer.ShowImage(), etc.
/// </summary>
public class FullscreenViewer : MonoBehaviour
{
    [Header("Elementos del Canvas")]
    [Tooltip("Panel de fondo (se activa/desactiva)")]
    public GameObject panel;

    [Tooltip("Para mostrar imágenes y documentos")]
    public Image imageDisplay;

    [Tooltip("Para mostrar textos largos")]
    public TextMeshProUGUI textDisplay;

    [Tooltip("Para mostrar videos (RawImage con RenderTexture)")]
    public RawImage videoDisplay;

    [Header("Configuración")]
    [Tooltip("¿Pausar el juego mientras se ve el contenido?")]
    public bool pausarJuego = false;

    private VideoPlayer currentVideoPlayer;
    private bool isActive = false;

    public static FullscreenViewer instance;

    void Awake()
    {
        // Singleton para acceder desde cualquier script
        instance = this;
    }

    void Start()
    {
        CloseViewer();
    }

    void Update()
    {
        // Cerrar con click derecho o Escape
        if (isActive && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            CloseViewer();
        }
    }

    /// <summary>
    /// Muestra una imagen a pantalla completa.
    /// Se puede llamar desde UnityEvents en el Inspector.
    /// </summary>
    public void ShowImage(Sprite sprite)
    {
        OpenViewer();
        if (imageDisplay != null)
        {
            imageDisplay.gameObject.SetActive(true);
            imageDisplay.sprite = sprite;
        }
    }

    /// <summary>
    /// Muestra un texto largo a pantalla completa.
    /// Se puede llamar desde UnityEvents en el Inspector.
    /// </summary>
    public void ShowText(string text)
    {
        OpenViewer();
        if (textDisplay != null)
        {
            textDisplay.gameObject.SetActive(true);
            textDisplay.text = text;
        }
    }

    /// <summary>
    /// Muestra un video a pantalla completa.
    /// Pasa el VideoPlayer del objeto que contiene el video.
    /// </summary>
    public void ShowVideo(VideoPlayer vp)
    {
        OpenViewer();
        if (videoDisplay != null && vp != null && vp.targetTexture != null)
        {
            videoDisplay.gameObject.SetActive(true);
            videoDisplay.texture = vp.targetTexture;
            currentVideoPlayer = vp;
            vp.Play();
        }
    }

    /// <summary>
    /// Cierra el visor y vuelve al juego.
    /// </summary>
    public void CloseViewer()
    {
        isActive = false;

        if (panel != null)
            panel.SetActive(false);

        if (imageDisplay != null)
            imageDisplay.gameObject.SetActive(false);

        if (textDisplay != null)
            textDisplay.gameObject.SetActive(false);

        if (videoDisplay != null)
            videoDisplay.gameObject.SetActive(false);

        // Pausar el video al cerrar
        if (currentVideoPlayer != null && currentVideoPlayer.isPlaying)
            currentVideoPlayer.Pause();

        currentVideoPlayer = null;

        if (pausarJuego)
            Time.timeScale = 1f;
    }

    private void OpenViewer()
    {
        isActive = true;

        // Desactivar todos los displays primero
        if (imageDisplay != null) imageDisplay.gameObject.SetActive(false);
        if (textDisplay != null) textDisplay.gameObject.SetActive(false);
        if (videoDisplay != null) videoDisplay.gameObject.SetActive(false);

        // Activar el panel de fondo
        if (panel != null)
            panel.SetActive(true);

        if (pausarJuego)
            Time.timeScale = 0f;
    }
}

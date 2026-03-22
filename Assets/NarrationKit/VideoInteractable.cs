using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Pantalla de video interactiva: el jugador hace click para reproducir/pausar.
///
/// Configuración:
/// 1. Crea un Plano o Cubo (será la "pantalla")
/// 2. Crea una RenderTexture (Assets → Create → Render Texture)
/// 3. Añade un componente VideoPlayer al plano
/// 4. En VideoPlayer: arrastra tu video a "Video Clip" y la RenderTexture a "Target Texture"
/// 5. Crea un Material, pon la RenderTexture como textura principal (Albedo)
///    - Para que emita luz: activa "Emission" y pon la misma RenderTexture como mapa de emisión
/// 6. Aplica el material al plano
/// 7. Añade este script al plano
/// 8. Añade un Collider al plano (si no lo tiene ya)
/// 9. Asegúrate de que el jugador tiene el tag "Player"
///
/// Controles:
/// - Click izquierdo mirando a la pantalla: play/pausa
/// - Click derecho (si fullscreen está activo): cerrar pantalla completa
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class VideoInteractable : MonoBehaviour
{
    [Header("Configuración de Video")]
    [Tooltip("¿El video empieza reproduciéndose automáticamente?")]
    public bool playOnStart = false;

    [Tooltip("¿El video se repite en bucle?")]
    public bool loop = true;

    [Header("Pantalla Completa (opcional)")]
    [Tooltip("Si se asigna, al hacer click el video se muestra a pantalla completa en este RawImage del Canvas")]
    public UnityEngine.UI.RawImage fullscreenDisplay;

    [Tooltip("Distancia máxima de interacción")]
    public float interactionDistance = 3f;

    [Header("Emisión de Luz (opcional)")]
    [Tooltip("Intensidad de la emisión del material (0 = sin emisión, 2-5 = brillo suave, 10+ = brillo intenso)")]
    public float emissionIntensity = 2f;

    private VideoPlayer videoPlayer;
    private Camera mainCam;
    private bool isFullscreen = false;
    private Material screenMaterial;
    private Color emissionColor = Color.white;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        mainCam = Camera.main;

        // Configurar bucle
        videoPlayer.isLooping = loop;

        // Configurar emisión del material si tiene Renderer
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            // Crear instancia del material para no afectar otros objetos con el mismo material
            screenMaterial = rend.material;
            UpdateEmission();
        }

        // Ocultar pantalla completa al inicio
        if (fullscreenDisplay != null)
        {
            fullscreenDisplay.gameObject.SetActive(false);
            // Asignar la misma RenderTexture al display de pantalla completa
            if (videoPlayer.targetTexture != null)
                fullscreenDisplay.texture = videoPlayer.targetTexture;
        }

        if (playOnStart)
            videoPlayer.Play();
        else
            videoPlayer.Stop();
    }

    void Update()
    {
        // Cerrar pantalla completa con click derecho
        if (isFullscreen && Input.GetMouseButtonDown(1))
        {
            CloseFullscreen();
            return;
        }

        // Detectar click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            // Lanzar raycast desde el centro de la pantalla
            Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    TogglePlayPause();
                }
            }
        }

        // Actualizar emisión según si el video está reproduciéndose
        if (screenMaterial != null && emissionIntensity > 0)
        {
            UpdateEmission();
        }
    }

    /// <summary>
    /// Alterna entre reproducir y pausar el video.
    /// </summary>
    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();

            // Si hay display de pantalla completa, mostrarlo
            if (fullscreenDisplay != null && !isFullscreen)
            {
                OpenFullscreen();
            }
        }
    }

    /// <summary>
    /// Reproduce el video (se puede llamar desde UnityEvents).
    /// </summary>
    public void Play()
    {
        videoPlayer.Play();
    }

    /// <summary>
    /// Pausa el video (se puede llamar desde UnityEvents).
    /// </summary>
    public void Pause()
    {
        videoPlayer.Pause();
    }

    private void OpenFullscreen()
    {
        if (fullscreenDisplay != null)
        {
            fullscreenDisplay.gameObject.SetActive(true);
            isFullscreen = true;
        }
    }

    private void CloseFullscreen()
    {
        if (fullscreenDisplay != null)
        {
            fullscreenDisplay.gameObject.SetActive(false);
            isFullscreen = false;
        }
    }

    private void UpdateEmission()
    {
        if (screenMaterial != null && videoPlayer.isPlaying)
        {
            // La emisión hace que la superficie "brille" e ilumine objetos cercanos
            screenMaterial.EnableKeyword("_EMISSION");
            screenMaterial.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }
    }
}

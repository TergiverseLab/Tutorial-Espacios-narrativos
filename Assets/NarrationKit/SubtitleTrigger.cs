using UnityEngine;

/// <summary>
/// Cuando el jugador entra en este trigger, lanza una secuencia de subtítulos.
///
/// Configuración:
/// 1. Crea un objeto vacío (o usa un cubo/esfera)
/// 2. Añade un Collider y marca "Is Trigger"
/// 3. Añade este script
/// 4. Arrastra el SubtitleManager de la escena al campo correspondiente
/// 5. Escribe tus subtítulos en el array del Inspector
/// 6. Asegúrate de que el jugador tiene el tag "Player"
/// </summary>
public class SubtitleTrigger : MonoBehaviour
{
    [Tooltip("Arrastra aquí el objeto que tiene el SubtitleManager")]
    public SubtitleManager subtitleManager;

    [Tooltip("Los subtítulos que se mostrarán al entrar en esta zona")]
    public SubtitleData[] subtitles;

    [Tooltip("Si está activado, los subtítulos solo se muestran la primera vez")]
    public bool soloUnaVez = false;

    private bool yaActivado = false;

    void OnTriggerEnter(Collider other)
    {
        // Solo reaccionar al jugador, y solo si no hay subtítulos ya en pantalla
        if (other.CompareTag("Player") && !yaActivado && !subtitleManager.SubtitlesPlaying)
        {
            subtitleManager.StartSubtitles(subtitles);

            if (soloUnaVez)
                yaActivado = true;
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cambia de escena después de un tiempo o cuando el jugador entra en un trigger.
///
/// Uso como TEMPORIZADOR (cambio automático):
///   - Activa "autoStart" y ajusta los minutos/segundos
///
/// Uso como TRIGGER (cambio al pisar una zona):
///   - Añade un Collider con "Is Trigger" al mismo objeto
///   - El jugador debe tener el tag "Player"
///
/// IMPORTANTE: Las escenas deben estar añadidas en File → Build Settings → Scenes In Build
/// </summary>
public class SceneSwitchController : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena de destino (como aparece en Build Settings)")]
    [SerializeField]
    private string nextSceneName;

    [Tooltip("Minutos antes de cambiar de escena")]
    public float countdownMinutes = 0;

    [Tooltip("Segundos antes de cambiar de escena")]
    public float countdownSeconds = 30;

    [Tooltip("Si está activado, la cuenta atrás empieza al cargar la escena")]
    public bool autoStart = false;

    private float countdownTime;
    private bool countdownActive = false;

    private void Start()
    {
        if (autoStart)
            StartCountdown();
    }

    private void Update()
    {
        if (!countdownActive)
            return;

        countdownTime -= Time.deltaTime;

        if (countdownTime <= 0)
        {
            countdownActive = false;
            ChangeScene();
        }
    }

    /// <summary>
    /// Inicia la cuenta atrás. Se puede llamar desde otros scripts o desde un trigger.
    /// </summary>
    public void StartCountdown()
    {
        countdownTime = (countdownMinutes * 60) + countdownSeconds;
        countdownActive = true;
    }

    /// <summary>
    /// Cambia de escena inmediatamente.
    /// </summary>
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("SceneSwitchController: No se ha indicado el nombre de la escena de destino.");
        }
    }

    // Si este objeto tiene un Collider con "Is Trigger", cambia de escena al entrar el jugador
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (countdownMinutes > 0 || countdownSeconds > 0)
                StartCountdown();  // Usa el temporizador
            else
                ChangeScene();     // Cambio inmediato
        }
    }
}

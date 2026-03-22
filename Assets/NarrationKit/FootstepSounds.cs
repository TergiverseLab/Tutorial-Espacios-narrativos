using UnityEngine;

/// <summary>
/// Sonidos de pasos del jugador. Reproduce sonidos a intervalos regulares
/// mientras el jugador camina por el suelo.
///
/// Configuración:
/// 1. Añade este script al objeto del jugador (el que tiene CharacterController)
/// 2. Añade un AudioSource al mismo objeto (Play On Awake = desactivado)
/// 3. Busca sonidos de pasos gratuitos (freesound.org, sonniss.com)
/// 4. Arrastra los clips al array "Default Footsteps" en el Inspector
/// 5. Dale Play y camina
///
/// Superficies diferentes (opcional):
/// Si quieres que suene diferente al pisar madera, hierba, metal, etc:
/// 1. Etiqueta (Tag) los suelos: "Wood", "Grass", "Metal", etc.
/// 2. Añade entradas en "Surface Types" con el tag y los clips correspondientes
/// El script detecta automáticamente qué superficie pisas.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FootstepSounds : MonoBehaviour
{
    [Header("Sonidos de pasos")]
    [Tooltip("Clips de audio de pasos por defecto (se eligen al azar)")]
    public AudioClip[] defaultFootsteps;

    [Header("Configuración")]
    [Tooltip("Segundos entre cada paso (0.4 = caminar, 0.25 = correr)")]
    public float stepInterval = 0.4f;

    [Tooltip("Velocidad mínima para que suenen los pasos")]
    public float minVelocity = 0.1f;

    [Tooltip("Volumen de los pasos (0 a 1)")]
    [Range(0f, 1f)]
    public float volume = 0.5f;

    [Header("Superficies (opcional)")]
    [Tooltip("Sonidos diferentes según el material del suelo. Usa Tags en los objetos del suelo.")]
    public SurfaceType[] surfaceTypes;

    private CharacterController controller;
    private AudioSource audioSource;
    private float stepTimer;
    private int lastClipIndex = -1;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Solo sonar cuando el jugador está en el suelo y moviéndose
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0f;
        bool isMoving = horizontalVelocity.magnitude > minVelocity;

        if (controller.isGrounded && isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            // Reiniciar timer para que el primer paso suene inmediatamente
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        AudioClip[] clips = GetClipsForCurrentSurface();

        if (clips == null || clips.Length == 0) return;

        // Elegir un clip al azar (evitando repetir el anterior)
        AudioClip clip = PickRandomClip(clips);
        audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Detecta qué superficie pisa el jugador y devuelve los clips correspondientes.
    /// Lanza un raycast hacia abajo para comprobar el tag del suelo.
    /// </summary>
    AudioClip[] GetClipsForCurrentSurface()
    {
        if (surfaceTypes != null && surfaceTypes.Length > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
            {
                foreach (var surface in surfaceTypes)
                {
                    if (hit.collider.CompareTag(surface.tag))
                        return surface.clips;
                }
            }
        }

        return defaultFootsteps;
    }

    /// <summary>
    /// Elige un clip al azar sin repetir el último (para evitar sonido robótico).
    /// </summary>
    AudioClip PickRandomClip(AudioClip[] clips)
    {
        if (clips.Length == 1) return clips[0];

        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        } while (index == lastClipIndex);

        lastClipIndex = index;
        return clips[index];
    }
}

/// <summary>
/// Define un tipo de superficie con su tag y sus sonidos de pasos.
/// Ejemplo: tag = "Wood", clips = [paso_madera_1, paso_madera_2, paso_madera_3]
/// </summary>
[System.Serializable]
public class SurfaceType
{
    [Tooltip("Tag del suelo (ej: Wood, Grass, Metal, Stone)")]
    public string tag;

    [Tooltip("Clips de pasos para esta superficie")]
    public AudioClip[] clips;
}

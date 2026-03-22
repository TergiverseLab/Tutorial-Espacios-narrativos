using UnityEngine;

/// <summary>
/// Define las propiedades de un objeto interactuable.
/// Es un ScriptableObject: se crea desde Assets → Create → Item.
///
/// Uso:
/// 1. Click derecho en el panel Project → Create → Item
/// 2. Dale un nombre (ej: "NotaSecreta", "CuadroMisterioso")
/// 3. Configura sus propiedades en el Inspector
/// 4. Asígnalo al componente Interactables de un objeto en la escena
///
/// Propiedades:
/// - grabbable: el jugador puede coger el objeto y rotarlo en 3D
/// - audioClip: sonido que se reproduce al interactuar
/// - text: texto/subtítulo que aparece al interactuar
/// - image: imagen que se muestra en pantalla (ej: un documento, una foto)
/// - inventoryItem: si se añade al inventario del jugador
/// - requiredItem: si es necesario para completar el juego
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>
[CreateAssetMenu(menuName = "NarracionKit/Item")]
public class Item : ScriptableObject
{
    [Header("Interacción")]
    [Tooltip("¿El jugador puede coger este objeto y rotarlo?")]
    public bool grabbable;

    [Tooltip("Sonido que se reproduce al interactuar (voz, efecto, música...)")]
    public AudioClip audioClip;

    [Tooltip("Texto que aparece como subtítulo al interactuar")]
    [TextArea]
    public string text;

    [Tooltip("Imagen que se muestra en pantalla (documento, foto, nota...)")]
    public Sprite image;

    [Header("Inventario")]
    [Tooltip("¿Este objeto se guarda en el inventario del jugador?")]
    public bool inventoryItem;

    [Tooltip("Mensaje que aparece cuando el jugador recoge este objeto")]
    public string collectMessage;

    [Header("Progresión")]
    [Tooltip("¿Este objeto es necesario para completar el juego/escena?")]
    public bool requiredItem;
}

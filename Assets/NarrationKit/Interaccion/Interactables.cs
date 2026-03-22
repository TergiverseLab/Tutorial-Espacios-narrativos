using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Marca un objeto del mundo como interactuable.
/// Ponlo en cualquier GameObject que el jugador pueda examinar.
///
/// Configuración:
/// 1. Añade este componente al objeto (debe tener un Collider)
/// 2. Crea un Item (Assets → Create → NarracionKit → Item) y asígnalo
/// 3. Configura los eventos OnInteract (qué pasa al interactuar)
///
/// Interacciones condicionales (previousItem):
/// Si quieres que un objeto reaccione diferente según lo que el jugador
/// ya haya recogido, usa el array "Interacciones Condicionales".
/// Ejemplo: un PC que dice "necesitas contraseña" pero si tienes la nota
/// con la contraseña, dice "acceso concedido".
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>

[System.Serializable]
public class PreviousItem
{
    [Tooltip("Item que el jugador debe tener en el inventario")]
    public Item requiredItem;

    [Tooltip("Item alternativo que se usa si el jugador tiene el requiredItem")]
    public Item interactionItem;

    [Tooltip("Evento que se dispara para esta interacción condicional")]
    public UnityEvent OnInteract;
}

public class Interactables : MonoBehaviour
{
    [Tooltip("El Item principal asociado a este objeto")]
    public Item item;

    [Header("Interacciones Condicionales")]
    [Tooltip("Interacciones alternativas según lo que el jugador ya tenga")]
    public PreviousItem[] previousItem;

    [Header("Eventos")]
    [Tooltip("Se ejecuta siempre que el jugador interactúa con este objeto")]
    public UnityEvent OnInteract;

    [Tooltip("Se ejecuta cuando el jugador recoge este objeto en su inventario")]
    public UnityEvent CollectItem;

    [HideInInspector]
    public bool isMoving;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema de inventario y condiciones de victoria.
///
/// Configuración:
/// 1. Añade este script al jugador (mismo objeto que PlayerInteraction)
/// 2. (Opcional) Configura las condiciones de victoria:
///    - Cada WinCondition tiene una lista de Items requeridos
///    - Cuando el jugador recoge todos los items de una condición,
///      se ejecuta la cutscene asociada
///
/// Para un walking sim simple puedes ignorar las condiciones de victoria
/// y usar solo el inventario como lista de objetos recogidos.
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>

[System.Serializable]
public class WinCondition
{
    [Tooltip("Items que el jugador debe encontrar para completar esta condición")]
    public List<Item> requiredItens;

    [HideInInspector]
    public List<Item> interactedItens;

    [Tooltip("Cutscene que se reproduce al completar esta condición (opcional)")]
    public CutsceneController winCutscene;

    [HideInInspector]
    public bool alreadyPlayed;
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Condiciones de Victoria (opcional)")]
    [Tooltip("Define qué objetos hay que encontrar para 'ganar' o desbloquear cutscenes")]
    public WinCondition[] winCondition;

    [Header("Inventario")]
    [Tooltip("Lista de objetos recogidos por el jugador")]
    public List<Item> itens;

    /// <summary>
    /// Añade un item al inventario (evita duplicados).
    /// </summary>
    public void AddItem(Item item)
    {
        if (itens.Contains(item)) return;

        UIManager.instance.SetItens(item, itens.Count);
        itens.Add(item);
    }

    /// <summary>
    /// Registra un item requerido y comprueba si se cumple alguna condición de victoria.
    /// </summary>
    public void AddRequiredItens(Item item)
    {
        for (int i = 0; i < winCondition.Length; i++)
        {
            if (winCondition[i].requiredItens.Contains(item))
            {
                if (!winCondition[i].interactedItens.Contains(item))
                {
                    winCondition[i].interactedItens.Add(item);
                }
            }
        }

        for (int i = 0; i < winCondition.Length; i++)
        {
            if (winCondition[i].requiredItens.Count == winCondition[i].interactedItens.Count)
            {
                if (!winCondition[i].alreadyPlayed)
                {
                    winCondition[i].alreadyPlayed = true;
                    if (winCondition[i].winCutscene != null)
                        StartCoroutine(PlayCutscene(winCondition[i].winCutscene));
                    break;
                }
            }
        }
    }

    IEnumerator PlayCutscene(CutsceneController cutscene)
    {
        yield return new WaitForSeconds(1.5f);
        cutscene.Play();
    }
}

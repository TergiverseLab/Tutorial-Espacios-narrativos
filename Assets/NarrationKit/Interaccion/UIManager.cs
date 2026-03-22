using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gestor de la interfaz de usuario (singleton).
/// Controla: cursor de mano, subtítulos, imágenes, inventario, notificaciones.
///
/// Configuración:
/// 1. Crea un Canvas en la escena
/// 2. Dentro del Canvas crea:
///    - Un Text para subtítulos (captionsText)
///    - Un Image para el cursor de mano (handCursor) - centrado en pantalla
///    - Un Image para "click derecho para cerrar" (backImage)
///    - Un Image para mostrar documentos/fotos (interactionImage)
///    - Un Panel para el inventario (inventoryImage) con Texts dentro
///    - Un Text para notificaciones flotantes (infoText)
/// 3. Arrastra cada elemento a los campos de este script
/// 4. Pon este script en el Canvas o en un objeto vacío "UI"
///
/// Acceso desde otros scripts: UIManager.instance.SetCaptions("texto");
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Subtítulos")]
    [Tooltip("Texto para mostrar subtítulos durante las interacciones")]
    public Text captionsText;

    [Header("Cursores")]
    [Tooltip("Imagen de mano que aparece al mirar un objeto interactuable")]
    public GameObject handCursor;

    [Tooltip("Imagen de 'click derecho para cerrar' que aparece al examinar")]
    public GameObject backImage;

    [Header("Visor de contenido")]
    [Tooltip("Imagen para mostrar documentos, fotos o notas")]
    public Image interactionImage;

    [Header("Inventario")]
    [Tooltip("Panel del inventario (se muestra/oculta con la tecla I)")]
    public GameObject inventoryImage;

    [Tooltip("Textos dentro del panel de inventario (uno por objeto recogido)")]
    public Text[] inventoryItens;

    [Tooltip("Texto de notificación flotante al recoger un objeto")]
    public Text infoText;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // Abrir/cerrar inventario con la tecla I
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryImage != null)
                inventoryImage.SetActive(!inventoryImage.activeInHierarchy);
        }
    }

    /// <summary>
    /// Muestra o borra el texto de subtítulos.
    /// </summary>
    public void SetCaptions(string text)
    {
        if (captionsText != null)
            captionsText.text = text;
    }

    /// <summary>
    /// Muestra u oculta el cursor de mano.
    /// </summary>
    public void SetHandCursor(bool state)
    {
        if (handCursor != null)
            handCursor.SetActive(state);
    }

    /// <summary>
    /// Muestra u oculta el indicador de "click derecho para cerrar".
    /// </summary>
    public void SetBackImage(bool state)
    {
        if (backImage != null)
            backImage.SetActive(state);

        if (!state && interactionImage != null)
        {
            interactionImage.enabled = false;
        }
    }

    /// <summary>
    /// Muestra una imagen (documento, foto, nota) en pantalla.
    /// </summary>
    public void SetImage(Sprite sprite)
    {
        if (interactionImage != null)
        {
            interactionImage.sprite = sprite;
            interactionImage.enabled = true;
        }
    }

    /// <summary>
    /// Actualiza el inventario y muestra una notificación flotante.
    /// </summary>
    public void SetItens(Item item, int index)
    {
        if (index < inventoryItens.Length && inventoryItens[index] != null)
            inventoryItens[index].text = item.collectMessage;

        if (infoText != null)
        {
            infoText.text = item.collectMessage;
            StartCoroutine(FadingText());
        }
    }

    IEnumerator FadingText()
    {
        if (infoText == null) yield break;

        Color newColor = infoText.color;

        // Fade in
        while (newColor.a < 1)
        {
            newColor.a += Time.deltaTime;
            infoText.color = newColor;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        // Fade out
        while (newColor.a > 0)
        {
            newColor.a -= Time.deltaTime;
            infoText.color = newColor;
            yield return null;
        }
    }
}

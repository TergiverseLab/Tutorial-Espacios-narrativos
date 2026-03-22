using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sistema principal de interacción del jugador.
/// Lanza un raycast desde el centro de la pantalla para detectar objetos
/// con el componente Interactables.
///
/// Funcionalidades:
/// - Mirar un objeto → aparece cursor de mano
/// - Click izquierdo → interactuar (audio + texto + imagen)
/// - Si es grabbable → el objeto se acerca y se puede rotar con el ratón
/// - Click derecho → cerrar interacción, devolver objeto
/// - Si es inventoryItem → se guarda en el inventario al cerrar
/// - Soporta interacciones condicionales (diferentes respuestas según inventario)
///
/// Configuración:
/// 1. Añade este script al jugador (mismo objeto que el CharacterController)
/// 2. Añade también AudioPlayer y PlayerInventory al jugador
/// 3. Crea un objeto vacío hijo de la cámara llamado "ObjectViewer"
///    (será la posición donde se muestran los objetos examinados)
/// 4. Arrastra ObjectViewer al campo correspondiente
/// 5. Configura los eventos OnView/OnFinishView si quieres bloquear
///    el movimiento del jugador durante la interacción
///
/// Adaptado de GabrielGameDev/Walking-Simulator
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Distancia máxima para interactuar con objetos")]
    public float rayDistance = 2f;

    [Tooltip("Velocidad de rotación al examinar objetos")]
    public float rotateSpeed = 200;

    [Tooltip("Sonido que se reproduce al guardar un objeto en el inventario")]
    public AudioClip writingSound;

    [Tooltip("Transform hijo de la cámara donde se muestran los objetos examinados")]
    public Transform objectViewer;

    [Header("Eventos")]
    [Tooltip("Se ejecuta al empezar a ver un objeto (ej: desactivar movimiento)")]
    public UnityEvent OnView;

    [Tooltip("Se ejecuta al terminar de ver un objeto (ej: reactivar movimiento)")]
    public UnityEvent OnFinishView;

    private Camera myCam;
    private bool isViewing;
    private bool canFinish;

    private Interactables currentInteractable;
    private Item currentItem;
    private Vector3 originPosition;
    private Quaternion originRotation;

    private AudioPlayer audioPlayer;
    private PlayerInventory inventory;

    private void Awake()
    {
        audioPlayer = GetComponent<AudioPlayer>();
        inventory = GetComponent<PlayerInventory>();
    }

    void Start()
    {
        myCam = Camera.main;
    }

    void Update()
    {
        CheckInteractables();
    }

    void CheckInteractables()
    {
        if (isViewing)
        {
            // Si el objeto es rotable, permitir rotación con click izquierdo
            if (currentInteractable.item.grabbable && Input.GetMouseButton(0))
            {
                RotateObject();
            }

            // Click derecho para cerrar la vista
            if (canFinish && Input.GetMouseButtonDown(1))
            {
                FinishView();
            }

            return;
        }

        // Lanzar raycast desde el centro de la pantalla
        RaycastHit hit;
        Vector3 rayOrigin = myCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));

        if (Physics.Raycast(rayOrigin, myCam.transform.forward, out hit, rayDistance))
        {
            Interactables interactable = hit.collider.GetComponent<Interactables>();
            if (interactable != null)
            {
                // Mostrar cursor de mano
                UIManager.instance.SetHandCursor(true);

                if (Input.GetMouseButtonDown(0))
                {
                    if (interactable.isMoving) return;

                    currentInteractable = interactable;
                    currentInteractable.OnInteract.Invoke();

                    if (currentInteractable.item != null)
                    {
                        OnView.Invoke();
                        isViewing = true;

                        // Comprobar interacciones condicionales
                        bool hasPreviousItem = false;
                        for (int i = 0; i < currentInteractable.previousItem.Length; i++)
                        {
                            if (inventory.itens.Contains(currentInteractable.previousItem[i].requiredItem))
                            {
                                Interact(currentInteractable.previousItem[i].interactionItem);
                                currentInteractable.previousItem[i].OnInteract.Invoke();
                                hasPreviousItem = true;
                                break;
                            }
                        }

                        if (hasPreviousItem) return;

                        // Interacción por defecto
                        Interact(currentInteractable.item);

                        // Si es cogible, moverlo frente a la cámara
                        if (currentInteractable.item.grabbable)
                        {
                            originPosition = currentInteractable.transform.position;
                            originRotation = currentInteractable.transform.rotation;
                            StartCoroutine(MovingObject(currentInteractable, objectViewer.position));
                        }
                    }
                }
            }
            else
            {
                UIManager.instance.SetHandCursor(false);
            }
        }
        else
        {
            UIManager.instance.SetHandCursor(false);
        }
    }

    void Interact(Item item)
    {
        currentItem = item;

        // Mostrar imagen si tiene
        if (item.image != null)
        {
            UIManager.instance.SetImage(item.image);
        }

        // Reproducir audio
        audioPlayer.PlayAudio(item.audioClip);

        // Mostrar texto
        UIManager.instance.SetCaptions(item.text);

        // Permitir cerrar después de que termine el audio
        if (item.audioClip != null)
        {
            Invoke("CanFinish", item.audioClip.length + 0.5f);
        }
        else
        {
            Invoke("CanFinish", 1f);
        }
    }

    void CanFinish()
    {
        canFinish = true;

        // Si no hay imagen ni objeto cogible, cerrar automáticamente
        if (currentItem.image == null && !currentItem.grabbable)
        {
            FinishView();
        }
        else
        {
            UIManager.instance.SetBackImage(true);
        }

        UIManager.instance.SetCaptions("");
    }

    void FinishView()
    {
        canFinish = false;
        isViewing = false;
        UIManager.instance.SetBackImage(false);

        // Si es de inventario, guardarlo
        if (currentItem.inventoryItem)
        {
            inventory.AddItem(currentItem);
            audioPlayer.PlayAudio(writingSound);
            currentInteractable.CollectItem.Invoke();
        }

        // Si era cogible, devolverlo a su posición original
        if (currentItem.grabbable)
        {
            currentInteractable.transform.rotation = originRotation;
            StartCoroutine(MovingObject(currentInteractable, originPosition));
        }

        // Si es requerido, registrar progreso
        if (currentItem.requiredItem)
        {
            inventory.AddRequiredItens(currentItem);
        }

        OnFinishView.Invoke();
    }

    IEnumerator MovingObject(Interactables obj, Vector3 position)
    {
        obj.isMoving = true;
        float timer = 0;
        while (timer < 1)
        {
            obj.transform.position = Vector3.Lerp(obj.transform.position, position, Time.deltaTime * 5);
            timer += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = position;
        obj.isMoving = false;
    }

    void RotateObject()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        currentInteractable.transform.Rotate(myCam.transform.right, -Mathf.Deg2Rad * y * rotateSpeed, Space.World);
        currentInteractable.transform.Rotate(myCam.transform.up, -Mathf.Deg2Rad * x * rotateSpeed, Space.World);
    }
}

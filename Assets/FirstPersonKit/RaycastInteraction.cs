using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastInteraction : MonoBehaviour
{
    /// <summary>
    /// This script shoots a raycast and if it find a trigger collider executes the action
    /// on TriggerInteraction OnInteract
    /// </summary>

    [Tooltip("Project settings > Input manager to change the mapping")]
    public string interactionButton = "Fire1";
    [Tooltip("Max raycast distance")]
    public float interactionDistance = 4f;
    [Tooltip("The offset of the ray on the y axis")]
    public float rayY = 1f;

    private TriggerInteraction currentInteractable;

    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //raycast to see if there is a collider with an interactable attached
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        //in the editor window visualize the raycast
        Debug.DrawRay(transform.position + new Vector3(0, rayY, 0), fwd * interactionDistance, Color.green);

        TriggerInteraction ti = null;

        //do a reverse raycast checking all the colliders
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + new Vector3(0, rayY, 0) + fwd * 100, -fwd, 100);

        foreach (RaycastHit h in hits)
        {
            //if hit something see if it's a trigger
            if (h.collider.isTrigger)
            {
                //see if there is a trigger interaction
                if (h.collider.gameObject.GetComponent<TriggerInteraction>() != null)
                {
                    //see if it's within the distance
                    float dist = Vector3.Distance(transform.position + new Vector3(0, rayY, 0), h.point);

                    if (dist < interactionDistance)
                    {
                        //then it's in range our you are inside it get it and call the function
                        ti = h.collider.gameObject.GetComponent<TriggerInteraction>();
                    }
                }
            }


        }

        //if any of the two raycast succeed and there is a trigger interaction component
        //and the first time it's triggered
        if (ti != null)
        {
            if (ti != currentInteractable)
            {
                currentInteractable = ti;

                //invoke whatever aim function is associated
                ti.OnAim.Invoke();
            }

            //invoke whatever interact function is associated
            if (Input.GetButtonDown(interactionButton))
                ti.OnInteract.Invoke();
        }
        else
        {
            currentInteractable = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;

//This script detect a trigger enter and exit and plays an even specified in the inspector
public class TriggerInteraction : MonoBehaviour
{
    [Tooltip("If not set it will look for an object named 'player'")]
    public GameObject player;
    [Tooltip("If not blank it will set the text on enter exit")]
    public string label = "";
    [Tooltip("If not set it fill try to look for an object called labelField")]
    public TMP_Text labelField;

    [Serializable]
    public class MyEvent : UnityEvent { }

    public MyEvent EnterTrigger;
    public MyEvent ExitTrigger;
    public MyEvent OnInteract;
    public MyEvent OnAim;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            //if not set assumes the template setup
            player = GameObject.Find("player");
        }

        if (label != "")
        {
            if (labelField == null)
            {
                GameObject lo = GameObject.Find("labelField");
                if (lo == null)
                {
                    Debug.LogWarning("Warning: " + gameObject.name + " has a label but no text field nor object named labelField can be found");
                }
                else
                {
                    labelField = lo.GetComponent<TMP_Text>();

                    if (labelField == null)
                        Debug.LogWarning("Warning: the object named labelField must have a TMP_Text attached");
                }
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            if (labelField != null && label != "")
                labelField.text = label;

            EnterTrigger.Invoke();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            if (labelField != null)
                labelField.text = "";

            ExitTrigger.Invoke();
        }
    }

}

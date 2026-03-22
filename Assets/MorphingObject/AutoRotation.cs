using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    //set in the inspector
    public Vector3 deltaRotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate locally
        transform.Rotate(deltaRotation*Time.deltaTime, Space.Self);
    }
}

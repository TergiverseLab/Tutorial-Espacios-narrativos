using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //restarts the current scene
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //changes the scene by name (it has to be added to the build)
    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void Activate(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Warning: Select an object to activate");
        }
        else
        {
            obj.SetActive(true);
        }
    }

    public void Deactivate(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("Warning: Select an object to activate");
        }
        else
        {
            obj.SetActive(false);
        }
    }




    //teleports the object this script is attached to, to a new location 
    public void Teleport(Transform destination)
    {
        CharacterController cc = gameObject.GetComponent<CharacterController>();

        if (destination == null)
        {
            Debug.LogWarning("Warning: no destination (object) has been choosen");
        }
        else
        {
            //disable character controller if any
            if (cc != null)
                cc.enabled = false;

            transform.position = destination.transform.position;
            
            //inherits the location
            transform.rotation = destination.transform.rotation;
            
            //enable character controller
            if (cc != null)
            {
                cc.enabled = true;
            }
        }
    }

    //plays a one shot audio clip on the current object source
    //you can select the sound dynamically
    public void PlaySound(AudioClip clip)
    {
        AudioSource source = GetComponent<AudioSource>();
        
        //if no source add one
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        if (clip == null)
        {
            print("Warning: no audioclip choosen");
        }
        else
        {
            source.PlayOneShot(clip);
        }
    }


    //plays a selected source loaded with whatever settings and clip the component has
    public void PlaySource(AudioSource source)
    {
        if (source == null) {
            Debug.LogWarning("Warning: Select a source");
        }
        else
        {
            source.Play();
        }
    }

    public void StopSource(AudioSource source)
    {
        if (source == null)
        {
            Debug.LogWarning("Warning: Select an audio source");
        }
        else
        {
            source.Stop();
        }
    }

    //prints a message in console
    public void DebugMessage(string msg)
    {
        print(msg);
    }


}

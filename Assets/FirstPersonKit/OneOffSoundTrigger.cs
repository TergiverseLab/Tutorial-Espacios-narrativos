using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneOffSoundTrigger : MonoBehaviour
{
    public AudioSource source;
    public AudioListener listener;

    [Tooltip("You can set the player collider")]
    public Collider playerCollider;
    
    [Tooltip("The collider is the one that triggers the sound")]
    public Collider soundTrigger;

    [Tooltip("You can set one or more random sound to be played on enter")]
    public AudioClip[] audioClips;

    [Tooltip("Play at every enter or play and disable itself")]
    public bool playOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        //if not set initialize

        //assumes the audio source on the same object
        if (source == null)
        {
            source = gameObject.GetComponent<AudioSource>();
            source.spatialBlend = 0; //has to be 2D

            if (source.loop == false)
                print("Area sound trigger would probably require a looped sound");
        }

        //assumes the listener is on the camera
        if (listener == null)
            listener = Camera.main.gameObject.GetComponent<AudioListener>();

        if (soundTrigger == null)
        {
            soundTrigger = gameObject.GetComponent<Collider>();

            soundTrigger.isTrigger = true;
        }

        if (playerCollider == null)
        {
            //if not set assumes the template setup
            playerCollider = GameObject.Find("player").GetComponent<Collider>();
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        //it's the player
        if (other.gameObject == playerCollider.gameObject)
        {
            if(audioClips.Length>0)
            {
                AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
                source.PlayOneShot(randomClip);
            }
            else
            {
                //otherwise default
                source.PlayOneShot(source.clip);
            }

            if(playOnce)
            {
                soundTrigger.enabled = false;
            }

        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSoundTrigger : MonoBehaviour
{
    public AudioSource source;
    public AudioListener listener;

    [Tooltip("You can set the player collider")]
    public Collider playerCollider;

    [Tooltip("The collider is the one that triggers the sound")]
    public Collider soundTrigger;
    [Tooltip("How long for the sound to fade in and out")]
    public float fadeTime = 2;

    private Coroutine fadeRoutine;

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

        if(playerCollider == null)
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
            if(fadeRoutine!=null)
                StopCoroutine(fadeRoutine);
            
            fadeRoutine = StartCoroutine(FadeIn(source, fadeTime));
        }
    }

    private void OnTriggerExit(Collider other)
    {

        //it's the player
        if (other.gameObject == playerCollider.gameObject)
        {
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(FadeOut(source, fadeTime));
        }
    }

    

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
        
        while (audioSource.volume > 0)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0, Time.deltaTime / FadeTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 0;
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {

        float startVolume = 0f;

        if (audioSource.volume > 0)
            startVolume = audioSource.volume;

        audioSource.Play();

        while (audioSource.volume < 1.0f)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 1, Time.deltaTime / FadeTime);
            yield return null;
        }

        audioSource.volume = 1f;
    }

}

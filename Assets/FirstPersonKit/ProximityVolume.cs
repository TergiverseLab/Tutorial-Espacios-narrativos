using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//makes an audiosource louder based on the proximity to the camera/listener
//different from 3D sound in that retains stereo and doesn't use pan to pinpoint a sound in space

public class ProximityVolume : MonoBehaviour
{
    public AudioSource source;
    public AudioListener listener;

    [Tooltip("Farther than this is silent")]
    public float maxRange = 10;
    [Tooltip("Closer than this stays at maximum volume")]
    public float minRange = 5;
    [Tooltip("Volume is the attenuation of the original audio clip (0-1)")]
    public float maxVolume = 1;

    // Start is called before the first frame update
    void Start()
    {
        //if not set initialize

        //assumes the audio source on the same object
        if (source == null)
            source = gameObject.GetComponent<AudioSource>();

        //assumes the listener is on the camera
        if (listener == null)
            listener = Camera.main.gameObject.GetComponent<AudioListener>();

        //2D
        source.spatialBlend = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(listener.transform.position, transform.position);

        if (distance < maxRange) {

            source.volume = Mathf.Clamp01(Map(distance, maxRange, minRange, 0, maxVolume));
        }
        else
        {
            source.volume = 0;
        }
    }

    private float Map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
    {

        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        return (NewValue);
    }
}

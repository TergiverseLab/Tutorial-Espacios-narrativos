using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundMorpher : MonoBehaviour
{

    [Tooltip("The renderer with blendshapes")]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    [Tooltip("Respond to microphone. If not set it expects an audiosource with a soundclip.")]
    public bool useMicrophone = true;

    [Tooltip("The number of the blendshape to animate starting from 0")]
    public int blendNumber = 0;

    [Tooltip("The frequency you are detecting 0-8 from bass to high freqs")]
    public int frequency = 3;

    [Tooltip("How sensitive is the blendshape to the sound")]
    public float sensitivity = 100;

    [Tooltip("How smoothed/responsive is the blendshape to the change of frequencies")]
    public float smoothing = 100;
    
    private float[] spectrum = new float[512];
    private float[] freqBand = new float[8];
    private float blendWeight = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

        if(useMicrophone)
            InitMicrophone();
        else
        {
            AudioSource source = gameObject.GetComponent<AudioSource>();
            if (source.clip == null)
                Debug.Log("The audiosource on SoundMorpher doesn't have an audio file.");
        }
    }

    void InitMicrophone()
    {
        AudioSource source = gameObject.GetComponent<AudioSource>();

        if (Microphone.devices.Length > 0)
        {
            int minFreq, maxFreq, freq;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            freq = Mathf.Min(44100, maxFreq);
            
            source = GetComponent<AudioSource>();
            source.clip = Microphone.Start(null, true, 5, freq);
            source.loop = true;

            while (!(Microphone.GetPosition(null) > 0)) { }
            source.Play();
        }
        else
        {
            Debug.Log("No Mic connected!");
        }
    }
    
    void Update()
    {
        //get the spectrum
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        //splits it in 8 bands
        MakeFrequencyBands();

        frequency = Mathf.Clamp(frequency, 0, freqBand.Length-1);

        blendNumber = Mathf.Clamp(blendNumber, 0, skinnedMeshRenderer.sharedMesh.blendShapeCount - 1);


        float targetValue = freqBand[frequency] * sensitivity * 100;
        
        blendWeight = blendWeight + (targetValue - blendWeight) / smoothing;

        blendWeight = Mathf.Clamp(blendWeight, 0, 100);
        skinnedMeshRenderer.SetBlendShapeWeight(blendNumber, blendWeight);
        
    }


    //splits the spectrum in 8 bands
    //from https://xr.berkeley.edu/decal/homework/hw2/
    void MakeFrequencyBands()
    {
        int count = 0;

        // Iterate through the 8 bins.
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i + 1);

            // Adding the remaining two samples into the last bin.
            if (i == 7)
            {
                sampleCount += 2;
            }

            // Go through the number of samples for each bin, add the data to the average
            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrum[count];
                count++;
            }

            // Divide to create the average, and scale it appropriately.
            average /= count;
            freqBand[i] = (i + 1) * 100 * average;
        }
    }


    //similar to p5 map function remap OldValue from a range between OldMin and OldMax
    //to a range NewMin NewMax
    public static float Map(float OldValue, float OldMin, float OldMax, float NewMin, float NewMax, bool clamp = false)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;

        if (clamp)
            NewValue = Mathf.Clamp(NewValue, NewMin, NewMax);

        return (NewValue);
    }
}

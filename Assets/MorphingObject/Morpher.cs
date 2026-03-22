using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morpher : MonoBehaviour
{

    [Tooltip("The renderer with blendshapes")]
    private SkinnedMeshRenderer skinnedMeshRenderer;

    [Tooltip("The number of the blendshape to animate starting from 0")]
    public int blendNumber = 0;
    
    [Tooltip("Oscillation duration in seconds")]
    public float oscillationTime = 1;

    public EasingFunction.Ease easing;

    
    // Start is called before the first frame update
    void Start()
    {
        if (skinnedMeshRenderer == null)
            skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time, oscillationTime);
        
        float tNormalized = Map(t, 0, oscillationTime, 0, 1);
        
        EasingFunction.Function f = EasingFunction.GetEasingFunction(easing);

        float easedValue = f(0, 1, tNormalized);

        blendNumber = Mathf.Clamp(blendNumber, 0, skinnedMeshRenderer.sharedMesh.blendShapeCount-1);

        float blendWeight = easedValue * 100;
        skinnedMeshRenderer.SetBlendShapeWeight(blendNumber, blendWeight);
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

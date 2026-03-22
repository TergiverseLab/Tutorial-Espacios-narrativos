using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightAndMaterials : MonoBehaviour
{


    public float colorTime = 0;

    public float counter = 0;

    private Color initialAmbientLight;

    public Light sun;
    public Light spotLight;

    public Color c1;
    public Color c2;

    public Material gradientSkybox;
    public Material starSkybox;


    // Start is called before the first frame update
    void Start()
    {

        //save the initial ambient light color
        initialAmbientLight = RenderSettings.ambientLight;

        //prevents skybox runtime changes from affecting the asset, just ignore
        RenderSettings.skybox = gradientSkybox = new Material(gradientSkybox);

        spotLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {

        //ambient light is set in Window > rendering > lighting > environment
        //in this scene ambient light is a color and can be changed like a color
        //by default the ambient light is the color of the skybox but I changed it
        if (Input.GetKey(KeyCode.X))
        {
            //color constructor RGB values 0-1 (over 1 it will be HDR light-emitting bright)
            RenderSettings.ambientLight = new Color(0.9f, 0.7f, 0.2f);
        }

        //the difference with using else is that it won't be triggered at every frame
        if (Input.GetKeyUp(KeyCode.X))
        {
            //set the original ambient light color saved at the beginning when unpress
            RenderSettings.ambientLight = initialAmbientLight;
        }

        if (Input.GetKey(KeyCode.Z))
        {

            //color preset red
            Color c1 = Color.red;

            //creating color with hue saturation brightness 0-1 values
            //cycling the hue

            colorTime += Time.deltaTime;
            Color c2 = Color.HSVToRGB(colorTime % 1, 0.8f, 0.9f);

            //skybox materials typically have multiple colors and properties
            //to change them you have to use a shader variable setter because
            //shaders are not in c#
            //to see the internal names of the properties they usually start with _
            //select the shader asset in asset panel and check the inspector
            RenderSettings.skybox.SetColor("_SkyGradientTop", c1);
            RenderSettings.skybox.SetColor("_SkyGradientBottom", c2);

            //if you ambient light comes from the skybox (Window > rendering > lighting > environment)
            //you need to call this function every time you change the skybox
            DynamicGI.UpdateEnvironment();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            sun.transform.rotation = Random.rotation;
        }

        if (Input.GetKey(KeyCode.V))
        {
            //goes back and forth between 0 and 1 (second value)
            //2 makes it ping pong faster
            float t = Mathf.PingPong(Time.time * 2, 1);

            //I use that value to interpolate between to predefined colors
            sun.color = Color.Lerp(c1, c2, t);
        }


        //this kills all the lights and skybox to show the spotlight
        if (Input.GetKeyDown(KeyCode.B))
        {
            spotLight.intensity = 2;
            sun.intensity = 0;
            RenderSettings.ambientLight = Color.black;
            RenderSettings.skybox = starSkybox;
            DynamicGI.UpdateEnvironment();
        }

        //keyup restores the setting
        if (Input.GetKeyUp(KeyCode.B))
        {
            spotLight.intensity = 0;
            sun.intensity = 1;
            RenderSettings.ambientLight = initialAmbientLight;
            RenderSettings.skybox = gradientSkybox;
            DynamicGI.UpdateEnvironment();
        }

        //change light + mouse wheel
        //Input.mouseScrollDelta is -1 or 1 every frame the wheel is scrolled, otherwise 0
        if (Input.mouseScrollDelta.y != 0)
        {
            sun.intensity += Input.mouseScrollDelta.y / 5;
        }


    }
    //end update

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

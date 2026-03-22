using UnityEngine;
using UnityEngine.Video;
using System.Collections;

/// <summary>
/// Luz reactiva al video: una Point Light cambia de color dinámicamente
/// siguiendo el contenido del video, como un efecto Ambilight.
///
/// Configuración:
/// 1. Ten un VideoPlayer con una RenderTexture asignada como Target Texture
/// 2. Crea una o más Point Lights cerca de la pantalla
/// 3. Añade este script a CUALQUIER objeto de la escena
/// 4. Arrastra la RenderTexture y la(s) luz(ces) al Inspector
/// 5. Pon la escena oscura para que el efecto se note
///
/// Nota técnica: el muestreo se hace con WaitForEndOfFrame para garantizar
/// que el VideoPlayer ya ha escrito el frame actual al RenderTexture.
/// </summary>
public class VideoReactiveLight : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("La RenderTexture donde el VideoPlayer renderiza el video")]
    public RenderTexture videoRenderTexture;

    [Tooltip("Luz principal que reacciona al color promedio del video")]
    public Light mainLight;

    [Header("Modo Ambilight (opcional)")]
    public Light lightTop;
    public Light lightBottom;
    public Light lightLeft;
    public Light lightRight;

    [Header("Muestreo")]
    [Range(2, 16)]
    public int sampleResolution = 4;

    [Tooltip("Muestrear cada N frames")]
    [Range(1, 10)]
    public int sampleEveryNFrames = 3;

    [Header("Apariencia")]
    [Range(0.01f, 0.5f)]
    public float smoothSpeed = 0.1f;

    [Range(1f, 3f)]
    public float saturationBoost = 1.5f;

    [Range(0.5f, 50f)]
    public float intensityMultiplier = 15f;

    private RenderTexture downscaledRT;
    private Texture2D readTexture;
    private Color currentColor = Color.white;
    private Color targetColor = Color.white;
    private Color targetColorTop, targetColorBottom, targetColorLeft, targetColorRight;
    private Color currentColorTop, currentColorBottom, currentColorLeft, currentColorRight;
    private int frameCounter = 0;
    private bool useAmbilight = false;

    void Start()
    {
        downscaledRT = new RenderTexture(sampleResolution, sampleResolution, 0, RenderTextureFormat.ARGB32);
        downscaledRT.Create();
        readTexture = new Texture2D(sampleResolution, sampleResolution, TextureFormat.RGBA32, false);
        useAmbilight = (lightTop != null && lightBottom != null && lightLeft != null && lightRight != null);

        // Iniciar la corrutina de muestreo que espera al final del frame
        StartCoroutine(SampleLoop());
    }

    /// <summary>
    /// Corrutina principal: espera a que termine el rendering (WaitForEndOfFrame)
    /// para garantizar que el VideoPlayer ya ha escrito al RenderTexture.
    /// </summary>
    IEnumerator SampleLoop()
    {
        while (true)
        {
            // Esperar al final del frame — DESPUÉS de que el VideoPlayer haya renderizado
            yield return new WaitForEndOfFrame();

            frameCounter++;
            if (frameCounter % sampleEveryNFrames != 0) continue;
            if (videoRenderTexture == null) continue;

            // Escalar el video a textura diminuta
            Graphics.Blit(videoRenderTexture, downscaledRT);

            // Leer los píxeles (ahora sí tienen datos válidos)
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = downscaledRT;
            readTexture.ReadPixels(new Rect(0, 0, sampleResolution, sampleResolution), 0, 0, false);
            readTexture.Apply();
            RenderTexture.active = previous;

            Color[] pixels = readTexture.GetPixels();

            Color avg = AveragePixels(pixels, 0, pixels.Length);
            targetColor = ProcessColor(avg);

            if (useAmbilight)
            {
                int res = sampleResolution;
                int half = res / 2;
                targetColorTop = ProcessColor(AverageRegion(pixels, res, 0, half, res, res));
                targetColorBottom = ProcessColor(AverageRegion(pixels, res, 0, 0, res, half));
                targetColorLeft = ProcessColor(AverageRegion(pixels, res, 0, 0, half, res));
                targetColorRight = ProcessColor(AverageRegion(pixels, res, half, 0, res, res));
            }
        }
    }

    void Update()
    {
        // Interpolar suavemente cada frame (esto sí va en Update)
        if (mainLight != null)
        {
            currentColor = Color.Lerp(currentColor, targetColor, smoothSpeed);
            mainLight.color = currentColor;
            float brightness = 0.299f * currentColor.r + 0.587f * currentColor.g + 0.114f * currentColor.b;
            mainLight.intensity = Mathf.Max(1f, brightness * intensityMultiplier);
        }

        if (useAmbilight)
        {
            currentColorTop = Color.Lerp(currentColorTop, targetColorTop, smoothSpeed);
            currentColorBottom = Color.Lerp(currentColorBottom, targetColorBottom, smoothSpeed);
            currentColorLeft = Color.Lerp(currentColorLeft, targetColorLeft, smoothSpeed);
            currentColorRight = Color.Lerp(currentColorRight, targetColorRight, smoothSpeed);
            ApplyToLight(lightTop, currentColorTop);
            ApplyToLight(lightBottom, currentColorBottom);
            ApplyToLight(lightLeft, currentColorLeft);
            ApplyToLight(lightRight, currentColorRight);
        }
    }

    Color AveragePixels(Color[] pixels, int start, int end)
    {
        float r = 0, g = 0, b = 0;
        int count = 0;
        for (int i = start; i < end && i < pixels.Length; i++)
        {
            r += pixels[i].r;
            g += pixels[i].g;
            b += pixels[i].b;
            count++;
        }
        if (count == 0) return Color.black;
        return new Color(r / count, g / count, b / count);
    }

    Color AverageRegion(Color[] pixels, int texWidth, int xMin, int yMin, int xMax, int yMax)
    {
        float r = 0, g = 0, b = 0;
        int count = 0;
        for (int y = yMin; y < yMax; y++)
        {
            for (int x = xMin; x < xMax; x++)
            {
                int idx = y * texWidth + x;
                if (idx < pixels.Length)
                {
                    r += pixels[idx].r;
                    g += pixels[idx].g;
                    b += pixels[idx].b;
                    count++;
                }
            }
        }
        if (count == 0) return Color.black;
        return new Color(r / count, g / count, b / count);
    }

    Color ProcessColor(Color color)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        s = Mathf.Clamp01(s * saturationBoost);
        v = Mathf.Clamp01(v * 1.3f);
        return Color.HSVToRGB(h, s, v);
    }

    void ApplyToLight(Light light, Color color)
    {
        if (light == null) return;
        light.color = color;
        float brightness = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        light.intensity = Mathf.Max(0.1f, brightness * intensityMultiplier);
    }

    void OnDestroy()
    {
        if (downscaledRT != null) { downscaledRT.Release(); Destroy(downscaledRT); }
        if (readTexture != null) { Destroy(readTexture); }
    }
}

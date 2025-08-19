using System;
using System.IO;
using UnityEngine;

// taken from
// https://kpprt.de/code-snippet/in-editor-screenshot-script-for-unity/

[RequireComponent(typeof(Camera))]
public class Screenshot : MonoBehaviour {

    [SerializeField] int width = 1024;
    [SerializeField] int height = 512;
    [SerializeField] string folder = "Screenshots";
    [SerializeField] string filenamePrefix = "screenshot";
    [SerializeField] bool ensureTransparentBackground = false;
    [SerializeField] bool fixLinearGamma = false;
    [SerializeField] bool forceOpaque = true;

    void Awake() {
        gameObject.SetActive(false);
    }

    [ContextMenu("Take Screenshot")]
    public void TakeScreenshot() {
        folder = GetSafePath(folder.Trim('/'));
        filenamePrefix = GetSafeFilename(filenamePrefix);

        string dir = Application.dataPath + "/" + folder + "/";
        string filename = filenamePrefix + "_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".png";
        string path = dir + filename;
        Debug.Log($"Attempting to save at {path}");

        Camera cam = GetComponent<Camera>();

        RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        
        cam.targetTexture = rt;

        CameraClearFlags clearFlags = cam.clearFlags;
        Color backgroundColor = cam.backgroundColor;

        if(ensureTransparentBackground) {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(); // alpha is zero
        }

        cam.Render();

        if(ensureTransparentBackground) {
            cam.clearFlags = clearFlags;
            cam.backgroundColor = backgroundColor;
        }

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

        if (fixLinearGamma)
        {
            if (QualitySettings.activeColorSpace == ColorSpace.Linear)
            {
                Color[] pixels = screenshot.GetPixels();
                for (int p = 0; p < pixels.Length; p++)
                {
                    pixels[p] = pixels[p].gamma;
                }
                screenshot.SetPixels(pixels);
            }
        }

        if (forceOpaque)
        {
            Color[] pixels = screenshot.GetPixels();
            for (int p = 0; p < pixels.Length; p++)
            {
                pixels[p].a = 1.0f;
            }
            screenshot.SetPixels(pixels);
        }

        screenshot.Apply(false);

        Directory.CreateDirectory(dir);
        byte[] png = screenshot.EncodeToPNG();
        try {
            File.WriteAllBytes(path, png);
            Debug.Log("Screenshot saved to:\n" + path);
        }
        catch (Exception e) {
            Debug.LogError(e.Message);
        }
        cam.targetTexture = null;

        RenderTexture.active = currentRT;
    }

    public string GetSafePath(string path) {
        return string.Join("_", path.Split(Path.GetInvalidPathChars()));
    }

    public string GetSafeFilename(string filename) {
        return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}

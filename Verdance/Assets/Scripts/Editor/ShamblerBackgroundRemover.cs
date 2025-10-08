using UnityEngine;
using UnityEditor;
using System.IO;

public class ShamblerBackgroundRemover : EditorWindow
{
    private Texture2D sourceTexture;
    private Color targetColor = Color.black;
    private float tolerance = 0.01f;

    [MenuItem("Tools/Clean Shambler Background")]
    public static void ShowWindow()
    {
        GetWindow<ShamblerBackgroundRemover>("Shambler BG Remover");
    }

    private void OnGUI()
    {
        GUILayout.Label("Remove Background Color", EditorStyles.boldLabel);
        sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", sourceTexture, typeof(Texture2D), false);
        targetColor = EditorGUILayout.ColorField("Target Color", targetColor);
        tolerance = EditorGUILayout.Slider("Tolerance", tolerance, 0f, 0.2f);

        if (GUILayout.Button("Remove Background"))
        {
            if (sourceTexture != null)
                RemoveBackground(sourceTexture, targetColor, tolerance);
        }
    }

    private void RemoveBackground(Texture2D texture, Color bgColor, float tolerance)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null || !importer.isReadable)
        {
            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        Texture2D editable = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(texture, editable);

        Color[] pixels = editable.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            if (IsColorMatch(pixels[i], bgColor, tolerance))
                pixels[i] = new Color(0, 0, 0, 0); // Transparent
        }

        editable.SetPixels(pixels);
        editable.Apply();

        byte[] pngData = editable.EncodeToPNG();
        string newPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_cleaned.png";
        File.WriteAllBytes(newPath, pngData);
        AssetDatabase.Refresh();

        Debug.Log($"Cleaned background and saved to: {newPath}");
    }

    private bool IsColorMatch(Color a, Color b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}

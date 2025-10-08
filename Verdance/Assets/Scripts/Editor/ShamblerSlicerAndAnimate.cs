using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ShamblerSlicerAndAnimator : EditorWindow
{
    private Texture2D spriteSheet;
    private int columns = 8;
    private int rows = 8;
    private Vector2 pivot = new Vector2(0.5f, 0f); // Bottom-center
    private string[] animationNames = { "Idle", "Walk", "Charge", "Hurt", "Death", "Spawn", "Twitch", "Stagger" };
    private int frameRate = 12;

    [MenuItem("Tools/Shambler Slice + Animate")]
    public static void ShowWindow()
    {
        GetWindow<ShamblerSlicerAndAnimator>("Shambler Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Shambler Sprite Sheet Setup", EditorStyles.boldLabel);
        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        columns = EditorGUILayout.IntField("Columns", columns);
        rows = EditorGUILayout.IntField("Rows", rows);
        pivot = EditorGUILayout.Vector2Field("Pivot", pivot);
        frameRate = EditorGUILayout.IntField("Frame Rate", frameRate);

        if (GUILayout.Button("Slice & Generate Animations"))
        {
            if (spriteSheet != null)
            {
                SliceSpriteSheet(spriteSheet, columns, rows, pivot);
                GenerateAnimations(spriteSheet, columns, rows, animationNames, frameRate);
            }
        }
    }

    private void SliceSpriteSheet(Texture2D texture, int cols, int rows, Vector2 pivot)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

        importer.spriteImportMode = SpriteImportMode.Multiple;

        int frameWidth = texture.width / cols;
        int frameHeight = texture.height / rows;

        SpriteMetaData[] slices = new SpriteMetaData[cols * rows];

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int index = y * cols + x;
                SpriteMetaData slice = new SpriteMetaData
                {
                    name = $"shambler_{index}",
                    rect = new Rect(x * frameWidth, texture.height - (y + 1) * frameHeight, frameWidth, frameHeight),
                    pivot = pivot,
                    alignment = (int)SpriteAlignment.Custom
                };
                slices[index] = slice;
            }
        }

        importer.spritesheet = slices;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();
    }

    private void GenerateAnimations(Texture2D texture, int cols, int rows, string[] names, int fps)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> sprites = new List<Sprite>();

        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite)
                sprites.Add(sprite);
        }

        sprites.Sort((a, b) => a.name.CompareTo(b.name));

        string animFolder = "Assets/Animations/Shambler/";
        if (!Directory.Exists(animFolder))
            Directory.CreateDirectory(animFolder);

        for (int row = 0; row < rows && row < names.Length; row++)
        {
            AnimationClip clip = new AnimationClip();
            clip.frameRate = fps;

            EditorCurveBinding binding = new EditorCurveBinding
            {
                type = typeof(SpriteRenderer),
                path = "",
                propertyName = "m_Sprite"
            };

            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[cols];
            for (int col = 0; col < cols; col++)
            {
                int index = row * cols + col;
                keyframes[col] = new ObjectReferenceKeyframe
                {
                    time = col / (float)fps,
                    value = sprites[index]
                };
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);
            AssetDatabase.CreateAsset(clip, $"{animFolder}Shambler_{names[row]}.anim");
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Shambler animations generated.");
    }
}

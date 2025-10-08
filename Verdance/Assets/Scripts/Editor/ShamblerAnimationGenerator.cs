using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ShamblerAnimationGenerator : EditorWindow
{
    private Texture2D spriteSheet;
    private int columns = 6;
    private int totalRows = 9;
    private int frameRate = 12;

    private string[] animationNames = {
        "Idle", "Walk", "Charge", "Hurt", "Spawn", "Twitch"
    };
    private string deathName = "Death";

    [MenuItem("Tools/Generate Shambler Animations")]
    public static void ShowWindow()
    {
        GetWindow<ShamblerAnimationGenerator>("Shambler Animation Generator");
    }

    private void OnGUI()
    {
        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sliced Sprite Sheet", spriteSheet, typeof(Texture2D), false);
        columns = EditorGUILayout.IntField("Columns", columns);
        totalRows = EditorGUILayout.IntField("Total Rows", totalRows);
        frameRate = EditorGUILayout.IntField("Frame Rate", frameRate);

        if (GUILayout.Button("Generate Animations"))
        {
            if (spriteSheet != null)
                GenerateAnimations(spriteSheet, columns, totalRows, animationNames, deathName, frameRate);
        }
    }

    private void GenerateAnimations(Texture2D texture, int cols, int rows, string[] names, string deathName, int fps)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        List<Sprite> sprites = new List<Sprite>();

        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite)
                sprites.Add(sprite);
        }

        sprites.Sort((a, b) => a.name.CompareTo(b.name)); // Ensure correct order

        string animFolder = "Assets/Animations/Shambler/";
        if (!Directory.Exists(animFolder))
            Directory.CreateDirectory(animFolder);

        // Generate clips for first 6 rows
        for (int row = 0; row < names.Length; row++)
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

        // Generate Death animation from last 3 rows
        AnimationClip deathClip = new AnimationClip();
        deathClip.frameRate = fps;

        EditorCurveBinding deathBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] deathFrames = new ObjectReferenceKeyframe[cols * 3];
        for (int i = 0; i < cols * 3; i++)
        {
            int index = (rows - 3) * cols + i;
            deathFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / (float)fps,
                value = sprites[index]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(deathClip, deathBinding, deathFrames);
        AssetDatabase.CreateAsset(deathClip, $"{animFolder}Shambler_{deathName}.anim");

        AssetDatabase.SaveAssets();
        Debug.Log("Shambler animations generated.");
    }
}

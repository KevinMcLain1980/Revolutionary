using UnityEngine;
using UnityEditor;

public class ShamblerSpriteSlicer : EditorWindow
{
    private Texture2D spriteSheet;
    private int columns = 8;
    private int rows = 8;
    private Vector2 pivot = new Vector2(0.5f, 0f); // Bottom-center

    [MenuItem("Tools/Slice Shambler Sprite Sheet")]
    public static void ShowWindow()
    {
        GetWindow<ShamblerSpriteSlicer>("Shambler Sprite Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Sheet Slicer", EditorStyles.boldLabel);
        spriteSheet = (Texture2D)EditorGUILayout.ObjectField("Sprite Sheet", spriteSheet, typeof(Texture2D), false);

        columns = EditorGUILayout.IntField("Columns", columns);
        rows = EditorGUILayout.IntField("Rows", rows);
        pivot = EditorGUILayout.Vector2Field("Pivot", pivot);

        if (GUILayout.Button("Slice"))
        {
            if (spriteSheet != null)
            {
                SliceSpriteSheet(spriteSheet, columns, rows, pivot);
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

        Debug.Log($"Sliced {cols * rows} frames from {texture.name} with pivot {pivot}");
    }
}

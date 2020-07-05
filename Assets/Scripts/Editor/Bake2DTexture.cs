using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class Bake2DTexture : EditorWindow
{
    Material ImageMaterial;
    Vector2Int Resolution;
    string FilePath = "Assets/texture.png";

    bool hasMaterial = false;
    bool hasResolution = false;
    bool hasFile = false;

    [MenuItem ("Tools/Bake Textures")]
    static void OpenWindow()
	{
        Bake2DTexture window = EditorWindow.GetWindow<Bake2DTexture>();
        window.Show();
	}

    void OnGUI()
	{
        using (var check = new EditorGUI.ChangeCheckScope())
		{
            ImageMaterial = (Material)EditorGUILayout.ObjectField("Material", ImageMaterial, typeof(Material), false);
            hasMaterial = ImageMaterial != null;
            if (!hasMaterial)
            {
                EditorGUILayout.HelpBox("Missing a material to bake.", MessageType.Warning);
            }


            Resolution = EditorGUILayout.Vector2IntField("Resolution", Resolution);
            hasResolution = Resolution.x > 0 && Resolution.y > 0;
            if (!hasResolution)
            {
                EditorGUILayout.HelpBox("Please set a size bigger than zero.", MessageType.Warning);
            }


            FilePath = FileField(FilePath);
            try
            {
                string ext = Path.GetExtension(FilePath);
                hasFile = ext.Equals(".png");
            }
            catch (ArgumentException) { }
            if (!hasFile)
            {
                EditorGUILayout.HelpBox("No file to save the image to given.", MessageType.Warning);
            }

        }

        GUI.enabled = hasMaterial && hasResolution && hasFile;
        if (GUILayout.Button("Bake"))
		{
            Bake();
		}
        GUI.enabled = true;
    }

    string FileField(string path)
    {
        //allow the user to enter output file both as text or via file browser
        EditorGUILayout.LabelField("Image Path");
        using (new GUILayout.HorizontalScope())
        {
            path = EditorGUILayout.TextField(path);
            if (GUILayout.Button("choose"))
            {
                //set default values for directory, then try to override them with values of existing path
                string directory = "Assets";
                string fileName = "MaterialImage.png";
                try
                {
                    directory = Path.GetDirectoryName(path);
                    fileName = Path.GetFileName(path);
                }
                catch (ArgumentException) { }
                string chosenFile = EditorUtility.SaveFilePanelInProject("Choose image file", fileName,
                        "asset", "Please enter a file name to save the image to", directory);
                if (!string.IsNullOrEmpty(chosenFile))
                {
                    path = chosenFile;
                }
                //repaint editor because the file changed and we can't set it in the textfield retroactively
                Repaint();
            }
        }
        return path;
    }

    void Bake()
	{
        // render to renderTexture
        RenderTexture renderTexture = RenderTexture.GetTemporary(Resolution.x, Resolution.y);
        Graphics.Blit(null, renderTexture, ImageMaterial);

        // read pixels from renderTexture
        Texture2D texture = new Texture2D(Resolution.x, Resolution.y);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(Vector2.zero, Resolution), 0, 0);

        // save texture
        byte[] png = texture.EncodeToPNG();
        File.WriteAllBytes(FilePath, png);
        AssetDatabase.Refresh();

        //Clean
        RenderTexture.ReleaseTemporary(renderTexture);
        RenderTexture.active = null;
        DestroyImmediate(texture);
    }
}

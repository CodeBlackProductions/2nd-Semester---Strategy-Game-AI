using UnityEditor;
using UnityEngine;

public class Texture2DArrayCreator : MonoBehaviour
{
    [SerializeField] private Texture2D[] textures;
    [SerializeField] private string fileName;
    [SerializeField] private bool normalMap;

    [ContextMenu("CreateTexture2DArray")]
    public void CreateTexture2DArray()
    {
        int width = textures[0].width;
        int height = textures[0].height;
        Texture2DArray newTexture;

        if (normalMap)
        {
            newTexture = new Texture2DArray(width, height, textures.Length, TextureFormat.RGBA32, true, true);
        }
        else
        {
            newTexture = new Texture2DArray(width, height, textures.Length, TextureFormat.RGBA32, true);
        }

        for (int i = 0; i < newTexture.depth; i++)
        {
            newTexture.SetPixels(textures[i].GetPixels(), i);
        }

        newTexture.Apply();

        AssetDatabase.CreateAsset(newTexture, $"Assets/Simulation/Shader/Materials/Textures/{fileName}.asset");
    }
}
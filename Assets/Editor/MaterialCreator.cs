// Temporary script to create materials
using UnityEngine;
using UnityEditor;

public class MaterialCreator
{
    [MenuItem("Tools/Create Upgrade Materials")]
    static void CreateMaterials()
    {
        // Create Vertex Displacement Material
        Shader vertexShader = Shader.Find("Custom/UpgradeVertexDisplacement");
        if (vertexShader != null)
        {
            Material vertexMat = new Material(vertexShader);
            AssetDatabase.CreateAsset(vertexMat, "Assets/Materials/UpgradeVertexMaterial.mat");
            Debug.Log("Created UpgradeVertexMaterial.mat");
        }

        // Create Color Modulation Material
        Shader colorShader = Shader.Find("Custom/UpgradeColorModulation");
        if (colorShader != null)
        {
            Material colorMat = new Material(colorShader);
            AssetDatabase.CreateAsset(colorMat, "Assets/Materials/UpgradeColorMaterial.mat");
            Debug.Log("Created UpgradeColorMaterial.mat");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}

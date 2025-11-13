using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class SetupStartMenuUI
{
    [MenuItem("Tools/Setup Start Menu UI")]
    public static void Setup()
    {
        GameObject uiGO = GameObject.Find("StartMenuUI");
        if (uiGO == null)
        {
            Debug.LogError("StartMenuUI GameObject not found!");
            return;
        }
        
        UIDocument uiDoc = uiGO.GetComponent<UIDocument>();
        if (uiDoc == null)
        {
            uiDoc = uiGO.AddComponent<UIDocument>();
            Debug.Log("Added UIDocument component");
        }
        
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
        if (panelSettings != null)
        {
            uiDoc.panelSettings = panelSettings;
            Debug.Log("Assigned PanelSettings");
        }
        else
        {
            Debug.LogError("Could not load PanelSettings!");
        }
        
        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/UI/StartMenu.uxml");
        if (uxml != null)
        {
            uiDoc.visualTreeAsset = uxml;
            Debug.Log("Assigned UXML");
        }
        else
        {
            Debug.LogError("Could not load StartMenu.uxml!");
        }
        
        StartMenuUIDocument menuDoc = uiGO.GetComponent<StartMenuUIDocument>();
        if (menuDoc == null)
        {
            menuDoc = uiGO.AddComponent<StartMenuUIDocument>();
            Debug.Log("Added StartMenuUIDocument component");
        }
        
        StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/StartMenu.uss");
        if (uss != null && uxml != null)
        {
            SerializedObject so = new SerializedObject(menuDoc);
            so.FindProperty("_uxml").objectReferenceValue = uxml;
            so.FindProperty("_uss").objectReferenceValue = uss;
            so.ApplyModifiedProperties();
            Debug.Log("Assigned USS and UXML to StartMenuUIDocument");
        }
        
        EditorUtility.SetDirty(uiGO);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(uiGO.scene);
        
        Debug.Log("Setup complete!");
    }
}

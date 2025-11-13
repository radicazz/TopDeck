using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Editor script to automatically add UI to scenes when opened
/// </summary>
[InitializeOnLoad]
public static class SceneUISetup
{
    static SceneUISetup()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }
    
    static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (scene.name == "Start Menu")
        {
            SetupStartMenuUI(scene);
        }
        else if (scene.name == "Game")
        {
            SetupGameSceneUI(scene);
        }
    }
    
    static void SetupStartMenuUI(Scene scene)
    {
        // Check if UI already exists
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            if (go.name == "StartMenuUI" || go.GetComponent<UIDocument>() != null)
            {
                Debug.Log("[SceneUISetup] Start Menu UI already exists");
                return;
            }
        }
        
        Debug.Log("[SceneUISetup] Adding UI to Start Menu scene...");
        
        // Create GameObject
        GameObject uiGO = new GameObject("StartMenuUI");
        SceneManager.MoveGameObjectToScene(uiGO, scene);
        
        // Add UIDocument component
        UIDocument uiDoc = uiGO.AddComponent<UIDocument>();
        
        // Load and assign assets
        PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(
            "Assets/UI Toolkit/PanelSettings.asset");
        if (panelSettings != null)
        {
            uiDoc.panelSettings = panelSettings;
        }
        
        VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/UI/StartMenu.uxml");
        if (uxml != null)
        {
            uiDoc.visualTreeAsset = uxml;
        }
        
        // Add StartMenuUIDocument component
        StartMenuUIDocument menuDoc = uiGO.AddComponent<StartMenuUIDocument>();
        
        // Use SerializedObject to set private fields
        SerializedObject so = new SerializedObject(menuDoc);
        
        if (uxml != null)
        {
            SerializedProperty uxmlProp = so.FindProperty("_uxml");
            if (uxmlProp != null)
            {
                uxmlProp.objectReferenceValue = uxml;
            }
        }
        
        StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/UI/StartMenu.uss");
        if (uss != null)
        {
            SerializedProperty ussProp = so.FindProperty("_uss");
            if (ussProp != null)
            {
                ussProp.objectReferenceValue = uss;
            }
        }
        
        so.ApplyModifiedProperties();
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(scene);
        
        Debug.Log("[SceneUISetup] Start Menu UI added successfully!");
    }
    
    static void SetupGameSceneUI(Scene scene)
    {
        // Check if InfoHud exists
        InfoHudUIDocument existingHud = Object.FindFirstObjectByType<InfoHudUIDocument>();
        if (existingHud == null)
        {
            Debug.Log("[SceneUISetup] Adding InfoHud to Game scene...");
            
            GameObject hudGO = new GameObject("InfoHud");
            SceneManager.MoveGameObjectToScene(hudGO, scene);
            
            UIDocument uiDoc = hudGO.AddComponent<UIDocument>();
            
            PanelSettings panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(
                "Assets/UI Toolkit/PanelSettings.asset");
            if (panelSettings != null)
            {
                uiDoc.panelSettings = panelSettings;
            }
            
            VisualTreeAsset uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI/InfoHud.uxml");
            if (uxml != null)
            {
                uiDoc.visualTreeAsset = uxml;
            }
            
            InfoHudUIDocument hudDoc = hudGO.AddComponent<InfoHudUIDocument>();
            
            SerializedObject so = new SerializedObject(hudDoc);
            if (uxml != null)
            {
                SerializedProperty uxmlProp = so.FindProperty("_uxml");
                if (uxmlProp != null) uxmlProp.objectReferenceValue = uxml;
            }
            
            StyleSheet uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/UI/InfoHud.uss");
            if (uss != null)
            {
                SerializedProperty ussProp = so.FindProperty("_uss");
                if (ussProp != null) ussProp.objectReferenceValue = uss;
            }
            
            so.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(scene);
            
            Debug.Log("[SceneUISetup] InfoHud added!");
        }
    }
    
    [MenuItem("Tools/Fix UI in Current Scene")]
    static void FixCurrentSceneUI()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        OnSceneOpened(activeScene, OpenSceneMode.Single);
    }
}

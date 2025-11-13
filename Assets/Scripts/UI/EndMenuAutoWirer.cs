using UnityEngine;
using UnityEngine.UIElements;

public class EndMenuAutoWirer : MonoBehaviour
{
    [SerializeField] bool verbose = true;

    void Awake()
    {
        SetupEndMenuUI();
    }

    void SetupEndMenuUI()
    {
        var go = GameObject.Find("EndMenuUI");
        if (go == null)
        {
            if (verbose) Debug.Log("[EndMenuAutoWirer] EndMenuUI not found");
            return;
        }

        var doc = go.GetComponent<UIDocument>();
        if (doc == null)
        {
            doc = go.AddComponent<UIDocument>();
            if (verbose) Debug.Log("[EndMenuAutoWirer] Added UIDocument");
        }

        var uxml = Resources.Load<VisualTreeAsset>("UI/EndMenu");
#if UNITY_EDITOR
        if (uxml == null) uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/EndMenu.uxml");
        if (uxml == null) uxml = UnityEditor.AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Resources/UI/EndMenu.uxml");
#endif
        if (uxml != null)
        {
            doc.visualTreeAsset = uxml;
            if (verbose) Debug.Log("[EndMenuAutoWirer] Assigned UXML");
        }

        var uss = Resources.Load<StyleSheet>("UI/EndMenu");
#if UNITY_EDITOR
        if (uss == null) uss = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/EndMenu.uss");
        if (uss == null) uss = UnityEditor.AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/UI/EndMenu.uss");
#endif

        if (doc.panelSettings == null)
        {
#if UNITY_EDITOR
            var ps = UnityEditor.AssetDatabase.LoadAssetAtPath<PanelSettings>("Assets/UI Toolkit/PanelSettings.asset");
            if (ps != null) doc.panelSettings = ps;
#endif
            if (doc.panelSettings == null)
            {
                var arr = Resources.FindObjectsOfTypeAll<PanelSettings>();
                if (arr.Length > 0) doc.panelSettings = arr[0];
            }
        }

        var driver = go.GetComponent<EndMenuUIDocument>();
        if (driver == null)
        {
            driver = go.AddComponent<EndMenuUIDocument>();
            if (verbose) Debug.Log("[EndMenuAutoWirer] Added driver");
        }

        if (driver != null)
        {
            SetField(driver, "_uxml", uxml);
            SetField(driver, "_uss", uss);
        }

        if (verbose) Debug.Log("[EndMenuAutoWirer] End Menu UI wired");
    }

    void SetField(object target, string name, object value)
    {
        if (target == null) return;
        var f = target.GetType().GetField(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        if (f != null) f.SetValue(target, value);
    }
}

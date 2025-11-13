using UnityEngine;

[ExecuteAlways]
public class UpgradeGlobalShaderDriver : MonoBehaviour
{
    [SerializeField] private string floatProperty = "_UpgradeLevel";
    [SerializeField] private string colorProperty = "_UpgradeColor";
    [SerializeField] private Gradient colorByLevel = new Gradient();

    void Update()
    {
        if (UpgradeManager.Instance == null) return;

        float defMax = Mathf.Max(1, UpgradeManager.Instance.GetDefenderMaxLevel());
        float def = Mathf.Clamp01(UpgradeManager.Instance.GetDefenderLevel() / defMax);

        Shader.SetGlobalFloat(floatProperty, def);
        if (!string.IsNullOrEmpty(colorProperty))
        {
            Shader.SetGlobalColor(colorProperty, colorByLevel.Evaluate(def));
        }
    }
}

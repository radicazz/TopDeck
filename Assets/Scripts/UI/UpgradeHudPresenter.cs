using UnityEngine;

#pragma warning disable 0414

/// <summary>
/// Placeholder for the legacy HUD presenter. New HUD logic lives in InfoHudUIDocument,
/// so this component simply ensures the old Canvas stops throwing missing-script errors.
/// </summary>
public sealed class UpgradeHudPresenter : MonoBehaviour
{
    void Awake()
    {
        if (Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}

#pragma warning restore 0414

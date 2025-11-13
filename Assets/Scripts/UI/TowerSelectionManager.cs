using UnityEngine;

#pragma warning disable 0414

/// <summary>
/// Retained as a noop component so scenes saved before the UI Toolkit migration continue to load.
/// </summary>
public sealed class TowerSelectionManager : MonoBehaviour
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

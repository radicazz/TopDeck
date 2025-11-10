using UnityEngine;

/// <summary>
/// Applies a material tint to the spawned enemy for visual differentiation of variants.
/// </summary>
public class EnemyVariantAppearance : MonoBehaviour
{
    [SerializeField] private Renderer rendererRef;
    [SerializeField] private Color tint = Color.white;

    public void Initialize(Color color)
    {
        tint = color;
        Apply();
    }

    void Apply()
    {
        if (rendererRef == null)
        {
            rendererRef = GetComponentInChildren<Renderer>();
        }
        if (rendererRef != null)
        {
            if (rendererRef.material != null)
            {
                rendererRef.material.color = tint;
            }
        }
    }
}


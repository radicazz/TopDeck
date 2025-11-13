using UnityEngine;

/// <summary>
/// Applies a material tint to the spawned enemy for visual differentiation of variants.
/// </summary>
public class EnemyVariantAppearance : MonoBehaviour
{
    private static readonly string[] ColorPropertyNames = new[] { "_BaseColor", "_Color", "_TintColor" };

    [SerializeField] private Renderer[] renderers;
    [SerializeField] private Color tint = Color.white;

    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        CacheRenderers();
        EnsureBlock();
    }

    public void Initialize(Color color)
    {
        tint = color;
        Apply();
    }

    void CacheRenderers()
    {
        if (renderers != null && renderers.Length > 0)
        {
            return;
        }

        renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
    }

    void EnsureBlock()
    {
        if (propertyBlock == null)
        {
            propertyBlock = new MaterialPropertyBlock();
        }
    }

    void Apply()
    {
        CacheRenderers();
        EnsureBlock();

        if (renderers == null)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            if (renderer == null || renderer.sharedMaterial == null)
            {
                continue;
            }

            renderer.GetPropertyBlock(propertyBlock);
            bool applied = false;

            foreach (string prop in ColorPropertyNames)
            {
                if (renderer.sharedMaterial.HasProperty(prop))
                {
                    propertyBlock.SetColor(prop, tint);
                    renderer.SetPropertyBlock(propertyBlock);
                    applied = true;
                    break;
                }
            }

            if (!applied)
            {
                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null)
                    {
                        continue;
                    }
                    materials[i].color = tint;
                }
                renderer.materials = materials;
            }
        }
    }
}

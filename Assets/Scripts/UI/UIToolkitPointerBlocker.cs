using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Tracks the mouse position relative to a UI Toolkit document so gameplay input can be blocked
/// while the pointer is hovering the HUD.
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class UIToolkitPointerBlocker : MonoBehaviour
{
    private static readonly List<UIToolkitPointerBlocker> s_ActiveBlockers = new List<UIToolkitPointerBlocker>();

    [SerializeField] private bool blockWhenHidden = false;

    private UIDocument _document;
    private VisualElement _root;
    private bool _isPointerInside;
    private bool _isPanelVisible;

    public bool BlockWhenHidden
    {
        get { return blockWhenHidden; }
        set { blockWhenHidden = value; }
    }

    void Awake()
    {
        EnsureDocumentReferences();
    }

    void OnEnable()
    {
        EnsureDocumentReferences();
        if (!s_ActiveBlockers.Contains(this))
        {
            s_ActiveBlockers.Add(this);
        }
    }

    void OnDisable()
    {
        s_ActiveBlockers.Remove(this);
        _isPointerInside = false;
        _isPanelVisible = false;
    }

    void Update()
    {
        EnsureDocumentReferences();

        if (_root == null || _root.panel == null)
        {
            _isPointerInside = false;
            _isPanelVisible = false;
            return;
        }

        _isPanelVisible = _root.resolvedStyle.display != DisplayStyle.None;

        if (!_isPanelVisible && !blockWhenHidden)
        {
            _isPointerInside = false;
            return;
        }

        Vector2 screenPos = Input.mousePosition;
        Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(_root.panel, screenPos);
        _isPointerInside = _root.worldBound.Contains(panelPos);
    }

    public static bool IsPointerBlocking()
    {
        for (int i = s_ActiveBlockers.Count - 1; i >= 0; i--)
        {
            UIToolkitPointerBlocker blocker = s_ActiveBlockers[i];
            if (blocker == null)
            {
                s_ActiveBlockers.RemoveAt(i);
                continue;
            }

            if (!blocker._isPanelVisible && !blocker.blockWhenHidden)
            {
                continue;
            }

            if (blocker._isPointerInside)
            {
                return true;
            }
        }

        return false;
    }

    void EnsureDocumentReferences()
    {
        if (_document == null)
        {
            _document = GetComponent<UIDocument>();
        }

        if (_document != null && _root == null)
        {
            _root = _document.rootVisualElement;
        }
    }
}

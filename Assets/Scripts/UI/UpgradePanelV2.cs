using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0414

/// <summary>
/// Legacy Canvas upgrade panel placeholder. The live upgrade UI is the Toolkit-based
/// UpgradePanelUIDocument, so this component simply keeps the serialized data intact and
/// hides the old panel at runtime to avoid duplicate UI.
/// </summary>
[DisallowMultipleComponent]
public sealed class UpgradePanelV2 : MonoBehaviour
{
    [Header("Legacy References")]
    [SerializeField] private UpgradeShop _upgradeShop;
    [SerializeField] private CanvasGroup _panelCanvasGroup;
    [SerializeField] private Image _panelBackground;
    [SerializeField] private GameObject _towerCard;
    [SerializeField] private Object _towerTitle;
    [SerializeField] private Object _towerLevel;
    [SerializeField] private Object _towerDescription;
    [SerializeField] private Button _towerUpgradeButton;
    [SerializeField] private Image _towerIcon;
    [SerializeField] private Object _towerProgressBar;
    [SerializeField] private GameObject _defenderCard;
    [SerializeField] private Object _defenderTitle;
    [SerializeField] private Object _defenderLevel;
    [SerializeField] private Object _defenderDescription;
    [SerializeField] private Button _defenderUpgradeButton;
    [SerializeField] private Image _defenderIcon;
    [SerializeField] private Object _defenderProgressBar;
    [SerializeField] private Object _statusText;
    [SerializeField] private Image _statusIcon;

    [Header("Behaviour Flags")]
    [SerializeField, Min(0f)] private float _statusDisplayDuration = 2.5f;
    [SerializeField] private bool _enableCardAnimations = true;
    [SerializeField, Min(0.1f)] private float _cardAnimationSpeed = 8f;
    [SerializeField, Min(0.1f)] private float _fadeSpeed = 5f;

    [Header("Messages")]
    [SerializeField] private string _readyMessage = "Select a tower to view upgrades";
    [SerializeField] private string _combatLockedMessage = "Upgrades locked during combat";
    [SerializeField] private string _upgradeSuccessMessage = "Upgrade purchased!";
    [SerializeField] private string _insufficientFundsMessage = "Insufficient funds";
    [SerializeField] private string _maxLevelMessage = "Maximum level reached";
    [SerializeField] private string[] _defenderDescriptions = System.Array.Empty<string>();
    [SerializeField] private string[] _towerDescriptions = System.Array.Empty<string>();

    void Awake()
    {
        if (Application.isPlaying)
        {
            // Hide the legacy panel so only the Toolkit UI remains visible
            gameObject.SetActive(false);
        }
    }
#pragma warning restore 0414
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Drives the HUD upgrade panel (buttons, cost labels, status copy) and talks to UpgradeShop.
/// </summary>
[DisallowMultipleComponent]
public class UpgradeHudPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UpgradeShop upgradeShop;
    [SerializeField] private Button defenderButton;
    [SerializeField] private Button towerButton;
    [SerializeField] private TextMeshProUGUI defenderLabel;
    [SerializeField] private TextMeshProUGUI defenderCostLabel;
    [SerializeField] private TextMeshProUGUI towerLabel;
    [SerializeField] private TextMeshProUGUI towerCostLabel;
    [SerializeField] private TextMeshProUGUI statusLabel;

    [Header("Formatting")]
    [SerializeField] private string defenderLabelFormat = "Defenders L{0}/{1}";
    [SerializeField] private string towerLabelFormat = "Tower L{0}/{1}";
    [SerializeField] private string costFormat = "${0}";
    [SerializeField] private string maxedText = "MAX";

    [Header("Status Messages")]
    [SerializeField] private string readyMessage = "Spend cash during prep to upgrade.";
    [SerializeField] private string combatLockedMessage = "Upgrades unavailable during combat.";
    [SerializeField] private string offlineMessage = "Upgrade systems offline.";
    [SerializeField] private string defenderPurchasedMessage = "Defenders upgraded!";
    [SerializeField] private string towerPurchasedMessage = "Tower reinforced!";

    [Header("Visuals")]
    [SerializeField] private Color readyColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(1f, 0.55f, 0.35f, 1f);
    [SerializeField] private float statusMessageDuration = 2.5f;

    private float statusTimer;
    private bool eventsRegistered;

    void Awake()
    {
        if (upgradeShop == null)
        {
            upgradeShop = GetComponent<UpgradeShop>();
        }
    }

    void OnEnable()
    {
        RegisterEvents();
    }

    void OnDisable()
    {
        UnregisterEvents();
    }

    void Update()
    {
        bool systemsReady = upgradeShop != null && UpgradeManager.Instance != null && GameController.Instance != null;
        if (!systemsReady)
        {
            if (defenderButton != null) defenderButton.interactable = false;
            if (towerButton != null) towerButton.interactable = false;
            if (statusTimer <= 0f)
            {
                DisplayStatus(offlineMessage, lockedColor, 0f);
            }
            return;
        }

        bool inPreparation = GameController.Instance.currentPhase == GameController.GamePhase.Preparation;
        bool defenderMaxed = !UpgradeManager.Instance.CanUpgradeDefender();
        bool towerMaxed = !UpgradeManager.Instance.CanUpgradeTower();
        bool defenderAffordable = GameController.Instance.money >= upgradeShop.DefenderUpgradeCost;
        bool towerAffordable = GameController.Instance.money >= upgradeShop.TowerUpgradeCost;

        UpdateButtonUI(
            defenderButton,
            defenderLabel,
            defenderCostLabel,
            defenderLabelFormat,
            UpgradeManager.Instance.GetDefenderLevel(),
            UpgradeManager.Instance.GetDefenderMaxLevel(),
            upgradeShop.DefenderUpgradeCost,
            defenderMaxed,
            inPreparation,
            defenderAffordable
        );

        UpdateButtonUI(
            towerButton,
            towerLabel,
            towerCostLabel,
            towerLabelFormat,
            UpgradeManager.Instance.GetTowerLevel(),
            UpgradeManager.Instance.GetTowerMaxLevel(),
            upgradeShop.TowerUpgradeCost,
            towerMaxed,
            inPreparation,
            towerAffordable
        );

        if (statusTimer > 0f)
        {
            statusTimer -= Time.deltaTime;
        }

        if (statusTimer <= 0f)
        {
            string idleMessage = inPreparation ? readyMessage : combatLockedMessage;
            Color idleColor = inPreparation ? readyColor : lockedColor;
            DisplayStatus(idleMessage, idleColor, 0f);
        }
    }

    public void RequestDefenderUpgrade()
    {
        if (!CanInteractWithShop())
        {
            return;
        }

        if (!UpgradeManager.Instance.CanUpgradeDefender())
        {
            DisplayStatus("Defenders already maxed.", lockedColor, statusMessageDuration);
            return;
        }

        if (!HasEnoughFunds(upgradeShop.DefenderUpgradeCost))
        {
            DisplayStatus($"Need ${upgradeShop.DefenderUpgradeCost} for defenders.", lockedColor, statusMessageDuration);
            return;
        }

        if (upgradeShop.TryPurchaseDefenderUpgrade())
        {
            DisplayStatus(defenderPurchasedMessage, readyColor, statusMessageDuration);
        }
    }

    public void RequestTowerUpgrade()
    {
        if (!CanInteractWithShop())
        {
            return;
        }

        if (!UpgradeManager.Instance.CanUpgradeTower())
        {
            DisplayStatus("Tower already maxed.", lockedColor, statusMessageDuration);
            return;
        }

        if (!HasEnoughFunds(upgradeShop.TowerUpgradeCost))
        {
            DisplayStatus($"Need ${upgradeShop.TowerUpgradeCost} for tower.", lockedColor, statusMessageDuration);
            return;
        }

        if (upgradeShop.TryPurchaseTowerUpgrade())
        {
            DisplayStatus(towerPurchasedMessage, readyColor, statusMessageDuration);
        }
    }

    void RegisterEvents()
    {
        if (eventsRegistered || upgradeShop == null)
        {
            return;
        }

        upgradeShop.DefenderUpgradePurchased.AddListener(HandleDefenderPurchased);
        upgradeShop.TowerUpgradePurchased.AddListener(HandleTowerPurchased);
        eventsRegistered = true;
    }

    void UnregisterEvents()
    {
        if (!eventsRegistered || upgradeShop == null)
        {
            return;
        }

        upgradeShop.DefenderUpgradePurchased.RemoveListener(HandleDefenderPurchased);
        upgradeShop.TowerUpgradePurchased.RemoveListener(HandleTowerPurchased);
        eventsRegistered = false;
    }

    void HandleDefenderPurchased()
    {
        DisplayStatus(defenderPurchasedMessage, readyColor, statusMessageDuration);
    }

    void HandleTowerPurchased()
    {
        DisplayStatus(towerPurchasedMessage, readyColor, statusMessageDuration);
    }

    bool CanInteractWithShop()
    {
        if (upgradeShop == null || UpgradeManager.Instance == null || GameController.Instance == null)
        {
            DisplayStatus(offlineMessage, lockedColor, statusMessageDuration);
            return false;
        }

        if (GameController.Instance.currentPhase != GameController.GamePhase.Preparation)
        {
            DisplayStatus(combatLockedMessage, lockedColor, statusMessageDuration);
            return false;
        }

        return true;
    }

    bool HasEnoughFunds(int cost)
    {
        return GameController.Instance != null && GameController.Instance.money >= cost;
    }

    void UpdateButtonUI(
        Button button,
        TextMeshProUGUI title,
        TextMeshProUGUI costLabel,
        string labelFormat,
        int level,
        int maxLevel,
        int cost,
        bool isMaxed,
        bool phaseAllows,
        bool canAfford
    )
    {
        int clampedLevel = Mathf.Clamp(level, 0, Mathf.Max(1, maxLevel));
        int clampedMax = Mathf.Max(1, maxLevel);

        if (title != null)
        {
            title.text = string.Format(labelFormat, clampedLevel, clampedMax);
        }

        if (costLabel != null)
        {
            if (isMaxed)
            {
                costLabel.text = maxedText;
                costLabel.color = readyColor;
            }
            else
            {
                costLabel.text = string.Format(costFormat, cost);
                costLabel.color = (phaseAllows && canAfford) ? readyColor : lockedColor;
            }
        }

        if (button != null)
        {
            button.interactable = !isMaxed && phaseAllows && canAfford;
        }
    }

    void DisplayStatus(string message, Color color, float holdDuration)
    {
        if (statusLabel == null)
        {
            return;
        }

        statusLabel.text = message;
        statusLabel.color = color;

        if (holdDuration > 0f)
        {
            statusTimer = holdDuration;
        }
        else if (holdDuration == 0f)
        {
            statusTimer = 0f;
        }
    }
}

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles purchasing defender/tower upgrades via currency or debug hotkeys.
/// Hook UI buttons to the public methods to trigger upgrades from the HUD.
/// </summary>
public class UpgradeShop : MonoBehaviour
{
    [Header("Costs")]
    [SerializeField] private int defenderUpgradeCost = 200;
    [SerializeField] private int towerUpgradeCost = 300;

    [Header("Debug Hotkeys")]
    [SerializeField] private bool enableHotkeys = false;
    [SerializeField] private KeyCode defenderUpgradeKey = KeyCode.Z;
    [SerializeField] private KeyCode towerUpgradeKey = KeyCode.X;

    [Header("Events")]
    [SerializeField] private UnityEvent onDefenderUpgradePurchased;
    [SerializeField] private UnityEvent onTowerUpgradePurchased;

    public int DefenderUpgradeCost => Mathf.Max(0, defenderUpgradeCost);
    public int TowerUpgradeCost => Mathf.Max(0, towerUpgradeCost);

    public UnityEvent DefenderUpgradePurchased
    {
        get
        {
            EnsureEvents();
            return onDefenderUpgradePurchased;
        }
    }

    public UnityEvent TowerUpgradePurchased
    {
        get
        {
            EnsureEvents();
            return onTowerUpgradePurchased;
        }
    }

    void Awake()
    {
        EnsureEvents();
    }

    void Update()
    {
        if (!enableHotkeys)
        {
            return;
        }

        if (Input.GetKeyDown(defenderUpgradeKey))
        {
            TryPurchaseDefenderUpgrade();
        }

        if (Input.GetKeyDown(towerUpgradeKey))
        {
            TryPurchaseTowerUpgrade();
        }
    }

    public bool TryPurchaseDefenderUpgrade()
    {
        if (UpgradeManager.Instance == null || GameController.Instance == null)
        {
            Debug.LogWarning("UpgradeShop missing references (UpgradeManager/GameController).");
            return false;
        }

        if (!UpgradeManager.Instance.CanUpgradeDefender())
        {
            Debug.Log("Defender upgrades are at max level.");
            return false;
        }

        if (!GameController.Instance.TrySpendMoney(DefenderUpgradeCost))
        {
            Debug.Log($"Not enough money for defender upgrade. Need ${DefenderUpgradeCost}.");
            return false;
        }

        UpgradeManager.Instance.UpgradeDefenders();
        onDefenderUpgradePurchased?.Invoke();
        return true;
    }

    public bool TryPurchaseTowerUpgrade()
    {
        if (UpgradeManager.Instance == null || GameController.Instance == null)
        {
            Debug.LogWarning("UpgradeShop missing references (UpgradeManager/GameController).");
            return false;
        }

        if (!UpgradeManager.Instance.CanUpgradeTower())
        {
            Debug.Log("Tower upgrades are at max level.");
            return false;
        }

        if (!GameController.Instance.TrySpendMoney(TowerUpgradeCost))
        {
            Debug.Log($"Not enough money for tower upgrade. Need ${TowerUpgradeCost}.");
            return false;
        }

        UpgradeManager.Instance.UpgradeTower();
        onTowerUpgradePurchased?.Invoke();
        return true;
    }

    void EnsureEvents()
    {
        if (onDefenderUpgradePurchased == null)
        {
            onDefenderUpgradePurchased = new UnityEvent();
        }

        if (onTowerUpgradePurchased == null)
        {
            onTowerUpgradePurchased = new UnityEvent();
        }
    }
}

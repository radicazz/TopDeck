# Unity Scene Wiring Guide - Part 3 Upgrade System

**⚠️ IMPORTANT: This guide must be executed in Unity Editor using MCP Unity tools**

## Overview
Previous agent created all C# scripts. This guide wires them in the Unity scene using MCP tools.

---

## Step 1: Create Upgrade UI Panel Structure

**Use MCP tools:** `Unity-MCP__GameObject_Create`, `Unity-MCP__GameObject_AddComponent`, `Unity-MCP__GameObject_Modify`

### 1.1 Find Canvas
```
Tool: Unity-MCP__Scene_GetHierarchy
Action: Locate "Canvas" GameObject in Game scene
```

### 1.2 Create Upgrade Panel
```
Parent: Canvas
GameObject: "UpgradePanel"
Components:
  - RectTransform (anchored top-right)
  - CanvasGroup (alpha: 1, interactable: true)
```

### 1.3 Create UI Elements under UpgradePanel

**Defender Button:**
```
Name: "DefenderUpgradeButton"
Components:
  - Button (interactable: true)
  - Image (background)
  - TextMeshProUGUI children:
    - "DefenderLabelText" (text: "Defenders L0/2")
    - "DefenderCostText" (text: "$200")
```

**Tower Button:**
```
Name: "TowerUpgradeButton"
Components:
  - Button (interactable: true)
  - Image (background)
  - TextMeshProUGUI children:
    - "TowerLabelText" (text: "Tower L0/2")
    - "TowerCostText" (text: "$300")
```

**Status Text:**
```
Name: "StatusText"
Component: TextMeshProUGUI
Default text: "Spend cash during prep to upgrade."
Color: White
```

---

## Step 2: Add UpgradeShop Component

**Use MCP tool:** `Unity-MCP__GameObject_AddComponent`

```
GameObject: UpgradePanel (or GameController)
Component: UpgradeShop
Fields:
  - defenderUpgradeCost: 200
  - towerUpgradeCost: 300
  - enableHotkeys: false
  - defenderUpgradeKey: KeyCode.Z
  - towerUpgradeKey: KeyCode.X
```

---

## Step 3: Add UpgradeHudPresenter Component

**Use MCP tool:** `Unity-MCP__GameObject_AddComponent`

```
GameObject: UpgradePanel
Component: UpgradeHudPresenter
Fields:
  - upgradeShop: (reference to UpgradeShop component)
  - defenderButton: (reference to DefenderUpgradeButton)
  - towerButton: (reference to TowerUpgradeButton)
  - defenderLabel: (reference to DefenderLabelText)
  - defenderCostLabel: (reference to DefenderCostText)
  - towerLabel: (reference to TowerLabelText)
  - towerCostLabel: (reference to TowerCostText)
  - statusLabel: (reference to StatusText)
  
  Formatting:
  - defenderLabelFormat: "Defenders L{0}/{1}"
  - towerLabelFormat: "Tower L{0}/{1}"
  - costFormat: "${0}"
  - maxedText: "MAX"
  
  Status Messages:
  - readyMessage: "Spend cash during prep to upgrade."
  - combatLockedMessage: "Upgrades unavailable during combat."
  - offlineMessage: "Upgrade systems offline."
  - defenderPurchasedMessage: "Defenders upgraded!"
  - towerPurchasedMessage: "Tower reinforced!"
  
  Visuals:
  - readyColor: White (1,1,1,1)
  - lockedColor: Orange (1, 0.55, 0.35, 1)
  - statusMessageDuration: 2.5
```

---

## Step 4: Wire Button OnClick Events

**Use MCP tool:** `Unity-MCP__GameObject_Modify`

```
GameObject: DefenderUpgradeButton
Component: Button
OnClick Event:
  - Target: UpgradeHudPresenter component
  - Function: RequestDefenderUpgrade()
```

```
GameObject: TowerUpgradeButton
Component: Button
OnClick Event:
  - Target: UpgradeHudPresenter component
  - Function: RequestTowerUpgrade()
```

---

## Step 5: Wire UpgradeShop UnityEvents to VFX

### 5.1 Add UpgradeVfxSpawner Component

**Use MCP tool:** `Unity-MCP__GameObject_AddComponent`

```
GameObject: Create new "VfxManager" (or add to GameController)
Component: UpgradeVfxSpawner
Fields:
  - defenderUpgradeEffect: (null for now - assign particle prefab later)
  - towerUpgradeEffect: (null for now - assign particle prefab later)
  - defenderEffectAnchor: (reference to tower/defender spawn point)
  - towerEffectAnchor: (reference to tower GameObject transform)
  - parentToAnchor: true
```

### 5.2 Wire UpgradeShop Events

**Use MCP tool:** `Unity-MCP__GameObject_Modify`

```
GameObject: UpgradePanel (wherever UpgradeShop is)
Component: UpgradeShop
Events:
  - onDefenderUpgradePurchased:
    - Add listener → UpgradeVfxSpawner.PlayDefenderUpgrade()
  
  - onTowerUpgradePurchased:
    - Add listener → UpgradeVfxSpawner.PlayTowerUpgrade()
```

---

## Step 6: Add PostProcessPulseController

### 6.1 Create Volume GameObject

**Use MCP tools:** `Unity-MCP__GameObject_Create`, `Unity-MCP__GameObject_AddComponent`

```
Menu: GameObject/Volume/Global Volume
Name: "TowerHealthVolume"
Component: Volume
  - isGlobal: true
  - weight: 0
  - profile: (create new or assign existing post-process profile)
  - priority: 10
```

### 6.2 Add PostProcessPulseController

**Use MCP tool:** `Unity-MCP__GameObject_AddComponent`

```
GameObject: TowerHealthVolume
Component: PostProcessPulseController
Fields:
  - volume: (auto-assigned from same GameObject)
  - fallbackProfile: (reference to Volume profile)
  - intensityByHealth: AnimationCurve (0,1) to (1,0)
  - pulseSpeed: 2.0
```

---

## Step 7: Update Prefab_Defender_Basic

**Already done by previous agent:**
- ✅ DefenderUpgrade component added
- ✅ UpgradeVisualShaderDriver component added

**Verify using MCP:**
```
Tool: Unity-MCP__GameObject_Find
Find: Prefab_Defender_Basic
Check components:
  - DefenderUpgrade (should exist)
  - UpgradeVisualShaderDriver (should exist)
  - Renderer reference assigned in UpgradeVisualShaderDriver
```

---

## Step 8: Verification Checklist

**Use MCP tools:**

### 8.1 Recompile Scripts
```
Tool: recompile_scripts
Verify: No compilation errors
```

### 8.2 Check Console Logs
```
Tool: Unity-MCP__Console_GetLogs
Verify: No errors related to upgrades/UI
```

### 8.3 Verify Scene Hierarchy
```
Tool: Unity-MCP__Scene_GetHierarchy
Verify structure:
  - Canvas
    - UpgradePanel
      - DefenderUpgradeButton
      - TowerUpgradeButton
      - StatusText
  - TowerHealthVolume (or VfxManager location)
```

### 8.4 Check Component References
```
Tool: Unity-MCP__GameObject_Find
Verify UpgradeHudPresenter has all serialized fields assigned
```

### 8.5 Validate Prefabs
```
Tool: Unity-MCP__Assets_Find
Filter: prefabs
Verify: Prefab_Defender_Basic has no missing script references
```

---

## Step 9: Test in Play Mode

**Manual testing (after MCP wiring):**

1. Enter Play Mode
2. Check Console for initialization errors
3. Verify UI appears in top-right
4. Check button states during Preparation phase
5. Test purchasing upgrades (if sufficient money)
6. Verify status messages appear
7. Check buttons disabled during Combat phase

---

## Expected File Structure After Completion

```
Assets/
  Scenes/
    Game.unity (modified with UI + components)
  Prefabs/
    Prefab_Defender_Basic.prefab (already has DefenderUpgrade + ShaderDriver)
  Scripts/
    UI/
      UpgradeHudPresenter.cs ✅
    Upgrades/
      UpgradeShop.cs ✅
      UpgradeManager.cs ✅
      DefenderUpgrade.cs ✅
    Visual/
      UpgradeVfxSpawner.cs ✅
      PostProcessPulseController.cs ✅
      UpgradeVisualShaderDriver.cs ✅
```

---

## Notes for MCP Agent

- All C# scripts already exist and compile
- Main task: Unity Editor scene/prefab wiring
- Use MCP Unity tools exclusively (no manual editing)
- Validate after each major step
- Components should auto-find references where possible (e.g., `GetComponent<>()`)
- Status messages and formatting can be tuned later

---

## Common Issues & Solutions

**Issue:** Button OnClick doesn't show UpgradeHudPresenter methods
**Solution:** Ensure component is on same GameObject or reference is set

**Issue:** UpgradeShop events don't trigger
**Solution:** Check UnityEvent is initialized in Awake (script already handles this)

**Issue:** PostProcessPulseController has no Volume
**Solution:** Component auto-creates Volume in EnsureVolumeReference()

**Issue:** UpgradeVisualShaderDriver can't find renderer
**Solution:** Component auto-searches GetComponent/GetComponentInChildren

---

## Final Validation Command

After all wiring is complete, run:
```bash
./Tools/validate_changes.sh
```

Expected output: All checks passed ✅

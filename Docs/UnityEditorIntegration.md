# Unity Editor Integration Guide

This document outlines the remaining Unity Editor steps to complete the Part 3 implementation.

## 1. Create ProceduralVariantConfig Asset

**Steps:**
1. In Unity Editor: `Assets → Create → TopDeck → Procedural Variant Config`
2. Save as `Assets/Resources/DefaultVariantConfig.asset`
3. Configure in Inspector:
   - **Health Curve**: Default Linear(0,1 → 1,1.2)
   - **Speed Curve**: Default Linear(0,1 → 1,1.15)
   - **Damage Curve**: Default Linear(0,1 → 1,1.15)
   - **Wave Scale Factor**: 0.05
   - **Defender Counter Factor**: 0.03
   - **Tint Gradient**: Create gradient (blue → red for difficulty)
   - **Extreme Threshold**: 0.85

4. Assign to GameController:
   - Select GameController in Game scene
   - Drag DefaultVariantConfig into `Variant Config` field

## 2. Create Variant Telemetry UI

**Hierarchy Setup:**
```
Canvas
  └─ Panel HUD (existing)
      └─ VariantTelemetryPanel (new)
          └─ TelemetryText (TextMeshProUGUI)
```

**Steps:**
1. Create UI Panel under existing HUD
2. Add `CanvasGroup` component (alpha: 0, interactable: false)
3. Position in bottom-left corner
4. Add TextMeshProUGUI child named "TelemetryText"
5. Add `VariantTelemetryPresenter` component to panel
6. Wire references:
   - `Telemetry Label` → TelemetryText
   - `Canvas Group` → CanvasGroup component
   - Format String: "Wave {0} Mutators: {1:+0}% HP, {2:+0}% SPD, {3:+0}% DMG"
   - Display Duration: 3.0

7. Assign to GameController:
   - Drag VariantTelemetryPanel into `Variant Telemetry` field

## 3. Create Materials Using Custom Shaders

**Defender Upgrade Material:**
1. Create material: `Assets/Materials/DefenderUpgradeMaterial.mat`
2. Set shader to `TopDeck/UpgradeColorModulation`
3. Configure:
   - Base Color: Current defender color
   - Upgrade Color: Cyan (0, 1, 1, 1)
   - Pulse Speed: 1.0
   - Pulse Intensity: 0.3
   - Emission Strength: 0.5

**Defender Displacement Material:**
1. Create material: `Assets/Materials/DefenderDisplacementMaterial.mat`
2. Set shader to `TopDeck/UpgradeVertexDisplacement`
3. Configure:
   - Base Color: Current defender color
   - Displacement Amount: 0.1
   - Displacement Frequency: 2.0
   - Displacement Speed: 1.0

## 4. Update Defender Prefab

**Prefab: `Assets/Prefabs/Prefab_Defender_Basic.prefab`**

1. Open prefab in edit mode
2. Locate `UpgradeVisualShaderDriver` component
3. Ensure:
   - `Renderer Ref` points to defender's renderer
   - `Use Property Block` is checked
4. Apply one of the upgrade materials to the renderer

## 5. Verify UpgradeHudPresenter Wiring

**Steps:**
1. Ensure UpgradePanel exists in Game scene Canvas
2. Verify button structure matches UNITY_WIRING_GUIDE.md
3. Check component references are assigned
4. Test button OnClick events fire correctly

## 6. Testing Checklist

**Edit Mode Tests:**
```bash
# Run from command line
Unity -batchmode -projectPath . -runTests -testPlatform EditMode -testResults results.xml -quit
```

**In-Editor Validation:**
1. Enter Play Mode
2. Start a game
3. Verify variant telemetry appears after wave spawns
4. Check Console for variant multiplier logs
5. Purchase upgrades and verify visual shader changes
6. Confirm variant difficulty scales with defender upgrades

## 7. Performance Verification

**Check:**
- No per-frame allocations from variant system
- Shader property updates use MaterialPropertyBlock
- Telemetry UI doesn't stay visible when faded
- Variant generation doesn't cause frame spikes

## 8. Final Polish

**Before Delivery:**
- [ ] Remove/reduce Debug.Log spam
- [ ] Verify all serialized fields are assigned
- [ ] Check no missing references in Console
- [ ] Run `./Tools/validate_changes.sh`
- [ ] Test full gameplay loop (prep → combat → upgrade → repeat)

## Troubleshooting

**Issue: Shaders render pink**
- Solution: Ensure project uses URP pipeline, assign URP asset in Project Settings

**Issue: Telemetry doesn't show**
- Solution: Check GameController has VariantTelemetryPresenter assigned and component is active

**Issue: Materials don't animate**
- Solution: Verify UpgradeVisualShaderDriver is updating _UpgradeLevel property each frame

**Issue: Tests fail to run**
- Solution: Ensure Test Framework package is installed, check assembly definition references

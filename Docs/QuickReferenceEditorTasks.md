# Quick Reference: Remaining Unity Editor Tasks

## Asset Creation (Cannot be done via code)

### 1. ProceduralVariantConfig Asset
**Location:** `Assets/Resources/DefaultVariantConfig.asset`
**How:** Assets → Create → TopDeck → Procedural Variant Config
**Settings:** Use defaults from Docs/UnityEditorIntegration.md
**Wire to:** GameController.variantConfig field

### 2. Materials
**Materials needed:**
- `Assets/Materials/DefenderUpgradeMaterial.mat` (ColorModulation shader)
- `Assets/Materials/DefenderDisplacementMaterial.mat` (VertexDisplacement shader)

**Apply to:** Prefab_Defender_Basic renderer

### 3. UI Panel for Telemetry
**Hierarchy:**
```
Canvas/Panel HUD/
  └─ VariantTelemetryPanel (new GameObject + CanvasGroup)
      └─ TelemetryText (TextMeshProUGUI)
```
**Component:** Add VariantTelemetryPresenter to panel
**Wire to:** GameController.variantTelemetry field

## Component Wiring (Can use Unity MCP tools)

### GameController Setup
- Assign ProceduralVariantConfig asset to `Variant Config`
- Assign VariantTelemetryPresenter to `Variant Telemetry`

### UpgradeHudPresenter Setup
Already scripted but needs scene references:
- Button components
- Text labels
- UpgradeShop reference

See: `UNITY_WIRING_GUIDE.md` for complete button hookup

## Testing Commands

### Edit Mode Tests
```bash
Unity -batchmode -projectPath . -runTests -testPlatform EditMode -quit
```

### Play Mode Tests  
```bash
Unity -batchmode -projectPath . -runTests -testPlatform PlayMode -quit
```

### Validation Script
```bash
./Tools/validate_changes.sh
```

## Verification Checklist

- [ ] No compilation errors in Console
- [ ] ProceduralVariantConfig assigned to GameController
- [ ] Telemetry UI appears during gameplay
- [ ] Variant stats display after wave spawn
- [ ] Shaders render correctly (not pink)
- [ ] Tests pass in Test Runner
- [ ] No missing references warnings

## Files Ready for MCP Wiring

All C# scripts are ready and will auto-generate .meta files on Unity import:
- ProceduralVariantConfig.cs
- VariantTelemetryPresenter.cs
- UpgradeHudPresenter.cs (already existed)
- All test files

Shaders are ready for material creation:
- UpgradeVertexDisplacement.shader
- UpgradeColorModulation.shader

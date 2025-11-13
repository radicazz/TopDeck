# UI System Architecture

## Overview
TopDeck uses **UI Toolkit** (not the old Canvas system) for all in-game UI. The system uses auto-wiring to set up UI components at runtime.

## Auto-Wiring Pattern

### How It Works
1. **UIAutoSetup.cs** - Global runtime initializer that listens for scene loads
2. **SceneAutoWirer.cs** - Game scene specific auto-wirer  
3. **StartMenuAutoWirer.cs** - Start Menu specific auto-wirer

When a scene loads, the appropriate auto-wirer:
- Finds UI GameObjects by name (e.g., "InfoHud", "UpgradePanel", "StartMenuUI")
- Loads UXML/USS assets from `Assets/UI/` or `Assets/Resources/UI/`
- Assigns PanelSettings from `Assets/UI Toolkit/PanelSettings.asset`
- Uses reflection to wire private fields on UI Document scripts

### Key Components

#### InfoHud (Game Scene)
- **GameObject**: `InfoHud` 
- **Script**: `InfoHudUIDocument.cs`
- **UXML**: `Assets/UI/InfoHud.uxml`
- **Purpose**: Left-side HUD showing tower health, enemies, defenders
- **Always visible** during gameplay

#### UpgradePanel (Game Scene)
- **GameObject**: `UpgradePanel`
- **Script**: `UpgradePanelUIDocument.cs`  
- **UXML**: `Assets/UI/UpgradePanel.uxml`
- **Purpose**: Right-side panel for tower upgrades
- **Hidden by default**, shown when tower is selected
- Press ESC to close

#### StartMenu (Start Menu Scene)
- **GameObject**: `StartMenuUI`
- **Script**: `StartMenuUIDocument.cs`
- **UXML**: `Assets/UI/StartMenu.uxml`
- **Purpose**: Main menu with Start Game and Quit buttons

## File Structure
```
Assets/
├── UI/                          # UXML and USS files
│   ├── InfoHud.uxml/uss
│   ├── UpgradePanel.uxml/uss
│   ├── StartMenu.uxml/uss
│   └── TopBanner.uxml/uss
├── UI Toolkit/
│   └── PanelSettings.asset      # Shared PanelSettings (1920x1080)
├── Resources/UI/                # Also checked for UXML/USS
└── Scripts/
    ├── Core/
    │   └── UIAutoSetup.cs       # Global scene load handler
    ├── SceneAutoWirer.cs        # Game scene auto-wirer
    └── UI/
        ├── StartMenuAutoWirer.cs
        ├── InfoHudUIDocument.cs
        ├── UpgradePanelUIDocument.cs
        ├── StartMenuUIDocument.cs
        └── TopBannerUIDocument.cs
```

## Critical Rules

1. **GameObject Names Matter**: Auto-wiring finds objects by exact name
   - `InfoHud` → InfoHudUIDocument
   - `UpgradePanel` → UpgradePanelUIDocument  
   - `StartMenuUI` → StartMenuUIDocument

2. **One PanelSettings Asset**: All UI shares `Assets/UI Toolkit/PanelSettings.asset`
   - DO NOT create separate PanelSettings per scene
   - Resolution: 1920x1080, ScaleWithScreenSize

3. **UXML Paths**: Assets searched in this order:
   - `Resources.Load<VisualTreeAsset>("UI/InfoHud")`
   - `AssetDatabase.LoadAssetAtPath("Assets/UI/InfoHud.uxml")`  
   - `AssetDatabase.LoadAssetAtPath("Assets/Resources/UI/InfoHud.uxml")`

4. **Scene Setup**:
   - Game scene MUST have: `InfoHud`, `UpgradePanel` GameObjects
   - Start Menu MUST have: `StartMenuUI` GameObject
   - Each must have `UIDocument` component (added by auto-wirer if missing)

## Debugging Tips

- Check console for `[SceneAutoWirer]` or `[StartMenuAutoWirer]` messages
- Verify GameObject names match exactly (case-sensitive)
- Ensure PanelSettings is assigned (auto-wirer will find and assign)
- If UI doesn't show, check UXML files exist at expected paths
- Missing scripts? Reimport the scene or restart Unity

## Common Issues

**UI Not Showing**: Check GameObject active state and UXML is assigned
**Wrong Panel Size**: Verify PanelSettings.asset has correct resolution  
**Button Clicks Not Working**: Check UXML element names match script queries
**Scene Changes Break UI**: Don't rename UI GameObjects or PanelSettings asset

## Example: Adding New UI

1. Create UXML in `Assets/UI/MyPanel.uxml`
2. Create script `MyPanelUIDocument.cs` extending MonoBehaviour
3. Add GameObject to scene named `MyPanel`
4. Update appropriate AutoWirer to wire it (see InfoHud example in SceneAutoWirer)
5. Script should cache UIDocument in Awake, setup in OnEnable

```csharp
void OnEnable()
{
    _doc = GetComponent<UIDocument>();
    if (_doc.panelSettings == null)
    {
        var ps = Resources.FindObjectsOfTypeAll<PanelSettings>();
        if (ps.Length > 0) _doc.panelSettings = ps[0];
    }
    if (_uxml != null) _doc.visualTreeAsset = _uxml;
    _root = _doc.rootVisualElement;
    // Cache elements, wire buttons, etc.
}
```

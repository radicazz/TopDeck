# UI Toolkit Setup Guide

## How UI Works in TopDeck

This project uses **UI Toolkit** (not legacy Canvas). All UI is auto-wired at runtime using AutoWirer scripts.

## Key Components

### 1. UI Assets (in `Assets/Resources/UI/` or `Assets/UI/`)
- **UXML files** - UI layout/structure (like HTML)
- **USS files** - UI styling (like CSS)

### 2. PanelSettings Asset
- **Location**: `Assets/UI Toolkit/PanelSettings.asset` (correct one)
- **What it does**: Required for UI Toolkit to render - no PanelSettings = no UI display
- **Note**: There are 3 PanelSettings in the project (2 wrong ones in `Scripts/Defenders/`) - ignore those

### 3. UIDocument Component
- Unity component that displays UI Toolkit UI
- **Must have**:
  - `panelSettings` - reference to PanelSettings asset
  - `visualTreeAsset` - reference to UXML file

### 4. AutoWirer Scripts
- **SceneAutoWirer.cs** - Game scene UI setup
- **StartMenuAutoWirer.cs** - Start Menu scene UI setup
- **UIAutoSetup.cs** - Automatically runs AutoWirers when scenes load

## How It Works (Runtime Flow)

```
1. Scene loads
   ↓
2. UIAutoSetup.OnSceneLoaded() runs (RuntimeInitializeOnLoadMethod)
   ↓
3. Creates AutoWirer GameObject (e.g., _StartMenuAutoWirer)
   ↓
4. AutoWirer.Awake() runs:
   - Finds UI GameObject (e.g., "StartMenuUI")
   - Loads UXML/USS from Resources
   - Assigns to UIDocument component
   - Uses Resources.FindObjectsOfTypeAll<PanelSettings>() to find PanelSettings
   - Assigns everything via reflection
   ↓
5. UI displays
```

## Creating New UI

### For a new scene menu:

1. **Create UXML/USS files** in `Assets/Resources/UI/`
   ```
   Assets/Resources/UI/MyMenu.uxml
   Assets/Resources/UI/MyMenu.uss
   ```

2. **Create UIDocument script** (e.g., `MyMenuUIDocument.cs`):
   ```csharp
   public class MyMenuUIDocument : MonoBehaviour
   {
       [SerializeField] private VisualTreeAsset _uxml;
       [SerializeField] private StyleSheet _uss;
       
       void OnEnable()
       {
           var uiDoc = GetComponent<UIDocument>();
           _root = uiDoc.rootVisualElement;
           // Cache elements and wire up buttons
       }
   }
   ```

3. **Create AutoWirer** (e.g., `MyMenuAutoWirer.cs`):
   - Copy `StartMenuAutoWirer.cs` as template
   - Update asset paths to your UXML/USS files
   - Update GameObject name to find

4. **Update UIAutoSetup.cs**:
   ```csharp
   static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
   {
       if (scene.name == "MyScene")
       {
           SetupMyMenu(scene);
       }
   }
   ```

5. **In scene**, create GameObject with:
   - UIDocument component
   - Your UIDocument script component

## Common Issues

### UI Not Showing
**Check in this order:**
1. Is PanelSettings assigned on UIDocument? (Check in Play mode, not Edit mode)
2. Is UXML assigned to UIDocument.visualTreeAsset?
3. Check Console for AutoWirer logs - should see "Assigned StartMenu.uxml to UIDocument"
4. Is the GameObject active?

### PanelSettings is null
- AutoWirer uses `Resources.FindObjectsOfTypeAll<PanelSettings>()` at runtime
- This finds ANY PanelSettings in the project
- As long as one exists, it will work
- **Don't reference PanelSettings in scene files** - Unity doesn't handle the GUID well

### UXML Not Found
- Must be in `Resources/UI/` folder OR loaded via AssetDatabase in editor
- AutoWirer tries both:
  ```csharp
  Resources.Load<VisualTreeAsset>("UI/MyMenu")
  AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/MyMenu.uxml") // Editor only
  ```

## Scene File Notes

- **Don't manually edit** scene YAML for UIDocument references
- Unity will revert PanelSettings GUIDs on save
- Let AutoWirers handle everything at runtime
- Scene should only have empty UIDocument component - AutoWirer fills it in

## Pattern: Follow SceneAutoWirer

`SceneAutoWirer.cs` (lines 230-310) shows the correct pattern:
1. Find GameObject
2. Get/Add UIDocument component  
3. Load UXML via Resources or AssetDatabase
4. Load USS via Resources or AssetDatabase
5. Assign PanelSettings via FindObjectsOfTypeAll
6. Use reflection to set private fields on UIDocument script

**This pattern works - copy it for all new UI.**

## Files to Check
- `Assets/Scripts/SceneAutoWirer.cs` - Game scene UI (working reference)
- `Assets/Scripts/UI/StartMenuAutoWirer.cs` - Start Menu UI
- `Assets/Scripts/Core/UIAutoSetup.cs` - Scene load hooks
- `Assets/UI Toolkit/PanelSettings.asset` - The correct PanelSettings asset

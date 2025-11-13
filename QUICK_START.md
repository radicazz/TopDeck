# TopDeck - Quick Start After Fixes

## 🎮 Game is Now Playable!

The critical issues have been fixed. Here's how to play:

### 1. Open Unity Editor
```bash
# From project root
unity-hub --projectPath /home/abusive/Coding/radicazz/TopDeck
```

### 2. Open Game Scene
- In Project window: `Assets/Scenes/Game.unity`
- Double-click to open

### 3. Press Play ▶️

### What You Should See:
✅ **HUD in top-left corner** showing:
   - Game phase (PREPARATION / COMBAT)
   - Money (yellow text)
   - Timer (cyan text)  
   - Health (green text)

✅ **Procedurally generated map** with:
   - Black paths for enemies
   - White/colored tiles for towers

✅ **Preparation phase** (20 seconds):
   - Click white tiles to place towers ($200 each)
   - Placed towers auto-select and show stats
   - Right-click to deselect

✅ **Combat phase** (after timer):
   - Enemies spawn and follow black paths
   - Towers shoot automatically
   - Money increases when enemies die
   - Health decreases if enemies reach the end

✅ **Wave progression**:
   - After all enemies defeated, new preparation phase
   - Wave number increases
   - More/stronger enemies each wave

---

## Controls

| Action | Input |
|--------|-------|
| Place Tower | Left-click white tile |
| Select Tower | Left-click placed tower |
| Deselect | Right-click anywhere |
| Speed Toggle | Spacebar (1x / 2x) |

---

## If Something Goes Wrong

### UI Not Visible
The game auto-creates HUD at runtime. If it doesn't appear:
1. Check Console (Ctrl+Shift+C) for errors
2. Verify GameController exists in Hierarchy
3. Check `[GameController] Runtime HUD created` message in console

### Enemies Not Spawning
1. Check Console for "Spawning X enemies" messages
2. Verify MapController generated black paths
3. Check that Controller Game has attacker types configured:
   - Select "Controller Game" in Hierarchy
   - Check "Attacker Types" list (should have 2 entries)

### Compilation Errors
```bash
./Tools/validate_changes.sh
```

---

## Optional: Manual HUD Setup (Advanced)

If you want to manually create the HUD instead of using runtime creation:

1. **Create Canvas:**
   - Hierarchy > Right-click > UI > Canvas
   - Name it "GameHUD"
   - Set Render Mode: Screen Space - Overlay

2. **Add TextMeshPro Elements:**
   - Right-click Canvas > UI > Text - TextMeshPro
   - Create 4 text elements:
     - StateText (font size: 28, bold, white)
     - MoneyText (font size: 24, yellow)
     - TimerText (font size: 22, cyan)
     - HealthText (font size: 22, green)

3. **Wire to GameController:**
   - Select "Controller Game" in Hierarchy
   - Drag text elements to matching fields in Inspector

4. **Disable Runtime Creation:**
   - Delete "RuntimeFixer" GameObject if it exists

---

## Testing Checklist

```
□ Game starts without errors
□ HUD appears and updates
□ Map generates with black/colored tiles
□ Preparation timer counts down
□ Can place towers on white tiles
□ Towers show selection indicator
□ Combat phase starts
□ Enemies spawn and move
□ Towers shoot at enemies
□ Health decreases when enemies reach end
□ Money increases when enemies killed
□ Next wave starts after enemies cleared
```

---

## Known Limitations

### Upgrade Panel
The upgrade panel exists but may need manual wiring:
- Select tower to trigger upgrade panel
- If panel doesn't appear, it needs Inspector wiring (see FIXES_APPLIED.md)

### UI Toolkit Conflicts
The project has both Canvas and UI Toolkit systems. The Canvas system is now active. You can safely:
- Disable UIToolkitRoot GameObject if it exists
- Remove Assets/UI/*.uxml files if not using them

---

## Performance Tips

- **Slow performance?** Reduce enemy spawn count:
  - Select "Controller Game"
  - Lower "Base Enemies Per Wave" value

- **Too easy/hard?** Adjust difficulty:
  - Modify "Wave Health Multiplier"
  - Change "Kill Reward" amount
  - Adjust "Starting Money"

---

## Next Steps

1. **Play test** the core gameplay loop
2. **Test upgrades** if upgrade panel is visible
3. **Verify procedural systems** (variants, difficulty scaling)
4. **Polish visuals** (shaders, VFX) if time permits

---

## Support

For issues:
1. Check **FIXES_APPLIED.md** for detailed fix information
2. Run `./Tools/cleanup_deprecated.sh` to check for issues
3. Check Unity Console for error messages
4. Verify scene has required GameObjects:
   - Controller Game (GameController)
   - UpgradeManager
   - Controller Map (MapController)
   - Camera, Light

---

**Ready to play! Press Play in Unity and enjoy!** 🎮

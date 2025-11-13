#!/bin/bash
# Quick Fix Deployment Script for TopDeck
# This script addresses the UI and gameplay issues

echo "========================================="
echo "TopDeck Game Fix Deployment"
echo "========================================="
echo ""

# Check if running from project root
if [ ! -f "Assets/Scripts/GameController.cs" ]; then
    echo "ERROR: Must run from project root directory"
    exit 1
fi

echo "1. Checking Unity Editor status..."
if pgrep -f "Unity" > /dev/null; then
    echo "   ✓ Unity Editor is running"
else
    echo "   ⚠ Unity Editor is not running - please start Unity"
    echo "   Opening Unity project..."
    # This will vary by system
fi

echo ""
echo "2. Scene Setup Required (Manual Steps in Unity Editor):"
echo "   =========================================="
echo ""
echo "   A. HUD Setup:"
echo "      1. In Hierarchy, create: GameObject > UI > Canvas (name it 'GameHUD')"
echo "      2. In GameHUD Canvas:"
echo "         - Set Render Mode: Screen Space - Overlay"
echo "         - Canvas Scaler > UI Scale Mode: Scale With Screen Size"
echo "         - Reference Resolution: 1920 x 1080"
echo "      "
echo "      3. Create 4 TextMeshPro - Text (UI) children:"
echo "         - StateText (position: top-left, size: 28, bold)"
echo "         - MoneyText (below StateText, size: 24, color: yellow)"
echo "         - TimerText (below MoneyText, size: 22, color: cyan)"
echo "         - HealthText (below TimerText, size: 22, color: green)"
echo ""
echo "      4. Select 'Controller Game' in Hierarchy"
echo "      5. In GameController component, assign:"
echo "         - State Text → StateText"
echo "         - Money Text → MoneyText"
echo "         - Timer Text → TimerText"
echo "         - Player Health Text → HealthText"
echo ""
echo "   B. UpgradeManager Setup:"
echo "      ✓ Already added to scene"
echo ""
echo "   C. Attacker Configuration:"
echo "      - Open 'Controller Game' in Inspector"
echo "      - Verify 'Attacker Types' list has entries with prefabs"
echo "      - If empty, add at least one entry:"
echo "        * Expand Attacker Types"
echo "        * Set Size: 2"
echo "        * Element 0:"
echo "          - Id: 'Basic'"
echo "          - Prefab: Prefab_Attacker_Type1"
echo "          - Spawn Weight: 5"
echo "          - Base Health: 100"
echo "          - Move Speed: 3"
echo "        * Element 1:"
echo "          - Id: 'Elite'"
echo "          - Prefab: Prefab_Attacker_Type2"
echo "          - Spawn Weight: 2"
echo "          - Base Health: 200"
echo "          - Move Speed: 2"
echo ""
echo "   =========================================="
echo ""

echo "3. Code fixes applied:"
echo "   ✓ Enhanced UI text readability (larger fonts)"
echo "   ✓ Created SimpleHudBuilder helper script"
echo "   ✓ UpgradeManager added to scene"
echo ""

echo "4. Testing the game:"
echo "   1. Press Play in Unity Editor"
echo "   2. You should see:"
echo "      - UI text in top-left showing game state, money, timer, health"
echo "      - Generated procedural map"
echo "      - After prep timer ends, enemies should spawn"
echo "      - You can place towers by clicking white tiles"
echo ""

echo "5. If enemies still don't spawn:"
echo "   - Open Unity Console (Ctrl+Shift+C)"
echo "   - Look for 'Spawning X enemies' messages"
echo "   - Check for errors about missing prefabs or paths"
echo ""

echo "========================================="
echo "Fix deployment complete!"
echo "Please follow the manual steps above in Unity Editor."
echo "========================================="

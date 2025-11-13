#!/bin/bash
# Cleanup Script for TopDeck - Removes Deprecated/Obsolete Components

echo "========================================="
echo "TopDeck Deprecated Code Cleanup"
echo "========================================="
echo ""

PROJECT_ROOT="/home/abusive/Coding/radicazz/TopDeck"

cd "$PROJECT_ROOT" || exit 1

echo "Checking for deprecated code patterns..."
echo ""

# Check for obsolete Unity API usage
echo "1. Checking for deprecated Unity APIs..."
DEPRECATED_APIS=()

# FindObjectOfType vs FindFirstObjectByType
if grep -r "FindObjectOfType<" Assets/Scripts --include="*.cs" > /dev/null 2>&1; then
    DEPRECATED_APIS+=("FindObjectOfType (should use FindFirstObjectByType)")
fi

# FindGameObjectWithTag vs FindWithTag  
if grep -r "FindGameObjectWithTag" Assets/Scripts --include="*.cs" > /dev/null 2>&1; then
    DEPRECATED_APIS+=("FindGameObjectWithTag (should use GameObject.FindWithTag)")
fi

if [ ${#DEPRECATED_APIS[@]} -eq 0 ]; then
    echo "   ✓ No deprecated Unity APIs found"
else
    echo "   ⚠ Found deprecated APIs:"
    for api in "${DEPRECATED_APIS[@]}"; do
        echo "      - $api"
    done
fi

echo ""

# Check for unused UI systems
echo "2. Checking for conflicting/duplicate UI systems..."

UI_TOOLKIT_FILES=$(find Assets/UI -name "*.uxml" -o -name "*.uss" 2>/dev/null | wc -l)
UI_TOOLKIT_SCRIPTS=$(find Assets/Scripts/UI -name "*UIDocument.cs" 2>/dev/null | wc -l)

if [ "$UI_TOOLKIT_FILES" -gt 0 ] || [ "$UI_TOOLKIT_SCRIPTS" -gt 0 ]; then
    echo "   ⚠ UI Toolkit files found: $UI_TOOLKIT_FILES UXML/USS, $UI_TOOLKIT_SCRIPTS scripts"
    echo "   → Consider removing if using Canvas-based HUD only"
    echo ""
    echo "   Files that could be removed:"
    find Assets/UI -name "*.uxml" -o -name "*.uss" 2>/dev/null | sed 's/^/      - /'
    find Assets/Scripts/UI -name "*UIDocument.cs" 2>/dev/null | sed 's/^/      - /'
else
    echo "   ✓ No UI Toolkit conflicts"
fi

echo ""

# Check for missing component references in prefabs
echo "3. Checking prefab health..."

PREFABS_WITH_ISSUES=()
for prefab in Assets/Prefabs/Prefab_Attacker_*.prefab; do
    if [ -f "$prefab" ]; then
        if grep -q "m_Script: {fileID: 0}" "$prefab" 2>/dev/null; then
            PREFABS_WITH_ISSUES+=("$(basename "$prefab")")
        fi
    fi
done

if [ ${#PREFABS_WITH_ISSUES[@]} -eq 0 ]; then
    echo "   ✓ All prefabs healthy"
else
    echo "   ⚠ Prefabs with missing script references:"
    for prefab in "${PREFABS_WITH_ISSUES[@]}"; do
        echo "      - $prefab"
    done
    echo "   → Open in Unity Editor and remove missing components"
fi

echo ""

# Check for obsolete scripts
echo "4. Checking for potentially obsolete scripts..."

OBSOLETE_SCRIPTS=()

# Scripts marked with [Obsolete] or TODO remove comments
while IFS= read -r -d '' file; do
    if grep -q "System.Obsolete\|TODO.*remove\|DEPRECATED" "$file" 2>/dev/null; then
        OBSOLETE_SCRIPTS+=("$(basename "$file")")
    fi
done < <(find Assets/Scripts -name "*.cs" -print0 2>/dev/null)

if [ ${#OBSOLETE_SCRIPTS[@]} -eq 0 ]; then
    echo "   ✓ No scripts marked obsolete"
else
    echo "   ⚠ Scripts marked for removal/review:"
    for script in "${OBSOLETE_SCRIPTS[@]}"; do
        echo "      - $script"
    done
fi

echo ""

# Check for empty/unused directories
echo "5. Checking for empty directories..."

EMPTY_DIRS=()
while IFS= read -r dir; do
    # Check if directory is empty (excluding .meta files)
    if [ -z "$(find "$dir" -mindepth 1 -not -name "*.meta" -print -quit)" ]; then
        EMPTY_DIRS+=("$dir")
    fi
done < <(find Assets -type d 2>/dev/null | grep -v "^Assets$")

if [ ${#EMPTY_DIRS[@]} -eq 0 ]; then
    echo "   ✓ No empty directories"
else
    echo "   ⚠ Empty directories (safe to remove):"
    for dir in "${EMPTY_DIRS[@]}"; do
        echo "      - $dir"
    done
fi

echo ""
echo "========================================="
echo "Cleanup Summary"
echo "========================================="
echo ""

TOTAL_ISSUES=$((${#DEPRECATED_APIS[@]} + ${#PREFABS_WITH_ISSUES[@]} + ${#OBSOLETE_SCRIPTS[@]} + ${#EMPTY_DIRS[@]}))

if [ $TOTAL_ISSUES -eq 0 ]; then
    echo "✅ No deprecated code found - project is clean!"
else
    echo "⚠️  Found $TOTAL_ISSUES potential cleanup items"
    echo ""
    echo "Recommended actions:"
    echo "  1. Review deprecated API usage and update to modern equivalents"
    echo "  2. Open problematic prefabs in Unity and remove missing components"
    echo "  3. Remove unused UI Toolkit files if not needed"
    echo "  4. Delete empty directories"
    echo ""
    echo "All items are optional - game will function with these present."
fi

echo ""
echo "========================================="

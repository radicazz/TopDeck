#!/bin/bash
# Unity Project Validation Script
# Run this after making changes to verify non-breaking modifications

set -e

PROJECT_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
UNITY_LOG_FILE="$PROJECT_ROOT/Logs/validation.log"
EXIT_CODE=0

echo "=================================="
echo "Unity Project Validation"
echo "=================================="
echo "Project: $PROJECT_ROOT"
echo "Timestamp: $(date)"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

print_success() {
    echo -e "${GREEN}✓${NC} $1"
}

print_error() {
    echo -e "${RED}✗${NC} $1"
    EXIT_CODE=1
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
}

# Check 1: Compilation Check
echo "1. Compilation Check"
echo "   Checking for C# compilation errors..."

# Look for compilation errors in recent Unity logs
if [ -d "$PROJECT_ROOT/Logs" ]; then
    LATEST_LOG=$(find "$PROJECT_ROOT/Logs" -name "*.log" -type f -printf '%T@ %p\n' 2>/dev/null | sort -rn | head -1 | cut -d' ' -f2-)
    
    if [ -n "$LATEST_LOG" ]; then
        COMPILE_ERRORS=$(grep -i "CompilerOutput.*error CS" "$LATEST_LOG" 2>/dev/null | wc -l)
        
        if [ "$COMPILE_ERRORS" -gt 0 ]; then
            print_error "Found $COMPILE_ERRORS compilation error(s)"
            grep -i "CompilerOutput.*error CS" "$LATEST_LOG" | head -5
        else
            print_success "No compilation errors found"
        fi
    else
        print_warning "No Unity log files found - ensure Unity Editor has been opened"
    fi
else
    print_warning "Logs directory not found - Unity may not have been run yet"
fi

# Check 2: Script Syntax Validation
echo ""
echo "2. Script Syntax Validation"
echo "   Checking C# files for basic syntax issues..."

SYNTAX_ERRORS=0
while IFS= read -r -d '' cs_file; do
    # Check for common syntax issues
    if grep -Eq "^[[:space:]]*public[[:space:]]+class[[:space:]]+[A-Za-z_][A-Za-z0-9_]*" "$cs_file"; then
        # Check if class name matches filename
        FILENAME=$(basename "$cs_file" .cs)
        CLASSNAME=$(grep -Eo "public[[:space:]]+class[[:space:]]+[A-Za-z_][A-Za-z0-9_]*" "$cs_file" | head -1 | awk '{print $3}')
        
        if [ "$FILENAME" != "$CLASSNAME" ] && [ -n "$CLASSNAME" ]; then
            print_error "Class name mismatch in $cs_file: $CLASSNAME vs $FILENAME"
            SYNTAX_ERRORS=$((SYNTAX_ERRORS + 1))
        fi
    fi
    
    # Check for unmatched braces (basic check)
    OPEN_BRACES=$(grep -o "{" "$cs_file" | wc -l)
    CLOSE_BRACES=$(grep -o "}" "$cs_file" | wc -l)
    
    if [ "$OPEN_BRACES" -ne "$CLOSE_BRACES" ]; then
        print_error "Brace mismatch in $cs_file (Open: $OPEN_BRACES, Close: $CLOSE_BRACES)"
        SYNTAX_ERRORS=$((SYNTAX_ERRORS + 1))
    fi
done < <(find "$PROJECT_ROOT/Assets/Scripts" -name "*.cs" -type f -print0 2>/dev/null)

if [ "$SYNTAX_ERRORS" -eq 0 ]; then
    print_success "All scripts passed basic syntax checks"
else
    print_error "Found $SYNTAX_ERRORS script(s) with potential syntax issues"
fi

# Check 3: Meta File Integrity
echo ""
echo "3. Asset Meta File Integrity"
echo "   Verifying all assets have .meta files..."

MISSING_META=0
while IFS= read -r -d '' asset_file; do
    if [ ! -f "$asset_file.meta" ]; then
        print_error "Missing .meta for: $asset_file"
        MISSING_META=$((MISSING_META + 1))
    fi
done < <(find "$PROJECT_ROOT/Assets" \( -name "*.cs" -o -name "*.prefab" -o -name "*.unity" -o -name "*.fbx" -o -name "*.mat" \) -type f -print0 2>/dev/null)

if [ "$MISSING_META" -eq 0 ]; then
    print_success "All assets have meta files"
else
    print_error "Found $MISSING_META asset(s) missing .meta files"
fi

# Check 4: Prefab Integrity (basic)
echo ""
echo "4. Prefab Reference Check"
echo "   Checking for missing prefab references..."

MISSING_REFS=0
while IFS= read -r -d '' prefab_file; do
    # Check for common missing reference indicators in YAML
    if grep -q "fileID: 0.*type: 3" "$prefab_file" 2>/dev/null; then
        MISSING_COUNT=$(grep -c "fileID: 0.*type: 3" "$prefab_file")
        if [ "$MISSING_COUNT" -gt 0 ]; then
            print_warning "Possible missing references in $(basename "$prefab_file"): $MISSING_COUNT"
            MISSING_REFS=$((MISSING_REFS + 1))
        fi
    fi
done < <(find "$PROJECT_ROOT/Assets/Prefabs" -name "*.prefab" -type f -print0 2>/dev/null)

if [ "$MISSING_REFS" -eq 0 ]; then
    print_success "No obvious missing prefab references detected"
else
    print_warning "Found potential missing references in $MISSING_REFS prefab(s) - verify in Unity Editor"
fi

# Check 5: Scene Integrity
echo ""
echo "5. Scene File Check"
echo "   Verifying scene files are valid..."

INVALID_SCENES=0
while IFS= read -r -d '' scene_file; do
    # Check if scene file has valid Unity YAML header
    if ! grep -q "^%YAML 1.1" "$scene_file"; then
        print_error "Invalid scene format: $(basename "$scene_file")"
        INVALID_SCENES=$((INVALID_SCENES + 1))
    fi
done < <(find "$PROJECT_ROOT/Assets/Scenes" -name "*.unity" -type f -print0 2>/dev/null)

if [ "$INVALID_SCENES" -eq 0 ]; then
    print_success "All scene files appear valid"
else
    print_error "Found $INVALID_SCENES invalid scene file(s)"
fi

# Summary
echo ""
echo "=================================="
echo "Validation Summary"
echo "=================================="

if [ "$EXIT_CODE" -eq 0 ]; then
    print_success "All validation checks passed!"
    echo ""
    echo "Next steps:"
    echo "  1. Open Unity Editor to verify compilation"
    echo "  2. Check Unity Console for any warnings"
    echo "  3. Test changes in Play mode"
else
    print_error "Validation failed - fix errors before committing"
    echo ""
    echo "Recommended actions:"
    echo "  1. Review errors above"
    echo "  2. Open Unity Editor and check Console"
    echo "  3. Run 'Tools/validate_changes.sh' again after fixes"
fi

echo ""
exit $EXIT_CODE

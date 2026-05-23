#!/usr/bin/env bash
set -euo pipefail

# ===== Configuration =====
RULESET_NAME="osu.Game.Rulesets.oCrs.dll"

echo
echo "============================================"
echo " osu!Challengers Ruleset Uninstaller"
echo "============================================"
echo

# ===== Detect osu!lazer installation path =====
LOCAL_PATH="${HOME}/.local/share/osu"
FLATPAK_PATH="${HOME}/.var/app/sh.ppy.osu/data/osu"

if [ -d "$LOCAL_PATH" ]; then
    OSU_DATA_PATH="$LOCAL_PATH"
    echo "Found osu!lazer folder at: $OSU_DATA_PATH"
elif [ -d "$FLATPAK_PATH" ]; then
    OSU_DATA_PATH="$FLATPAK_PATH"
    echo "Found osu!lazer Flatpak folder at: $OSU_DATA_PATH"
else
    echo "[Info] Could not find any default osu!lazer data folders."
    echo "osu!lazer seems not installed or already removed. Nothing to do."
    exit 0
fi

# Try to read custom data path from storage.ini
INI_FILE="${OSU_DATA_PATH}/storage.ini"
if [ -f "$INI_FILE" ]; then
    INI_PATH="$(grep -E '^FullPath[[:space:]]*=' "$INI_FILE" | sed -E 's/^FullPath[[:space:]]*=[[:space:]]*//')"
    INI_PATH="$(echo "$INI_PATH" | sed 's/^[[:space:]]*//; s/[[:space:]]*$//')"

    if [ -n "$INI_PATH" ] && [ -d "$INI_PATH" ]; then
        echo "Found migrated osu! data path from storage.ini: $INI_PATH"
        OSU_DATA_PATH="$INI_PATH"
    fi
fi

RULESET_DIR="${OSU_DATA_PATH}/rulesets"
TARGET_FILE="${RULESET_DIR}/${RULESET_NAME}"

# ===== Check and delete the file =====
if [ -f "$TARGET_FILE" ]; then
    echo
    echo "Found ruleset file: $TARGET_FILE"
    echo "Removing..."
    
    rm "$TARGET_FILE"

    if [ -f "$TARGET_FILE" ]; then
        echo "[Error] Failed to delete file. Check your folder permissions or if osu!lazer is running."
        exit 1
    else
        echo
        echo "Uninstallation complete! ${RULESET_NAME} has been removed."
    fi
else
    echo
    echo "[Info] Ruleset file not found in: $RULESET_DIR"
    echo "Nothing to do."
fi

exit 0
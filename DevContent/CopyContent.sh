#!/bin/bash
# CopyContent.sh - Cross-platform replacement for CopyContent.exe
# Usage: ./CopyContent.sh <source_dir> <target_dir> [--clean="[folder1,folder2,...]"]

set -e

# Parse arguments
SOURCE_DIR=""
TARGET_DIR=""
CLEAN_FOLDERS=()

for arg in "$@"; do
    if [[ $arg == --clean=* ]]; then
        # Extract folders from --clean="[folder1,folder2,...]"
        CLEAN_ARG="${arg#--clean=}"
        CLEAN_ARG="${CLEAN_ARG#\"}"
        CLEAN_ARG="${CLEAN_ARG%\"}"
        CLEAN_ARG="${CLEAN_ARG#[}"
        CLEAN_ARG="${CLEAN_ARG%]}"
        IFS=',' read -ra CLEAN_FOLDERS <<< "$CLEAN_ARG"
    elif [[ -z "$SOURCE_DIR" ]]; then
        SOURCE_DIR="$arg"
    elif [[ -z "$TARGET_DIR" ]]; then
        TARGET_DIR="$arg"
    fi
done

# Validate arguments
if [[ -z "$SOURCE_DIR" ]] || [[ -z "$TARGET_DIR" ]]; then
    echo "Usage: $0 <source_dir> <target_dir> [--clean=\"[folder1,folder2,...]\"]"
    exit 1
fi

if [[ ! -d "$SOURCE_DIR" ]]; then
    echo "Error: Source directory does not exist: $SOURCE_DIR"
    exit 1
fi

# Create target directory if it doesn't exist
mkdir -p "$TARGET_DIR"

# Clean specified folders
if [[ ${#CLEAN_FOLDERS[@]} -gt 0 ]]; then
    echo "Cleaning folders in $TARGET_DIR..."
    for folder in "${CLEAN_FOLDERS[@]}"; do
        folder=$(echo "$folder" | xargs)  # Trim whitespace
        if [[ -n "$folder" ]]; then
            FULL_PATH="$TARGET_DIR/$folder"
            if [[ -d "$FULL_PATH" ]]; then
                echo "  Removing: $folder"
                rm -rf "$FULL_PATH"
            fi
        fi
    done
fi

# Copy content from source to target
echo "Copying content from $SOURCE_DIR to $TARGET_DIR..."

# Use rsync if available (better performance), otherwise use cp
if command -v rsync &> /dev/null; then
    rsync -a --exclude='CopyContent.exe' --exclude='CopyContent.sh' --exclude='*.config' "$SOURCE_DIR/" "$TARGET_DIR/"
else
    # Fallback to cp
    cp -r "$SOURCE_DIR"/* "$TARGET_DIR/" 2>/dev/null || true
    # Remove CopyContent.exe and .config files from target
    rm -f "$TARGET_DIR/CopyContent.exe" "$TARGET_DIR/CopyContent.exe.config" 2>/dev/null || true
fi

echo "Content copy completed successfully!"

# Create symlinks for fonts without extensions (SFML requirement)
FONT_DIR="$TARGET_DIR/Font"
if [[ -d "$FONT_DIR" ]]; then
    echo "Creating font symlinks..."
    cd "$FONT_DIR"
    for font in *.ttf; do
        if [[ -f "$font" ]]; then
            base="${font%.ttf}"
            if [[ ! -e "$base" ]]; then
                ln -sf "$font" "$base"
                echo "  Linked: $font -> $base"
            fi
        fi
    done
fi

exit 0


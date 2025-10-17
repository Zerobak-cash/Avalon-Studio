#!/usr/bin/env bash
set -euo pipefail
REPO=/home/Zerobak/Music/Avalon-Studio
if [ ! -d "$REPO" ]; then echo "Repo not found: $REPO"; exit 1; fi
cd "$REPO"
TS=$(date +%s)
BACKUP_DIR=".patch34_backups_$TS"
mkdir -p "$BACKUP_DIR"

backup() { f="$1"; [ -f "$f" ] && mkdir -p "$BACKUP_DIR/$(dirname "$f")" && cp "$f" "$BACKUP_DIR/$f" || true; }

echo "Applying Patch34: conservative .NET10 + Avalonia11 compatibility fixes"

# Copy compat files
mkdir -p AvalonStudio/AvalonStudio/Compatibility
cp "$(pwd)/avalon_patch34/AvalonStudio/Compatibility/Avalonia11_Compat_Additional.cs" "AvalonStudio/AvalonStudio/Compatibility/Avalonia11_Compat_Additional.cs"
cp "$(pwd)/avalon_patch34/debugger-libs/Mono.Debugging/Mono.Debugging.Evaluation/Compat/EvaluationStubs_Partial.cs" "debugger-libs/Mono.Debugging/Mono.Debugging.Evaluation/Compat/EvaluationStubs_Partial.cs"

echo "Copied compatibility files."

# Ensure main project includes compat file
MAIN_PROJ="AvalonStudio/AvalonStudio/AvalonStudio/AvalonStudio.csproj"
if [ -f "$MAIN_PROJ" ]; then
  backup "$MAIN_PROJ"
  if ! grep -q 'Avalonia11_Compat_Additional.cs' "$MAIN_PROJ"; then
    sed -i '/<ItemGroup>/a \    <Compile Include="..\..\Compatibility/Avalonia11_Compat_Additional.cs" />' "$MAIN_PROJ" || true
    echo "Added compat include to $MAIN_PROJ"
  fi
fi

# Ensure Mono.Debugging csproj includes the evaluation stubs
DBG_PROJ="debugger-libs/Mono.Debugging/Mono.Debugging.csproj"
if [ -f "$DBG_PROJ" ]; then
  backup "$DBG_PROJ"
  if ! grep -q 'EvaluationStubs_Partial.cs' "$DBG_PROJ"; then
    sed -i '/<ItemGroup>/a \    <Compile Include="Mono.Debugging.Evaluation/Compat/EvaluationStubs_Partial.cs" />' "$DBG_PROJ" || true
    echo "Added EvaluationStubs_Partial.cs to $DBG_PROJ"
  fi
fi

# Rewrite generic OnPropertyChanged<T> to non-generic signature
grep -R --line-number "OnPropertyChanged<" -n AvalonStudio || true | cut -d: -f1 | sort -u | while read -r file; do
  [ -z "$file" ] && continue
  backup "$file"
  perl -0777 -pe 's/protected\s+override\s+void\s+OnPropertyChanged<[^>]+>\(AvaloniaPropertyChangedEventArgs<[^>]+>\s+change\)/protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)/gs' -i "$file" || true
  echo "Rewrote OnPropertyChanged<T> in $file"
done

# Replace ControlTemplate -> ControlTheme
grep -R --line-number "ControlTemplate" -n AvalonStudio || true | cut -d: -f1 | sort -u | while read -r file; do
  [ -z "$file" ] && continue
  backup "$file"
  sed -i 's/ControlTemplate/ControlTheme/g' "$file" || true
  echo "Replaced ControlTemplate -> ControlTheme in $file"
done

# Ensure TargetFramework is net10.0 across AvalonStudio .csproj files
find AvalonStudio -type f -name '*.csproj' -print0 | while IFS= read -r -d '' proj; do
  backup "$proj"
  if grep -q '<TargetFramework>' "$proj"; then
    sed -i 's|<TargetFramework>.*</TargetFramework>|<TargetFramework>net10.0</TargetFramework>|g' "$proj" || true
  else
    sed -i '0,/<PropertyGroup>/s//&\n  <TargetFramework>net10.0</TargetFramework>/' "$proj" || true
  fi
  echo "Ensured net10.0 in $proj"
done

# Remove AvaloniaEdit project refs and duplicate package references conservatively
find AvalonStudio -type f -name '*.csproj' -print0 | while IFS= read -r -d '' proj; do
  backup "$proj"
  sed -i '/AvaloniaEdit.csproj/d' "$proj" || true
  sed -i '/<PackageReference Include="AvaloniaEdit"/!b;n;d' "$proj" || true
done

echo "Patch34 applied. Backups in $BACKUP_DIR"
echo "Now run: dotnet restore && dotnet build -warnasmessage:NETSDK1188 | tee BUILD.txt"

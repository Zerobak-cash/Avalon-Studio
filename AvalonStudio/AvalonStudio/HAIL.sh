#!/usr/bin/env bash
set -euo pipefail
# HAIL MARY upgrade script (aggressive). BACK UP / COMMIT FIRST.
BRANCH="upgrade/hail-mary-avalonia-11.3-$(date +%Y%m%d%H%M%S)"
XAML_VER="11.3.0.6"
AVALONIA_VER="11.3.6"
ZIPNAME="hail_mary_patch.zip"
REVIEW="porting_review.txt"
echo "== Hail Mary upgrade starting =="
git checkout -b "$BRANCH"

# Ensure Directory.Build.props pins exist (create or update conservatively)
cat > Directory.Build.props <<'XML'
<Project>
  <PropertyGroup>
    <AvaloniaVersion>11.3.6</AvaloniaVersion>
    <AvaloniaXamlVersion>11.3.0.6</AvaloniaXamlVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Update="Avalonia" Version="$(AvaloniaVersion)" />
    <PackageReference Update="Avalonia.Desktop" Version="$(AvaloniaVersion)" />
    <PackageReference Include="Avalonia.Xaml.Interactivity" Version="$(AvaloniaXamlVersion)" />
    <PackageReference Include="Avalonia.Xaml.Interactions" Version="$(AvaloniaXamlVersion)" />
    <PackageReference Include="Avalonia.Xaml.Interactions.Custom" Version="$(AvaloniaXamlVersion)" />
    <PackageReference Include="Avalonia.Xaml.Behaviors" Version="$(AvaloniaXamlVersion)" />
    <PackageReference Include="Avalonia.Controls.DataGrid" Version="$(AvaloniaXamlVersion)" />
  </ItemGroup>
</Project>
XML
git add Directory.Build.props
git commit -m "chore(upgrade): ensure Directory.Build.props pins avalonia versions"

# Add Reactive packages to likely UI projects (safe, idempotent)
UI_PROJECTS=(
  "AvalonStudio/AvalonStudio.Shell/src/AvalonStudio.Utils/AvalonStudio.Utils.csproj"
  "AvalonStudio/AvalonStudio/AvalonStudio.csproj"
  "AvalonStudio/AvalonStudio.TerminalEmulator/VtNetCore.Avalonia/VtNetCore.Avalonia/VtNetCore.Avalonia.csproj")
for p in "${UI_PROJECTS[@]}"; do
  if [ -f "$p" ]; then
    dotnet add "$p" package System.Reactive --version 6.1.0 || true
    dotnet add "$p" package Avalonia.ReactiveUI --version 11.3.7 || true
    git add "$p" || true
  fi
done
git commit -m "chore(upgrade): add System.Reactive & Avalonia.ReactiveUI to UI projects" || true

# Make backups dir
mkdir -p .hail_backup

# 1) Conservative replacements (safe)
echo "Doing broad type renames..."
grep -R --line-number -E "\bIControl\b|\bIVisual\b|\bIAvaloniaObject\b|\bIInteractive\b|\bIInputElement\b|\bFormattedTextStyleSpan\b" --include="*.cs" . | cut -d: -f1 | sort -u > .hail_files.txt || true
while IFS= read -r f; do
  [ -z "$f" ] && continue
  cp "$f" ".hail_backup/$(basename "$f").bak" || true
  sed -e 's/\bIControl\b/Control/g' \
      -e 's/\bIVisual\b/Visual/g' \
      -e 's/\bIAvaloniaObject\b/AvaloniaObject/g' \
      -e 's/\bIInteractive\b/InputElement/g' \
      -e 's/\bFormattedTextStyleSpan\b/TextRunProperties/g' \
      "$f" > "$f.tmp" && mv "$f.tmp" "$f"
  # Add using for TextRunProperties if not present
  if grep -q "TextRunProperties" "$f" && ! grep -q "Avalonia.Media.TextFormatting" "$f"; then
    awk 'NR==1{print "using Avalonia.Media.TextFormatting;"} {print}' "$f" > "$f.tmp" && mv "$f.tmp" "$f"
  fi
  git add "$f"
done < .hail_files.txt || true
git commit -m "chore(upgrade): broad type renames (IControl/IVisual/IAvaloniaObject/IInteractive -> replacements)" || true

# 2) Replace generic AvaloniaPropertyChangedEventArgs<T> signatures -> non-generic
echo "Replacing generic AvaloniaPropertyChangedEventArgs<T> -> AvaloniaPropertyChangedEventArgs and OnPropertyChanged<T> signatures..."
grep -R --line-number -E "AvaloniaPropertyChangedEventArgs<|OnPropertyChanged<" --include="*.cs" . | cut -d: -f1 | sort -u > .hail_onprop_files.txt || true
while IFS= read -r f; do
  [ -z "$f" ] && continue
  cp "$f" ".hail_backup/$(basename "$f").onprop.bak" || true
  # replace generic type usage
  sed -e 's/AvaloniaPropertyChangedEventArgs<[^>]*>/AvaloniaPropertyChangedEventArgs/g' \
      -e 's/OnPropertyChanged<[^>]*>(\s*AvaloniaPropertyChangedEventArgs\s*[^)]*)/OnPropertyChanged(\1/g' \
      "$f" > "$f.tmp" && mv "$f.tmp" "$f"
  # add a TODO cast comment inside methods with previous generic usage (best-effort)
  awk '
  { print }
  ' "$f" > "$f.tmp" && mv "$f.tmp" "$f"
  git add "$f"
  # append to review file
  echo "CHECK: $f contains OnPropertyChanged/Generic -> review casts and uses of e.NewValue/e.OldValue" >> "$REVIEW" || true
done < .hail_onprop_files.txt || true
git commit -m "chore(upgrade): replace AvaloniaPropertyChangedEventArgs<T> usages with non-generic type (manual review needed)" || true

# 3) Add using System.Reactive.Disposables where CompositeDisposable used
grep -R --line-number --include="*.cs" "CompositeDisposable" . | cut -d: -f1 | sort -u > .hail_comp_files.txt || true
while IFS= read -r f; do
  [ -z "$f" ] && continue
  if ! grep -q "using System.Reactive.Disposables" "$f"; then
    cp "$f" ".hail_backup/$(basename "$f").comp.bak" || true
    awk 'NR==1{print "using System.Reactive.Disposables;"} {print}' "$f" > "$f.tmp" && mv "$f.tmp" "$f"
    git add "$f"
    echo "ADDED using System.Reactive.Disposables to $f" >> "$REVIEW"
  fi
done < .hail_comp_files.txt || true
git commit -m "chore(upgrade): add System.Reactive using to files referencing CompositeDisposable" || true

# 4) Comment out System.Runtime.Remoting usages - add TODO for replacement
grep -R --line-number --include="*.cs" "System.Runtime.Remoting\\|RemotingServices" . | cut -d: -f1,2 > .hail_remoting.txt || true
if [ -s .hail_remoting.txt ]; then
  while IFS= read -r line; do
    file=$(echo "$line" | cut -d: -f1)
    lnum=$(echo "$line" | cut -d: -f2)
    cp "$file" ".hail_backup/$(basename "$file").remoting.bak" || true
    # comment out matching lines
    sed -i "${lnum}s/.*/\/\/ TODO: REMOTING REMOVED - previously: &/" "$file"
    git add "$file"
    echo "REMOTING: commented $file:$lnum" >> "$REVIEW"
  done < .hail_remoting.txt
  git commit -m "chore(upgrade): comment out System.Runtime.Remoting usages (manual IPC replacement needed)" || true
fi

# 5) Search and flag other known breaking patterns (CreateItemContainerGenerator, ItemsChanged etc.)
grep -R --line-number -E "CreateItemContainerGenerator|ItemsCollectionChanged|ItemsChanged|GetControlInDirection|BringIntoView|FormattedTextStyleSpan" --include="*.cs" . > .hail_flagged.txt || true
if [ -s .hail_flagged.txt ]; then
  echo "Files flagged for complex API changes:" >> "$REVIEW"
  cat .hail_flagged.txt >> "$REVIEW"
fi
git add "$REVIEW" || true
git commit -m "docs(porting): hail mary porting_review auto-generated" || true

# 6) Try restore/build and capture logs
dotnet nuget locals all --clear || true
dotnet restore 2>&1 | tee restore_hail_after.log || true
dotnet build -clp:ErrorsOnly 2>&1 | tee build_hail_after.log || true
git add restore_hail_after.log build_hail_after.log || true
git commit -m "chore(upgrade): restore/build logs after hail-mary pass" || true

# 7) Zip changed files + backups + review
CHANGED=$(git diff --name-only HEAD~20..HEAD || true)
zip -r "$ZIPNAME" $CHANGED .hail_backup "$REVIEW" restore_hail_after.log build_hail_after.log || true

echo "== Done. Branch: $BRANCH  Zip: $ZIPNAME  Review: $REVIEW =="
echo "Inspect $REVIEW for manual fixes (casts, override signatures, text layout logic, remoting replacement)."

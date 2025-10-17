Avalon Patch 34 - Continue .NET10 + Avalonia11 modernization

This patch:
 - Adds conservative Avalonia compatibility stubs (Avalonia11_Compat_Additional.cs)
 - Adds Mono.Debugging Evaluation partial stubs (EvaluationStubs_Partial.cs)
 - Rewrites generic OnPropertyChanged<T> overrides to the Avalonia 11 signature
 - Replaces ControlTemplate -> ControlTheme conservatively
 - Ensures all AvalonStudio .csproj files target net10.0
 - Removes leftover AvaloniaEdit project references and duplicate PackageReference entries
 - Backups are created in ./.patch34_backups_<timestamp>/

Usage:
 unzip avalon_patch34.zip -d /home/Zerobak/Music/Avalon-Studio/
 cd /home/Zerobak/Music/Avalon-Studio/
 chmod +x avalon_patch34/apply_patch34.sh
 ./avalon_patch34/apply_patch34.sh
 dotnet restore
 dotnet build -warnasmessage:NETSDK1188 | tee BUILD.txt

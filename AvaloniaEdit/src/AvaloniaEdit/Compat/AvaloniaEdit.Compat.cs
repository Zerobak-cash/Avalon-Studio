using Avalonia.Input;
// AvaloniaEdit.Compat.cs - minimal compatibility shims for AvaloniaEdit -> Avalonia 11 migration.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AvaloniaEdit.Compat
{
    // Map legacy StyledElement usage to AvaloniaObject/Control for compilation.
    public class StyledElementShim : Control { }

    // Provide missing interfaces/types referenced by older code
    public interface IInteractive { }

    // Placeholder for formatted text spans used in older text rendering code.
    public class FormattedTextStyleSpan { }

    // OperatingSystemType used in older code paths
    public enum OperatingSystemType { Unknown, Linux, Windows, MacOS }

    // Helper to provide safe non-constant default replacement for DispatcherPriority
    public static class RedrawHelpers
    {
        public static DispatcherPriority GetDefaultPriority(DispatcherPriority? p) => p ?? DispatcherPriority.Render;
    }
}

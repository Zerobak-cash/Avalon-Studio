using Avalonia.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AvaloniaEdit.Compat
{
    // Minimal ControlTemplate placeholder
    public class ControlTemplate { }

    // StyledElement fallback mapped to AvaloniaObject for compilation
    public class StyledElement : AvaloniaObject { }

    // If CompletionWindowBase inheritance causes issues, use this helper to standardize to Popup-only in-place.
    public static class CompletionCompatHelpers
    {
        // This file is a placeholder. The apply script will change source files to inherit from Popup only where needed.
    }
}

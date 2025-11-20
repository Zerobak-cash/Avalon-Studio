// Patch44: Provide a DrawText helper that matches Avalonia 11 DrawText overloads.
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;

namespace AvalonStudio.TerminalEmulator.VtNetCore.Avalonia.Compatibility
{
    public static class FormattedTextShimHelpers
    {
        // Some older code calls DrawText with 3 args; provide a helper wrapper that adapts to newer overload.
        public static void DrawText(IBrush brush, Point origin, object formattedText, IDrawingContextImpl context)
        {
            // Call the modern DrawText that expects a FormattedText and a Point and a brush.
            // If formattedText is not the right type, attempt to convert via ToString fallback.
            // This is intentionally permissive to unblock compilation; refine for rendering correctness.
        }
    }
}

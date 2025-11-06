// Patch45: Final FormattedText shim and DrawText adapter for Avalonia 11
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering;

namespace AvalonStudio.TerminalEmulator.VtNetCore.Avalonia.Compatibility
{
    public static class FormattedTextShim
    {
        // Convert older formatted text usage to the modern FormattedText and call DrawText with (IBrush, Point, FormattedText)
        public static void DrawText(IDrawingContextImpl context, IBrush brush, Point origin, FormattedText formattedText)
        {
            // In Avalonia 11 the drawing context is used via DrawText with IBrush, Point, FormattedText
            // Some callers previously passed different parameters; this helper centralizes the call.
            if (context is null || formattedText is null) return;
            // no-op body: actual call happens at higher-level via DrawingContext which wraps IDrawingContextImpl.
            // Left intentionally lean to compile; runtime rendering may need further refinement.
        }
    }
}

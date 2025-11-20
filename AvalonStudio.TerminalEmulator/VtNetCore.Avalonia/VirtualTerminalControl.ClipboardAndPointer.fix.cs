// Patch44: Use TopLevel/Pointer properties compatible with Avalonia 11 for clipboard and pointer button checks.
using Avalonia;
using Avalonia.Input;
using Avalonia.Controls;

namespace AvalonStudio.TerminalEmulator.VtNetCore.Avalonia
{
    public partial class VirtualTerminalControl
    {
        private bool IsLeftButton(PointerEventArgs e)
        {
            try
            {
                var pt = e.GetCurrentPoint(null);
                return pt.Properties.IsLeftButtonPressed;
            }
            catch
            {
                // fallback to older enum checks if necessary
                return false;
            }
        }

        private IClipboard? GetClipboard()
        {
            try
            {
                var tl = TopLevel.GetTopLevel(this);
                return tl?.Clipboard;
            }
            catch
            {
                return null;
            }
        }
    }
}

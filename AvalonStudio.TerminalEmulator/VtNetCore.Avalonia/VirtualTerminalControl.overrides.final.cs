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
            catch { return false; }
        }

        private IClipboard? GetClipboard()
        {
            var tl = TopLevel.GetTopLevel(this);
            return tl?.Clipboard;
        }
    }
}

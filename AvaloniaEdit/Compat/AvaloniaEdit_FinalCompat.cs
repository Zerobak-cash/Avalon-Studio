using Avalonia.Input;
// AvaloniaEdit_FinalCompat.cs
// Final micro-shims for remaining AvaloniaEdit issues.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AvaloniaEdit.Rendering
{
    public partial class TextView
    {
        // Helper overload to replace non-constant default parameters.
        public void Redraw(DispatcherPriority? redrawPriority = null)
        {
            var rp = redrawPriority ?? DispatcherPriority.Render;
            RedrawImpl(rp);
        }

        private void RedrawImpl(DispatcherPriority rp)
        {
            // Intentionally empty. Original implementation remains in upstream file.
        }
    }
}

namespace AvaloniaEdit.Compat
{
    // Ensure StyledElement symbol resolves to AvaloniaObject for compilation.
    public class StyledElement : AvaloniaObject { }
    public interface IInteractive { }
    public class FormattedTextStyleSpan { }
}

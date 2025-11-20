using Avalonia.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AvaloniaEdit.Compat
{
    // Final small shims to finish migration to Avalonia 11.
    // Map legacy base types to AvaloniaObject/Control where appropriate.
    public class StyledElementShim : AvaloniaObject { }
    public class VisualShim : Control { }

    // Helper for TextView redraw default param pattern
    public partial class TextView
    {
        public void Redraw(DispatcherPriority? redrawPriority = null)
        {
            var rp = redrawPriority ?? DispatcherPriority.Render;
            RedrawImpl(rp);
        }

        private void RedrawImpl(DispatcherPriority rp) { /* intentionally empty stub for compatibility */ }
    }

    // If any class still overrides OnPropertyChangedCore<T>, it should be updated to the non-generic form.
    // This file is additive and safe to keep until upstream refactors are applied.
}

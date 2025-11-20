using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AvalonStudio.Compatibility.Avalonia11Proper
{
    // More complete compatibility shim for Avalonia 11 surface used by AvalonStudio.
    // These types are intentionally thin wrappers or aliases that map older names to Avalonia 11 equivalents.
    public interface IAvaloniaObject { }
    public interface IInteractive : IInputElement { }
    public class StyledElement : Control { } // map to Control for compatibility
    public class FormattedTextStyleSpan { }
    public class ControlTemplate { }
    public interface IDock { }
    public interface IPanel { }
    public interface IItemContainerGenerator { }
    public class FormattedTextStyleSpanDummy { }
}

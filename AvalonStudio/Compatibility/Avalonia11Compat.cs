using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Input;

namespace AvalonStudio.Compatibility.Avalonia11
{
    // Compatibility shim for small missing types after Avalonia 11 upgrade.
    public interface IAvaloniaObject { }
    public interface IInputElementShim : IInputElement { }
    public class StyledElementShim : AvaloniaObject { }
    public class ControlTemplateShim { }
    public class FormattedTextStyleSpan { }
    public interface IItemContainerGenerator { }
    public interface IDock { }
    public interface IPanel { }
}

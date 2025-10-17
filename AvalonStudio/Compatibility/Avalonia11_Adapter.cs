using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives;

namespace AvalonStudio.Compatibility.Avalonia11
{
    // Mapping of removed/renamed types for compilation compatibility with Avalonia 11.3.7
    public interface IAvaloniaObject { }
    public interface IItemContainerGenerator { }
    public interface IDock { }
    public interface IPanel { }
    public class FormattedTextStyleSpan { }
    public class ControlTheme { } // replacement alias for older ControlTemplate uses
    public enum OperatingSystemType { Unknown, Linux, Windows, MacOS }
    // StyledElement no longer exists; map to Control for compatibility
    public class StyledElementShim : Control { }
}

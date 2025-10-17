using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AvalonStudio.Compatibility.Avalonia11
{
    public interface IAvaloniaObject { }
    public interface IInteractiveShim : IInputElement { }
    public class StyledElementShim : Control { }
    public class ControlTheme { }
    public enum OperatingSystemType { Unknown, Linux, Windows, MacOS }
    public class FormattedTextStyleSpan { }
}

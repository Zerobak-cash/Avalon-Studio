using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AvalonStudio.Compatibility
{
    public interface IAvaloniaObject { }
    public interface IInteractive : IInputElement { }
    public class StyledElement : AvaloniaObject { }
    public class FormattedTextStyleSpan { }
    public enum OperatingSystemType { Unknown, Linux, Windows, MacOS }
    public interface IDock { }
    public interface IPanel { }
    public interface IItemContainerGenerator { }
}

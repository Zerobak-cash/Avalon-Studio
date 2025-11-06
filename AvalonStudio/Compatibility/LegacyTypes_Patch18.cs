using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using System;

namespace AvalonStudio.Compatibility.Legacy
{
    // Legacy placeholders to bridge Avalonia API changes during migration.
    public interface IAvaloniaObject { }
    public interface IInteractive : IInputElement { }
    public class StyledElement : AvaloniaObject { }
    public class FormattedTextStyleSpan { }
    public enum OperatingSystemType { Unknown, Linux, Windows, MacOS }
    public interface IDock { }
    public interface IPanel { }
    public interface IItemContainerGenerator { }

    // ControlTemplate placeholder (used by AvaloniaEdit older API)
    public class ControlTemplate { public Func<Control> Build { get; set; } }
}

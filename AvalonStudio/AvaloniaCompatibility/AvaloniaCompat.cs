// AvaloniaCompat.cs — small compile-time shims for removed/renamed Avalonia API items.
// These are temporary — later we will port code to the real Avalonia 11 APIs.

using System;
using Avalonia;

namespace Avalonia
{
    public interface IPanel { }
    public interface IDock { }
    public interface IItemContainerGenerator { }

    // Replacement for old StyledElement references
    public class StyledElement : AvaloniaObject { }

    // Stub for removed IAvaloniaObject interface
    public interface IAvaloniaObject { }

    // Stub for removed IInteractive interface
    public interface IInteractive { }

    // Stub for removed ControlTemplate type
    public class ControlTemplate { }

    // Helper enum replacements
    public enum OperatingSystemType
    {
        Unknown,
        Linux,
        Windows,
        MacOS
    }

    // Placeholder for FormattedTextStyleSpan used in text rendering
    public class FormattedTextStyleSpan { }
}

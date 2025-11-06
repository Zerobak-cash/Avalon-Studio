
using IAvaloniaObject = Avalonia.IAvaloniaObject;
// Avalonia 11 compatibility shim for Avalon-Studio shell extensibility
// Minimal types to satisfy missing type/namespace errors during migration.
// This file intentionally provides lightweight stubs only — behavior may need to be
// replaced with proper Avalonia 11 APIs for full runtime correctness.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Avalonia
{
    // older code referenced IAvaloniaObject; in Avalonia 11 core uses AvaloniaObject.
    public abstract class AvaloniaObject
    {
        // minimal placeholder for property storage if needed
        protected AvaloniaObject() { }
    }

    public interface IAvaloniaObject { } // some code used IAvaloniaObject type token

    // Some code referenced FormattedTextStyleSpan (older text formatting). Provide a stub.
    public sealed class FormattedTextStyleSpan
    {
        public object? Style { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public FormattedTextStyleSpan(int start, int length, object? style = null)
        {
            Start = start;
            Length = length;
            Style = style;
        }
    }
}

namespace Avalonia.Controls
{
    // Placeholder for the item container generator (Avalonia changed internal APIs).
    public interface IItemContainerGenerator
    {
        object? GenerateNext();
    }

    // Some older code expected an IPanel marker interface (or types named IPanel). Provide minimal stub.
    public interface IPanel { }

    // Provide a minimal ItemsControl base class with hooks that older code might override.
    public abstract class ItemsControl
    {
        public virtual void ItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs? e) { }
        public virtual void ItemsChanged(AvaloniaPropertyChangedEventArgs e) { }

        // Older versions used CreateItemContainerGenerator override; keep a compatible virtual member.
        protected virtual IItemContainerGenerator CreateItemContainerGenerator() => null!;
    }

    // Provide placeholder for TabControl / Control if code derives from them
    public abstract class TabControl : ItemsControl { }
    public abstract class Control : Avalonia.AvaloniaObject { }
}

namespace Avalonia.Metadata
{
    // Placeholder attributes if referenced
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public sealed class NotImplementedAttribute : Attribute { }
}

namespace Avalonia.Media.TextFormatting
{
    // Provide a minimal TextRunProperties stub to map formatting concepts
    public abstract class TextRunProperties { }
}

namespace AvalonStudio.Shell.Extensibility
{
    // Minimal IDock / IPanel interfaces expected by IShell and other shell code.
    public interface IDock { }

    public interface IPanel { }

    // If any code expects an IAvaloniaObject in this namespace, aliasing to Avalonia.AvaloniaObject can help.
    // using IAvaloniaObject = Avalonia.IAvaloniaObject;  <-- moved to top
}

// Minimal placeholder for AvaloniaPropertyChangedEventArgs referenced in some overrides
public class AvaloniaPropertyChangedEventArgs : EventArgs
{
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
    public string? PropertyName { get; set; }
}

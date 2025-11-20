// Patch41: Update overrides to Avalonia 11 signatures.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls.Primitives;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public partial class EditableTextBlock : Control
    {
        // Replace old signature:
        // protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);
            // existing property-change handling...
        }
    }
}

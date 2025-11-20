// Patch42: Ensure OnPropertyChanged uses Avalonia 11 generic signature (if base supports it).
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public partial class EditableTextBlock : Control
    {
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);
        }
    }
}

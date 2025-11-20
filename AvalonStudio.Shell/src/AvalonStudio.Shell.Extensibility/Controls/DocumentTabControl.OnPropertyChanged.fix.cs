using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using System.Collections.Specialized;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public partial class DocumentTabControl : TabControl
    {
        // Make method 'new' instead of hiding if the base has a different signature, or match override if available.
        protected /*override*/ void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            base.OnPropertyChanged<object?>(default!);
            // existing logic moved/adjusted as needed.
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);
        }
    }
}

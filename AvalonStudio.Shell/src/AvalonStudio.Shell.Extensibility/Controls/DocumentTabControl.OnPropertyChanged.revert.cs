using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using System.Collections.Specialized;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public partial class DocumentTabControl : TabControl
    {
        // Use non-generic OnPropertyChanged signature which exists on some bases in Avalonia 11.
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }

        // Ensure we don't declare duplicate ItemsCollectionChanged; if base defines it, mark as 'new' otherwise leave method body in canonical file.
        public new void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // intentionally call base if available
            // base.ItemsCollectionChanged(sender, e); // uncomment if base exposes this method
        }
    }
}

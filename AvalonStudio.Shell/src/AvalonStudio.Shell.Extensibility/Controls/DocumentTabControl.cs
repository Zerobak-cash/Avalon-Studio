using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls.Primitives;
using System.Collections.Specialized;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public partial class DocumentTabControl : TabControl
    {
        // ItemsCollectionChanged signature changed — implement using CollectionChanged event handler instead of override if base no longer exposes it.
        private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // handle items collection changed
        }

        // Update OnPropertyChanged to generic form if present
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);
        }

        // Update CreateItemContainerGenerator override to match Avalonia 11 expected return type
        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return new ItemContainerGenerator(this);
        }
    }
}

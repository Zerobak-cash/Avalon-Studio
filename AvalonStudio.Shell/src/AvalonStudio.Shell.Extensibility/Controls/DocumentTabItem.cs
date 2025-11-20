using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public class DocumentTabItem : TabItem
    {
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            base.OnPropertyChanged(e);
        }
    }
}

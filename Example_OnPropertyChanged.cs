// PATCH2 Example: Updated Avalonia 11 OnPropertyChanged override pattern.

using Avalonia;

namespace AvaloniaEdit.Editing
{
    public class ExampleControl : Avalonia.Controls.Control
    {
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            // Example pattern: use GetNewValue<T>() instead of change.NewValue
            if (change.Property == SomeProperty)
            {
                var newValue = change.GetNewValue<int>(); // Replace int with actual type
                // handle property change
            }
        }
    }
}

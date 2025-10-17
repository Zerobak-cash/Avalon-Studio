// PATCH: OnPropertyChanged migration example (replace existing generic overrides)
// Replace signatures like:
// protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
//
// With:
using Avalonia;

namespace Example.Patch
{
    public class OnPropertyChangedExample : Avalonia.Controls.Control
    {
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            // Example: retrieve typed value
            if (change.Property == SomeProperty)
            {
                // use the appropriate type in GetNewValue<T>()
                var newValue = change.GetNewValue<int>();
            }
        }
    }
}

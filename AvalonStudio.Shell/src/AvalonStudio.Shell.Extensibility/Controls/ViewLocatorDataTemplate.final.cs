using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Controls.Templates;

namespace AvalonStudio.Shell.Extensibility.Controls
{
    public class ViewLocatorDataTemplate : IDataTemplate // adapt to non-generic IDataTemplate if needed
    {
        public bool Match(object? data) => true;
        public Control Build(object? param)
        {
            // Build your control here. Minimal placeholder to satisfy compile.
            return new Control();
        }
    }
}

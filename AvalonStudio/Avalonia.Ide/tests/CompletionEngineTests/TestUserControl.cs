using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace CompletionEngineTests
{
    public class TestUserControl : UserControl
    {
        public TestUserControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace CompletionEngineTests
{
    public class TestCompiledTheme : Styles
    {
        public TestCompiledTheme()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
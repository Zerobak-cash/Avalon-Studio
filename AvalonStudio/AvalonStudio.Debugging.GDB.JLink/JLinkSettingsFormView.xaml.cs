using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace AvalonStudio.Debugging.GDB.JLink
{
    public class JLinkSettingsFormView : UserControl
    {
        public JLinkSettingsFormView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
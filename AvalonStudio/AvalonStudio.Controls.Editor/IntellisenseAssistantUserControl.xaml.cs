using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace AvalonStudio.Controls.Editor
{
    public class IntellisenseAssistantUserControl : UserControl
    {
        public IntellisenseAssistantUserControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
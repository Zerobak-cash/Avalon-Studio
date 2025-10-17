// PATCH2 Example: Fix missing GetVisualRoot() by importing Avalonia.VisualTree
using Avalonia.VisualTree;

namespace AvaloniaEdit.Rendering
{
    public class ExampleUsage
    {
        void Example()
        {
            var root = this.GetVisualRoot();
            // root now resolves correctly under Avalonia 11
        }
    }
}

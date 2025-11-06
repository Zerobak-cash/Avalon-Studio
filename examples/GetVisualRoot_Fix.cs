// PATCH: Add using Avalonia.VisualTree for GetVisualRoot() extension method
// Add at top of file:
// using Avalonia.VisualTree;
using Avalonia.VisualTree;

namespace Example.Patch
{
    public class VisualRootExample : Avalonia.Controls.Control
    {
        void Example()
        {
            var root = this.GetVisualRoot();
            // Use root as needed
        }
    }
}

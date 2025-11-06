// PATCH: Default parameter compile-time constant fix example
// Replace signatures with non-constant defaults and set inside the method.
using Avalonia.Threading;

namespace Example.Patch
{
    public class DefaultParamExample
    {
        // before: void Redraw(DispatcherPriority redrawPriority = DispatcherPriority.Render)
        void Redraw(DispatcherPriority? redrawPriority = null)
        {
            redrawPriority ??= DispatcherPriority.Render;
            // ...
        }
    }
}

// PATCH: Pointer event rename example for Avalonia 11
// Replace OnPointerLeave override with OnPointerExited
using Avalonia.Input;

namespace Example.Patch
{
    public class PointerFixExample : Avalonia.Controls.Control
    {
        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);
            // ...
        }
    }
}

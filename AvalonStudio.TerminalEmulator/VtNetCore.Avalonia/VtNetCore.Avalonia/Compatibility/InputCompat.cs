using Avalonia;

namespace AvalonStudio.Compat
{
    // Provide enum members expected by older code
    public enum PointerUpdateKind
    {
        LeftButton,
        RightButton,
        MiddleButton,
        XButton1,
        XButton2,
        Other
    }

    public static class CompatRect
    {
        public static readonly Rect Empty = new Rect(0,0,0,0);
    }
}

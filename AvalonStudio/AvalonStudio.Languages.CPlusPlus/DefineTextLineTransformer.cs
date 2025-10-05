using Avalonia.Media;
using AvaloniaEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AvalonStudio.Languages.CPlusPlus.Rendering
{
    internal class DefineTextLineTransformer : IVisualLineTransformer
    {
        private readonly SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(0xD0, 0xB8, 0x48, 0xFF));
        private readonly IBrush pragmaBrush = Brush.Parse("#9B9B9B");

#pragma warning disable 67

        public event EventHandler<EventArgs> DataChanged;

        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {
            /*var trimmed = line.RenderedText.Text.Trim();

            if (trimmed.StartsWith("#") && !trimmed.StartsWith("#include"))
            {
                var startIndex = line.RenderedText.Text.IndexOf("#");

                var firstEndOffset = line.RenderedText.Text.IndexOf(" ", startIndex);

                line.RenderedText.SetTextStyle(startIndex, firstEndOffset - startIndex, pragmaBrush);

                var lastWordOffset = firstEndOffset != -1 ? line.RenderedText.Text.LastIndexOf(" ", firstEndOffset) + 1 : -1;

                if (lastWordOffset != -1)
                {
                    line.RenderedText.SetTextStyle(lastWordOffset, line.RenderedText.Text.Length - lastWordOffset, brush);
                }
            }*/
        }

#pragma warning restore 67
    }
}
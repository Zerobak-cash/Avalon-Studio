using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using AvalonStudio.TextEditor.Document;
using AvalonStudio.TextEditor.Indentation;
using AvalonStudio.TextEditor.Rendering;
using OmniXaml.Attributes;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace AvalonStudio.TextEditor
{
    [ContentProperty("Content")]
    public class TextEditor : TemplatedControl
    {
        private readonly CompositeDisposable disposables;

        #region Properties

        public TextView TextView { get; private set; }

        #endregion Properties

        public void ScrollToLine(int line)
        {
            TextView.ScrollToLine(line);
        }

        public void Indent(IIndentationStrategy indentationStrategy)
        {
            if (CaretIndex >= 0 && CaretIndex < TextDocument.TextLength)
            {
                if (TextDocument.GetCharAt(CaretIndex) == '}')
                {
                    TextDocument.Insert(CaretIndex, Environment.NewLine);
                    CaretIndex--;

                    var currentLine = TextDocument.GetLineByOffset(CaretIndex);

                    CaretIndex = indentationStrategy.IndentLine(TextDocument, currentLine, CaretIndex);
                    CaretIndex = indentationStrategy.IndentLine(TextDocument, currentLine.NextLine.NextLine, CaretIndex);
                    CaretIndex = indentationStrategy.IndentLine(TextDocument, currentLine.NextLine, CaretIndex);
                }

                var newCaret = indentationStrategy.IndentLine(TextDocument,
                    TextDocument.GetLineByOffset(CaretIndex), CaretIndex);

                CaretIndex = newCaret;
            }
        }

        #region Contructors

        static TextEditor()
        {
            FocusableProperty.OverrideDefaultValue(typeof(TextEditor), true);

            CaretIndexProperty.Changed.AddClassHandler<TextEditor>((s, v) =>
            {
                if (s.TextDocument != null && s.CaretIndex >= 0 && s.TextView != null)
                {
                    s.InvalidateCaretPosition();

                    s.InvalidateSelectedWord();
                }
            });

            HeaderProperty.Changed.AddClassHandler<TextEditor>((s, v) =>
            {
                if (v.OldValue as ILogical != null)
                {
                    s.LogicalChildren.Remove(v.OldValue as ILogical);
                }

                if (v.NewValue as ILogical != null)
                {
                    s.LogicalChildren.Add(v.NewValue as ILogical);
                }
            });

            ContentProperty.Changed.AddClassHandler<TextEditor>((s, v) =>
            {
                if (v.OldValue as ILogical != null)
                {
                    s.LogicalChildren.Remove(v.OldValue as ILogical);
                }

                if (v.NewValue as ILogical != null)
                {
                    s.LogicalChildren.Add(v.NewValue as ILogical);
                }
            });

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                Application.Current.Styles.Add(new TextEditorTheme());
            });
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            var canScrollHorizontally = this.GetObservable(AcceptsReturnProperty)
                .Select(x => !x);

            var horizontalScrollBarVisibility = this.GetObservable(AcceptsReturnProperty)
                .Select(x => x ? ScrollBarVisibility.Auto : ScrollBarVisibility.Hidden);

            disposables.Add(Bind(
                ScrollViewer.HorizontalScrollBarVisibilityProperty,
                horizontalScrollBarVisibility,
                BindingPriority.Style));

            disposables.Add(TextDocumentProperty.Changed.Subscribe(_ => { SelectionStart = SelectionEnd = CaretIndex = -1; }));

            disposables.Add(this.GetObservable(OffsetProperty).Subscribe(_ =>
            {
                EditorScrolled?.Invoke(this, new EventArgs());
            }));

            disposables.Add(AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble));
        }

        public event EventHandler<EventArgs> EditorScrolled;

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            TextView = null;
            TextDocument = null;
            disposables.Dispose();
        }

        ~TextEditor()
        {
        }

        public TextEditor()
        {
            disposables = new CompositeDisposable();

            Name = "textEditor";
            highestUserSelectedColumn = 1;
        }

        #endregion Contructors

        #region Private Data

        private int highestUserSelectedColumn;

        #endregion Private Data

        #region Pespex Properties

        public static readonly AvaloniaProperty<string> TabCharacterProperty =
            AvaloniaProperty.Register<TextEditor, string>(nameof(TabCharacter), "    ");

        public string TabCharacter
        {
            get { return GetValue(TabCharacterProperty); }
            set { SetValue(TabCharacterProperty, value); }
        }

        public static readonly AvaloniaProperty<int> MouseCursorOffsetProperty =
            AvaloniaProperty.Register<TextEditor, int>(nameof(MouseCursorOffset));

        public int MouseCursorOffset
        {
            get { return GetValue(MouseCursorOffsetProperty); }
            set { SetValue(MouseCursorOffsetProperty, value); }
        }

        public static readonly AvaloniaProperty<Point> MouseCursorPositionProperty =
            AvaloniaProperty.Register<TextEditor, Point>(nameof(MouseCursorPosition), defaultBindingMode: BindingMode.TwoWay);

        public Point MouseCursorPosition
        {
            get { return GetValue(MouseCursorPositionProperty); }
            set { SetValue(MouseCursorPositionProperty, value); }
        }

        public static readonly AvaloniaProperty<string> SelectedWordProperty =
            AvaloniaProperty.Register<TextEditor, string>(nameof(SelectedWord), string.Empty,
                defaultBindingMode: BindingMode.TwoWay);

        public string SelectedWord
        {
            get { return GetValue(SelectedWordProperty); }
            set { SetValue(SelectedWordProperty, value); }
        }

        public static readonly AvaloniaProperty<double> LineHeightProperty =
            AvaloniaProperty.Register<TextEditor, double>(nameof(LineHeight), defaultBindingMode: BindingMode.TwoWay);

        public double LineHeight
        {
            get { return GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public static readonly StyledProperty<ICommand> BeforeTextChangedCommandProperty =
            TextView.BeforeTextChangedCommandProperty.AddOwner<TextEditor>();

        public ICommand BeforeTextChangedCommand
        {
            get { return GetValue(BeforeTextChangedCommandProperty); }
            set { SetValue(BeforeTextChangedCommandProperty, value); }
        }

        public static readonly StyledProperty<object> ContentProperty = ContentControl.ContentProperty.AddOwner<TextEditor>();

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        ///     Defines the <see cref="Header" /> property.
        /// </summary>
        public static readonly StyledProperty<object> HeaderProperty =
            AvaloniaProperty.Register<TextEditor, object>(nameof(Header));

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly StyledProperty<ObservableCollection<TextViewMargin>> MarginsProperty =
            TextView.MarginsProperty.AddOwner<TextEditor>();

        public ObservableCollection<TextViewMargin> Margins
        {
            get { return GetValue(MarginsProperty); }
            set { SetValue(MarginsProperty, value); }
        }

        public static readonly AvaloniaProperty<ObservableCollection<IBackgroundRenderer>> BackgroundRenderersProperty =
            TextView.BackgroundRenderersProperty.AddOwner<TextEditor>();

        public ObservableCollection<IBackgroundRenderer> BackgroundRenderers
        {
            get { return GetValue(BackgroundRenderersProperty); }
            set { SetValue(BackgroundRenderersProperty, value); }
        }

        public static readonly AvaloniaProperty<ObservableCollection<IDocumentLineTransformer>>
            DocumentLineTransformersProperty =
                TextView.DocumentLineTransformersProperty.AddOwner<TextEditor>();

        public ObservableCollection<IDocumentLineTransformer> DocumentLineTransformers
        {
            get { return GetValue(DocumentLineTransformersProperty); }
            set { SetValue(DocumentLineTransformersProperty, value); }
        }

        public static readonly AvaloniaProperty<ICommand> TextChangedCommandProperty =
            TextView.TextChangedCommandProperty.AddOwner<TextEditor>();

        public ICommand TextChangedCommand
        {
            get { return GetValue(TextChangedCommandProperty); }
            set { SetValue(TextChangedCommandProperty, value); }
        }

        public static readonly AvaloniaProperty<bool> AcceptsReturnProperty =
            AvaloniaProperty.Register<TextEditor, bool>(nameof(AcceptsReturn));

        public bool AcceptsReturn
        {
            get { return GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        public static readonly AvaloniaProperty<bool> AcceptsTabProperty =
            AvaloniaProperty.Register<TextEditor, bool>(nameof(AcceptsTab));

        public bool AcceptsTab
        {
            get { return GetValue(AcceptsTabProperty); }
            set { SetValue(AcceptsTabProperty, value); }
        }

        public static readonly AvaloniaProperty<int> CaretIndexProperty =
            TextView.CaretIndexProperty.AddOwner<TextEditor>();

        public int CaretIndex
        {
            get
            {
                return GetValue(CaretIndexProperty);
            }
            set
            {
                SetValue(CaretIndexProperty, value);
            }
        }

        public static readonly AvaloniaProperty<Point> CaretLocationProperty =
            AvaloniaProperty.Register<TextEditor, Point>(nameof(CaretLocation), defaultBindingMode: BindingMode.TwoWay);

        public Point CaretLocation
        {
            get { return GetValue(CaretLocationProperty); }
            set { SetValue(CaretLocationProperty, value); }
        }

        public static readonly AvaloniaProperty<Point> CaretLocationInTextViewProperty =
            AvaloniaProperty.Register<TextEditor, Point>(nameof(CaretLocationInTextView), defaultBindingMode: BindingMode.TwoWay);

        public Point CaretLocationInTextView
        {
            get { return GetValue(CaretLocationInTextViewProperty); }
            set { SetValue(CaretLocationInTextViewProperty, value); }
        }

        public static readonly AvaloniaProperty<int> SelectionStartProperty =
            AvaloniaProperty.Register<TextEditor, int>(nameof(SelectionStart));

        public int SelectionStart
        {
            get { return GetValue(SelectionStartProperty); }
            set { SetValue(SelectionStartProperty, value); }
        }

        public static readonly AvaloniaProperty<int> SelectionEndProperty =
            AvaloniaProperty.Register<TextEditor, int>(nameof(SelectionEnd));

        public int SelectionEnd
        {
            get { return GetValue(SelectionEndProperty); }
            set { SetValue(SelectionEndProperty, value); }
        }

        public TextSegment GetSelectionAsSegment()
        {
            TextSegment result = null;

            if (SelectionStart < SelectionEnd)
            {
                result = new TextSegment { StartOffset = SelectionStart, EndOffset = SelectionEnd };
            }
            else
            {
                result = new TextSegment { StartOffset = SelectionEnd, EndOffset = SelectionStart };
            }

            return result;
        }

        public void SetSelection(TextSegment segment)
        {
            SelectionStart = segment.StartOffset;
            SelectionEnd = segment.EndOffset;
        }

        public static readonly AvaloniaProperty<IIndentationStrategy> IndentationStrategyProperty =
            AvaloniaProperty.Register<TextEditor, IIndentationStrategy>(nameof(IndentationStrategy));

        public IIndentationStrategy IndentationStrategy
        {
            get { return GetValue(IndentationStrategyProperty); }
            set { SetValue(IndentationStrategyProperty, value); }
        }

        public static readonly AvaloniaProperty<TextDocument> TextDocumentProperty =
            TextView.TextDocumentProperty.AddOwner<TextEditor>();

        public TextDocument TextDocument
        {
            get { return GetValue(TextDocumentProperty); }
            set { SetValue(TextDocumentProperty, value); }
        }

        public static readonly AvaloniaProperty<Vector> OffsetProperty =
            TextView.OffsetProperty.AddOwner<TextEditor>(o => o.Offset,
                (o, v) => o.Offset = v);

        private Vector offset;

        public Vector Offset
        {
            get
            {
                return offset;
            }
            set
            {
                if (value.Y != offset.Y || value.X != offset.X)
                {
                    SetAndRaise(OffsetProperty, ref offset, value);
                }
            }
        }

        #endregion Pespex Properties

        #region Private Methods

        private void InvalidateCaretPosition()
        {
            CaretLocation = VisualLineGeometryBuilder.GetViewPortPosition(TextView, CaretIndex).TopLeft;
            var textViewCaretLocation = VisualLineGeometryBuilder.GetTextViewPosition(TextView, CaretIndex).TopLeft;
            CaretLocationInTextView = new Point(textViewCaretLocation.X, textViewCaretLocation.Y + TextView.CharSize.Height);
        }

        public string GetPreviousWordAtIndex(int index)
        {
            var lastWordIndex = TextUtilities.GetNextCaretPosition(TextDocument, index, TextUtilities.LogicalDirection.Backward, TextUtilities.CaretPositioningMode.WordBorder);

            if (lastWordIndex >= 0 && TextDocument.GetLocation(lastWordIndex).Line == TextDocument.GetLocation(index).Line)
            {
                return GetWordAtIndex(lastWordIndex);
            }
            else
            {
                return GetWordAtIndex(index);
            }
        }

        public string GetWordAtIndex(int index)
        {
            var result = string.Empty;

            if (index >= 0 && TextDocument.TextLength > index)
            {
                var start = index;

                var currentChar = TextDocument.GetCharAt(index);
                var prevChar = '\0';

                if (index > 0)
                {
                    prevChar = TextDocument.GetCharAt(index - 1);
                }

                var charClass = TextUtilities.GetCharacterClass(currentChar);

                if (charClass != TextUtilities.CharacterClass.LineTerminator && prevChar != ' ' &&
                    TextUtilities.GetCharacterClass(prevChar) != TextUtilities.CharacterClass.LineTerminator)
                {
                    start = TextUtilities.GetNextCaretPosition(TextDocument, index, TextUtilities.LogicalDirection.Backward,
                        TextUtilities.CaretPositioningMode.WordStart);
                }

                var end = TextUtilities.GetNextCaretPosition(TextDocument, start, TextUtilities.LogicalDirection.Forward,
                    TextUtilities.CaretPositioningMode.WordBorder);

                if (start != -1 && end != -1)
                {
                    var word = TextDocument.GetText(start, end - start).Trim();

                    if (TextUtilities.IsSymbol(word))
                    {
                        result = word;
                    }
                }
            }

            return result;
        }

        private void InvalidateSelectedWord()
        {
            SelectedWord = GetWordAtIndex(CaretIndex);
        }

        private void HandleTextInput(string input)
        {
            InvalidateSelectedWord();

            if (!string.IsNullOrEmpty(input))
            {
                TextDocument.BeginUpdate();

                DeleteSelection();

                var caretIndex = CaretIndex;

                if (caretIndex >= 0)
                {
                    TextDocument.Insert(caretIndex, input);
                }

                TextDocument.EndUpdate();

                CaretIndex += input.Length;
                SelectionStart = SelectionEnd = CaretIndex;
                TextView.Invalidate();
            }
        }

        private void SelectAll()
        {
            SelectionStart = 0;
            SelectionEnd = TextDocument.TextLength;
        }

        private bool DeleteSelection()
        {
            var selectionStart = SelectionStart;
            var selectionEnd = SelectionEnd;

            if (selectionStart != selectionEnd)
            {
                var start = Math.Min(selectionStart, selectionEnd);
                var end = Math.Max(selectionStart, selectionEnd);
                TextDocument.Remove(start, end - start);
                TextView.Invalidate();

                SelectionStart = SelectionEnd = CaretIndex = start;

                return true;
            }
            return false;
        }

        private string GetSelection()
        {
            var selectionStart = SelectionStart;
            var selectionEnd = SelectionEnd;
            var start = Math.Min(selectionStart, selectionEnd);
            var end = Math.Max(selectionStart, selectionEnd);

            if (start == end || (TextDocument?.TextLength ?? 0) < end)
            {
                return "";
            }

            return TextDocument.GetText(start, end - start);
        }

        private void SetHighestColumn()
        {
            if (CaretIndex != -1)
            {
                highestUserSelectedColumn = TextDocument.GetLocation(CaretIndex).Column;
            }
        }

        private void MoveToWordBoundard(int count, int caretIndex)
        {
            if (count > 0)
            {
                count =
                    TextUtilities.GetNextCaretPosition(TextDocument, caretIndex, TextUtilities.LogicalDirection.Forward,
                        TextUtilities.CaretPositioningMode.WordStartOrSymbol) - caretIndex;
            }
            else
            {
                count =
                    TextUtilities.GetNextCaretPosition(TextDocument, caretIndex, TextUtilities.LogicalDirection.Backward,
                        TextUtilities.CaretPositioningMode.WordStartOrSymbol) - caretIndex;
            }

            if (caretIndex + count <= TextDocument.TextLength && caretIndex + count >= 0)
            {
                CaretIndex += count;
            }
        }

        private int MoveForward(int count, int caretIndex)
        {
            for (var i = 0; i < Math.Abs(count); i++)
            {
                var line = TextDocument.GetLineByOffset(caretIndex);

                if (caretIndex == line.EndOffset)
                {
                    if (line.NextLine != null)
                    {
                        caretIndex = line.NextLine.Offset;
                    }
                }
                else
                {
                    caretIndex = TextUtilities.GetNextCaretPosition(TextDocument, caretIndex, TextUtilities.LogicalDirection.Forward,
                        TextUtilities.CaretPositioningMode.Normal);
                }
            }

            return caretIndex;
        }

        private int MoveBackward(int count, int caretIndex)
        {
            for (var i = 0; i < Math.Abs(count); i++)
            {
                var line = TextDocument.GetLineByOffset(caretIndex);

                if (caretIndex == line.Offset)
                {
                    if (line.PreviousLine != null)
                    {
                        caretIndex = line.PreviousLine.EndOffset;
                    }
                }
                else
                {
                    caretIndex = TextUtilities.GetNextCaretPosition(TextDocument, caretIndex,
                        TextUtilities.LogicalDirection.Backward, TextUtilities.CaretPositioningMode.Normal);
                }
            }

            return caretIndex;
        }

        private void MoveHorizontal(int count, InputModifiers modifiers)
        {
            var caretIndex = CaretIndex;

            if (caretIndex > TextDocument.TextLength)
            {
                caretIndex = TextDocument.TextLength;
            }

            if (caretIndex >= 0)
            {
                if ((modifiers & InputModifiers.Control) != 0)
                {
                    MoveToWordBoundard(count, caretIndex);
                }
                else
                {
                    if (count > 0)
                    {
                        CaretIndex = MoveForward(count, caretIndex);
                    }
                    else
                    {
                        CaretIndex = MoveBackward(count, caretIndex);
                    }
                }

                SetHighestColumn();
            }
        }

        private void MoveVertical(int count, InputModifiers modifiers)
        {
            var caretIndex = CaretIndex;

            if (caretIndex >= 0)
            {
                var currentPosition = TextDocument.GetLocation(caretIndex);

                if (currentPosition.Line + count > 0 && currentPosition.Line + count <= TextDocument.LineCount)
                {
                    var line = TextDocument.Lines[currentPosition.Line - 1 + count];

                    var col = line.EndOffset;

                    if (highestUserSelectedColumn <= line.Length)
                    {
                        col = highestUserSelectedColumn;
                    }

                    CaretIndex = TextDocument.GetOffset(currentPosition.Line + count, col);
                }
            }
        }

        private void MoveHome(InputModifiers modifiers)
        {
            var text = TextDocument ?? null;
            var caretIndex = CaretIndex;

            if (caretIndex >= 0)
            {
                if ((modifiers & InputModifiers.Control) != 0)
                {
                    caretIndex = 0;
                }
                else
                {
                    var lineOffset = TextDocument.GetLineByOffset(CaretIndex).Offset;
                    var whiteSpace = TextUtilities.GetWhitespaceAfter(TextDocument, lineOffset);
                    caretIndex = lineOffset + whiteSpace.Length;
                }

                CaretIndex = caretIndex;
                SetHighestColumn();
            }
        }

        private void MoveEnd(InputModifiers modifiers)
        {
            var text = TextDocument ?? null;
            var caretIndex = CaretIndex;

            if (caretIndex >= 0)
            {
                if ((modifiers & InputModifiers.Control) != 0)
                {
                    caretIndex = TextDocument.TextLength;
                }
                else
                {
                    var lineOffset = TextDocument.GetLineByOffset(CaretIndex).EndOffset;
                    var whiteSpace = TextUtilities.GetWhitespaceBefore(TextDocument, lineOffset);
                    caretIndex = lineOffset - whiteSpace.Length;
                }

                CaretIndex = caretIndex;
                SetHighestColumn();
            }
        }

        private async void Cut()
        {
            await ((IClipboard)AvaloniaLocator.Current.GetService(typeof(IClipboard)))
                .SetTextAsync(GetSelection());

            DeleteSelection();
        }

        private async void Copy()
        {
            await ((IClipboard)AvaloniaLocator.Current.GetService(typeof(IClipboard)))
                .SetTextAsync(GetSelection());
        }

        private async void Paste()
        {
            var text = await ((IClipboard)AvaloniaLocator.Current.GetService(typeof(IClipboard))).GetTextAsync();
            if (text == null)
            {
                return;
            }

            HandleTextInput(text.Replace("\t", TabCharacter));
        }

        private void Undo()
        {
            TextDocument?.UndoStack.Undo();
        }

        private void Redo()
        {
            TextDocument?.UndoStack.Redo();
        }

        private sealed class RestoreCaretAndSelectionUndoAction : IUndoableOperation
        {
            private readonly int caretPosition;
            private readonly int selectionEnd;
            private readonly int selectionStart;

            // keep textarea in weak reference because the IUndoableOperation is stored with the document
            private readonly WeakReference textAreaReference;

            public RestoreCaretAndSelectionUndoAction(TextEditor editor)
            {
                textAreaReference = new WeakReference(editor);
                // Just save the old caret position, no need to validate here.
                // If we restore it, we'll validate it anyways.
                caretPosition = editor.CaretIndex;
                selectionStart = editor.SelectionStart;
                selectionEnd = editor.SelectionEnd;
            }

            public void Undo()
            {
                var textEditor = (TextEditor)textAreaReference.Target;
                if (textEditor != null)
                {
                    textEditor.CaretIndex = caretPosition;
                    textEditor.SelectionStart = selectionStart;
                    textEditor.SelectionEnd = selectionEnd;
                }
            }

            public void Redo()
            {
                // redo=undo: we just restore the caret/selection state
                Undo();
            }
        }

        #endregion Private Methods

        #region Overrides

        protected override void OnTemplateApplied(TemplateAppliedEventArgs e)
        {
            TextView = e.NameScope.Find<TextView>("textView");

            LogicalChildren.Add(TextView);

            disposables.Add(TextDocumentProperty.Changed.Subscribe(args =>
            {
                if (args.NewValue != null)
                {
                    // Todo unsubscribe these events.
                    TextDocument.Changing += (sender, ee) =>
                    {
                        TextDocument?.UndoStack.PushOptional(new RestoreCaretAndSelectionUndoAction(this));

                        if (BeforeTextChangedCommand != null)
                        {
                            BeforeTextChangedCommand.Execute(null);
                        }
                    };

                    TextDocument.Changed += (sender, ee) =>
                    {
                        InvalidateVisual();

                        LineHeight = TextView.CharSize.Height;

                        if (TextChangedCommand != null && TextChangedCommand.CanExecute(null))
                        {
                            TextChangedCommand.Execute(null);
                        }
                    };
                }
            }));
        }

        public event EventHandler<EventArgs> CaretChangedByPointerClick;

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.Source.InteractiveParent.InteractiveParent == TextView)
            {
                var point = e.GetPosition(TextView.TextSurface);

                var index = TextView.GetOffsetFromPoint(point);

                if (index != -1)
                {
                    CaretIndex = index;

                    var text = TextDocument;

                    switch (e.ClickCount)
                    {
                        case 1:
                            SelectionStart = SelectionEnd = index;
                            break;

                        case 2:
                            SelectionStart = TextUtilities.GetNextCaretPosition(TextDocument, index, TextUtilities.LogicalDirection.Backward,
                                TextUtilities.CaretPositioningMode.WordStart);

                            SelectionEnd = TextUtilities.GetNextCaretPosition(TextDocument, index, TextUtilities.LogicalDirection.Forward,
                                TextUtilities.CaretPositioningMode.WordBorder);
                            break;

                        case 3:
                            SelectionStart = 0;
                            SelectionEnd = text.TextLength;
                            break;
                    }

                    e.Device.Capture(TextView);
                    e.Handled = true;

                    InvalidateVisual();

                    if (CaretChangedByPointerClick != null)
                    {
                        CaretChangedByPointerClick(this, e);
                    }
                }
                else if (TextDocument?.TextLength == 0)
                {
                    SelectionStart = SelectionEnd = CaretIndex = 0;

                    e.Device.Capture(TextView);
                    e.Handled = true;

                    InvalidateVisual();
                }

                SetHighestColumn();
            }
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            // Need to check this incase control was virtualized?
            if (TextView != null)
            {
                var point = e.GetPosition(TextView.TextSurface);

                if (e.Device.Captured == TextView)
                {
                    var currentMouseOffset = TextView.GetOffsetFromPoint(point);

                    if (currentMouseOffset != -1)
                    {
                        CaretIndex = currentMouseOffset;

                        if (CaretIndex >= 0)
                        {
                            SelectionEnd = CaretIndex;
                        }
                        else
                        {
                            SelectionEnd = 0;
                        }
                    }
                }
            }
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (e.Device.Captured == TextView)
            {
                e.Device.Capture(null);
            }
        }

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            TextView.ShowCaret();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            TextView?.HideCaret();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            HandleTextInput(e.Text);
        }

        private void TransformSelectedLines(Action<IDocumentLine> transformLine)
        {
            var selection = GetSelectionAsSegment();
            var lines = VisualLineGeometryBuilder.GetLinesForSegmentInDocument(TextDocument, selection);

            if (lines.Count() > 0)
            {
                var anchors = new TextSegmentCollection<TextSegment>(TextDocument);

                anchors.Add(selection);

                TextDocument.BeginUpdate();

                foreach (var line in lines)
                {
                    transformLine(line);
                }

                TextDocument.EndUpdate();

                SetSelection(selection);
            }
        }

        private void OnBackKey()
        {
            // TODO use thread-safe copy of caret index.
            if (!DeleteSelection() && CaretIndex > 0)
            {
                var line = TextDocument.GetLineByOffset(CaretIndex);

                if (CaretIndex == line.Offset && line.PreviousLine != null)
                {
                    var delimiterLength = line.PreviousLine.DelimiterLength;
                    TextDocument.Remove(CaretIndex - delimiterLength, delimiterLength);
                    CaretIndex -= delimiterLength;
                }
                else
                {
                    TextDocument.Remove(CaretIndex - 1, 1);
                    --CaretIndex;
                }

                TextView.Invalidate();
            }
        }

        private void OnTabKey(KeyEventArgs e)
        {
            var shiftedLines = false;

            if (SelectionStart != SelectionEnd)
            {
                if (e.Modifiers == InputModifiers.Shift)
                {
                    var selection = GetSelectionAsSegment();
                    var lines = VisualLineGeometryBuilder.GetLinesForSegmentInDocument(TextDocument, selection);

                    if (lines.Count() > 1)
                    {
                        TransformSelectedLines(line =>
                        {
                            var offset = line.Offset;
                            var s = TextUtilities.GetSingleIndentationSegment(TextDocument, offset, TabCharacter.Length);

                            if (s.Length > 0)
                            {
                                TextDocument.Remove(s.Offset, s.Length);
                            }
                        });
                    }
                }
                else
                {
                    var selection = GetSelectionAsSegment();
                    var lines = VisualLineGeometryBuilder.GetLinesForSegmentInDocument(TextDocument, selection);

                    if (lines.Count() > 1)
                    {
                        TransformSelectedLines(line => { TextDocument.Insert(line.Offset, TabCharacter); });

                        shiftedLines = true;
                    }
                }
            }

            if (!shiftedLines)
            {
                if (e.Modifiers == InputModifiers.Shift)
                {
                    TransformSelectedLines(line =>
                    {
                        var offset = CaretIndex - TabCharacter.Length;
                        var s = TextUtilities.GetSingleIndentationSegment(TextDocument, offset, TabCharacter.Length);

                        if (s.Length > 0)
                        {
                            TextDocument.Remove(s.Offset, s.Length);
                        }
                    });
                }
                else
                {
                    HandleTextInput(TabCharacter);
                }
            }
        }

        private void OnDeleteKey(int caretIndex)
        {
            if (!DeleteSelection() && caretIndex < TextDocument.TextLength)
            {
                var line = TextDocument.GetLineByOffset(caretIndex);

                if (CaretIndex == line.EndOffset && line.NextLine != null)
                {
                    TextDocument.Remove(caretIndex, line.DelimiterLength);
                }
                else
                {
                    TextDocument.Remove(caretIndex, 1);
                }

                TextView.Invalidate();
            }
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }

            var caretIndex = CaretIndex;
            var movement = false;
            var handled = true;
            var modifiers = e.Modifiers;

            switch (e.Key)
            {
                case Avalonia.Input.Key.OemPlus:
                    if (modifiers == InputModifiers.Control)
                    {
                        if (TextView.FontSize < 60)
                        {
                            TextView.FontSize++;
                        }
                    }
                    break;

                case Avalonia.Input.Key.OemMinus:
                    if (modifiers == InputModifiers.Control)
                    {
                        if (TextView.FontSize > 1)
                        {
                            TextView.FontSize--;
                        }
                    }
                    break;

                case Avalonia.Input.Key.A:
                    if (modifiers == InputModifiers.Control)
                    {
                        SelectAll();
                    }
                    break;

                case Avalonia.Input.Key.C:
                    if (modifiers == InputModifiers.Control)
                    {
                        Copy();
                    }
                    break;

                case Avalonia.Input.Key.V:
                    if (modifiers == InputModifiers.Control)
                    {
                        Paste();
                    }
                    break;

                case Avalonia.Input.Key.X:
                    if (modifiers == InputModifiers.Control)
                    {
                        Cut();
                    }
                    break;

                case Avalonia.Input.Key.Y:
                    if (modifiers == InputModifiers.Control)
                    {
                        Redo();
                    }
                    break;

                case Avalonia.Input.Key.Z:
                    if (modifiers == InputModifiers.Control)
                    {
                        Undo();
                    }
                    break;

                case Avalonia.Input.Key.Left:
                    MoveHorizontal(-1, modifiers);
                    movement = true;
                    break;

                case Avalonia.Input.Key.Right:
                    MoveHorizontal(1, modifiers);
                    movement = true;
                    break;

                case Avalonia.Input.Key.Up:
                    MoveVertical(-1, modifiers);
                    movement = true;
                    break;

                case Avalonia.Input.Key.Down:
                    MoveVertical(1, modifiers);
                    movement = true;
                    break;

                case Avalonia.Input.Key.Home:
                    MoveHome(modifiers);
                    movement = true;
                    break;

                case Avalonia.Input.Key.End:
                    MoveEnd(modifiers);
                    movement = true;
                    break;

                case Avalonia.Input.Key.Back:
                    OnBackKey();
                    break;

                case Avalonia.Input.Key.Delete:
                    OnDeleteKey(caretIndex);
                    break;

                case Avalonia.Input.Key.Enter:
                    if (AcceptsReturn)
                    {
                        HandleTextInput("\r\n");
                    }
                    break;

                case Avalonia.Input.Key.Tab:
                    if (AcceptsTab)
                    {
                        e.Handled = true;

                        OnTabKey(e);
                    }
                    else
                    {
                        base.OnKeyDown(e);
                        handled = false;
                    }

                    break;

                case Avalonia.Input.Key.PageUp:
                    TextView.PageUp();
                    break;

                case Avalonia.Input.Key.PageDown:
                    TextView.PageDown();
                    break;
            }

            if (movement && ((modifiers & InputModifiers.Shift) != 0))
            {
                SelectionEnd = CaretIndex;
            }
            else if (movement)
            {
                SelectionStart = SelectionEnd = CaretIndex;
            }

            if (handled)
            {
                InvalidateVisual();
            }
        }

        #endregion Overrides
    }
}
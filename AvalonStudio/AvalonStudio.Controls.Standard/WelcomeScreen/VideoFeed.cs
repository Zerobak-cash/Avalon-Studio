namespace AvalonStudio.Controls.Standard.WelcomeScreen
{
    using Avalonia.Media.Imaging;
    using AvalonStudio.MVVM;
    using ReactiveUI;
    using System;
    using System.Reactive;

    public class VideoFeedViewModel : ViewModel
    {
        private string _title;
        private string _url;
        private Bitmap _image;

        public VideoFeedViewModel(string url, string title, Bitmap image = null)
        {
            _url = url;
            _title = title;
            _image = image;

            ClickCommand = ReactiveCommand.Create(() =>
            {
                System.Diagnostics.Process.Start(url);
            });
        }

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public Bitmap Image
        {
            get { return _image; }
            set { this.RaiseAndSetIfChanged(ref _image, value); }
        }

        public ReactiveCommand<Unit, Unit> ClickCommand { get; }
    }
}
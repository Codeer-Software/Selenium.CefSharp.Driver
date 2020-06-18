using System.Windows;

namespace CefSharpWPFTarget
{
    public partial class MainWindow : Window
    {
        public bool IsLoadEnded { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _browser.FrameLoadEnd += (_, __) => IsLoadEnded = true;
        }
    }
}

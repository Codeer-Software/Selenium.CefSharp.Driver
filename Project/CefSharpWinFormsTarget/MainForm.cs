using System.Windows.Forms;

namespace CefSharpWinFormsTarget
{
    public partial class MainForm : Form
    {
        CefSharp.WinForms.ChromiumWebBrowser _browser = new CefSharp.WinForms.ChromiumWebBrowser("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
      
        public bool IsLoadEnded { get; set; }

        public MainForm()
        {
            InitializeComponent();
            _browser.Dock = DockStyle.Fill;
            Controls.Add(_browser);
            _browser.FrameLoadEnd += (_, __) => IsLoadEnded = true;
        }
    }
}

using System.Windows.Forms;

namespace CefSharpWinFormsSample
{
    public partial class MainForm : Form
    {
        CefSharp.WinForms.ChromiumWebBrowser _browser = new CefSharp.WinForms.ChromiumWebBrowser("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
      
        public MainForm()
        {
            InitializeComponent();

            _browser.Dock = DockStyle.Fill;
            _panel.Controls.Add(_browser);
        }

        private void _buttonURL_Click(object sender, System.EventArgs e)
        {
            _browser.Load(@"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Controls.html");
        }
    }
}

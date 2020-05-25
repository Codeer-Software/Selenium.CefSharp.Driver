using System.Windows;

namespace SampleApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
            => InitializeComponent();

        void _buttonNextDialog_Click(object sender, RoutedEventArgs e)
            => new NextDialog().ShowDialog();
    }
}

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace DemoGame.Editor.Avalonia.Views
{
    /// <summary>
    /// About dialog showing version and credits
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void Close_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}


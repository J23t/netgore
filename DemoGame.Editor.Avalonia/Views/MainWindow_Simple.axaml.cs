using Avalonia.Controls;

namespace DemoGame.Editor.Avalonia.Views
{
    /// <summary>
    /// Simplified version of MainWindow - just TabControl to test panels work
    /// Once we verify panels load correctly, we'll add full docking back
    /// </summary>
    public partial class MainWindow_Simple : Window
    {
        public MainWindow_Simple()
        {
            InitializeComponent();
        }
    }
}


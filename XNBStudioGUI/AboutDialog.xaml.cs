using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace XNBStudioGUI;

public partial class AboutDialog : Window
{
    public AboutDialog()
    {
        InitializeComponent();
    }

    private void HandleCloseDialog(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void HandleOpenLink(object sender, RequestNavigateEventArgs e)
    {
        var startInfo = new ProcessStartInfo(e.Uri.AbsoluteUri)
        {
            UseShellExecute = true
        };
        
        Process.Start(startInfo);
        e.Handled = true;
    }
}
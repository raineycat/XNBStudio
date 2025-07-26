using System.Windows;
using Microsoft.Win32;

namespace XNBStudioGUI;

public static class ToolHelpers
{
    private static readonly Guid DialogId = Guid.Parse("879C96CE-CE33-4B58-8E11-58634EB108EE");

    public static bool ShowDialog<T>(this Window owner) where T : Window, new()
    {
        var dlg = new T
        {
            Owner = owner
        };
        
        return dlg.ShowDialog() ?? false;
    }

    public static MessageBoxResult ShowMessageBox(this Window owner, 
                                                  string text, 
                                                  string caption = "XNB Studio", 
                                                  MessageBoxButton buttons = MessageBoxButton.OK, 
                                                  MessageBoxImage image = MessageBoxImage.Information,
                                                  MessageBoxResult defaultResult = MessageBoxResult.OK,
                                                  MessageBoxOptions options = MessageBoxOptions.None)
    {
        return MessageBox.Show(owner, text, caption, buttons, image, defaultResult, options);
    }
    
    public static string? OpenFile(this Window owner, string type, string filter)
    {
        var dlg = new OpenFileDialog
        {
            Title = $"Select {type}...",
            Filter = $"{type} ({filter})|{filter}|All files|*.*",
            ClientGuid = DialogId,
            CheckFileExists = true
        };

        return dlg.ShowDialog(owner).GetValueOrDefault() ? dlg.FileName : null;
    }
    
    public static IEnumerable<string> OpenFileMulti(this Window owner, string type, string filter)
    {
        var dlg = new OpenFileDialog
        {
            Title = $"Select {type}...",
            Filter = $"{type} ({filter})|{filter}|All files|*.*",
            ClientGuid = DialogId,
            CheckFileExists = true,
            Multiselect = true
        };

        return dlg.ShowDialog(owner).GetValueOrDefault() ? dlg.FileNames : Enumerable.Empty<string>();
    }
    
    public static string? SaveFile(this Window owner, string type, string filter, string filename = "")
    {
        var dlg = new SaveFileDialog
        {
            Title = $"Save {type}...",
            Filter = $"{type} ({filter})|{filter}|All files|*.*",
            ClientGuid = DialogId,
            CheckFileExists = false,
            FileName = filename
        };

        return dlg.ShowDialog(owner).GetValueOrDefault() ? dlg.FileName : null;
    }
}
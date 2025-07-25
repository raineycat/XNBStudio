using System.Windows.Input;

namespace XNBStudioGUI;

public static class StudioCommands
{
    public static RoutedUICommand CloseFile = new(
        "Close file",
        nameof(CloseFile),
        typeof(StudioCommands),
        [new KeyGesture(Key.C, ModifierKeys.Alt)]
    );
    
    public static RoutedUICommand CloseAllFiles = new(
        "Close all files",
        nameof(CloseAllFiles),
        typeof(StudioCommands),
        [new KeyGesture(Key.C, ModifierKeys.Alt | ModifierKeys.Shift)]
    );
}
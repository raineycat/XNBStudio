using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace XNBStudioGUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : INotifyPropertyChanged
{
    private const string _dynamicPanelTag = "[DynamicContentPanel]";
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public ObservableCollection<LoadedFile> LoadedFiles { get; set; } = [];

    private LoadedFile? _selectedFile;
    public LoadedFile? SelectedFile
    {
        get => _selectedFile;
        set
        {
            _selectedFile = value;
            OnPropertyChanged();
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void ExecuteOpenCommand(object sender, ExecutedRoutedEventArgs e)
    {
        var paths = this.OpenFileMulti("XNB files", "*.xnb").ToArray();
        if (paths.Length == 0)
        {
            return;
        }
        
        int success = 0, fail = 0;
        foreach (var path in paths)
        {
            var file = LoadedFile.Open(path);
            if (file.Data != null) success++;
            if (file.Error != null) fail++;
            LoadedFiles.Add(file);
        }

        var msg = new StringBuilder()
            .Append("Finished loading ")
            .Append(success + fail)
            .AppendLine(" files:")
            .Append(success)
            .Append(" loaded successfully");

        if (fail > 0)
        {
            msg
                .Append("; ")
                .Append(fail)
                .AppendLine(" failed to load.")
                .AppendLine()
                .AppendLine("See the individual files in the left pane for detailed error info");
        }

        this.ShowMessageBox(msg.ToString(), 
            image: fail > 0 ? MessageBoxImage.Exclamation : MessageBoxImage.Asterisk);
    }
    
    private void ExecuteHelpCommand(object sender, ExecutedRoutedEventArgs e)
    {
        this.ShowDialog<AboutDialog>();
    }
    
    private void ExecuteCloseCommand(object sender, ExecutedRoutedEventArgs e)
    {
        Close();
    }
    
    private void ExecuteCloseFileCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if (LoadedFileList.SelectedItem is LoadedFile f)
        {
            f.Data?.Dispose();
            LoadedFiles.Remove(f);
            SelectedFile = null;
            ClearDynamicEditor();
        }
    }
    
    private void ExecuteCloseAllFilesCommand(object sender, ExecutedRoutedEventArgs e)
    {
        foreach (var f in LoadedFiles)
        {
            f.Data?.Dispose();
        }
        
        LoadedFiles.Clear();
        SelectedFile = null;
        ClearDynamicEditor();
    }
    
    private void CanExecuteWhenFileSelected(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = LoadedFileList.SelectedItem is LoadedFile;
    }
    
    private void CanExecuteWhenFilesLoaded(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = LoadedFiles.Any();
    }

    private void HandleFileSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count < 1) return;
        if (e.AddedItems[0] is LoadedFile first)
        {
            SelectedFile = first;
            ClearDynamicEditor();

            if(SelectedFile.IsError)
            {
                var stackTrace = new StackTraceViewer();
                stackTrace.Tag = _dynamicPanelTag;
                stackTrace.ExceptionData = SelectedFile.Error!;
                stackTrace.TitleText = "Failed to load the file!";
                ContentPanel.Children.Add(stackTrace);
            }
            else
            {
                var editors = PriorityResolver.GetEditorsForFile(SelectedFile.Data!);
                var ed = editors[0];

                ed.File = SelectedFile;
                if(ed is FrameworkElement ui)
                {
                    ui.Tag = _dynamicPanelTag;
                    ContentPanel.Children.Add(ui);
                }
            }
        }
    }
    
    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private void ClearDynamicEditor()
    {
        for (var i = 0; i < ContentPanel.Children.Count; i++)
        {
                
            if (ContentPanel.Children[i] is not FrameworkElement el) continue;
            if (el.Tag is string tag && tag == _dynamicPanelTag)
            {
                ContentPanel.Children.RemoveAt(i--);
            }
        }
    }
}
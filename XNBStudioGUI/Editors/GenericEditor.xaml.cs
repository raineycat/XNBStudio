using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using PortableXNB;
using Spooksoft.HexEditor.Infrastructure;

namespace XNBStudioGUI.Editors;

public partial class GenericEditor : IXnbEditor, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string EditorName => "Generic editor";

    private XnbFile? _file;
    public XnbFile? File
    {
        get => _file;
        set
        {
            _file = value;
            OnPropertyChanged();
            UpdateHexEditor();
        }
    }

    private Stream? _stream;
    
    public GenericEditor()
    {
        InitializeComponent();
        DataContext = this;
    }
    
    public int GetPriorityForType(string type)
    {
        // use this for anything not already handled
        // higher priority means pick me first
        // negative means ignore me
        return 0;
    }
    
    private void UpdateHexEditor()
    {
        if(File == null) return;
        _stream = File.Contents!;
        //FileContentHexEditor.Document = new HexByteContainer(_stream);
    }
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
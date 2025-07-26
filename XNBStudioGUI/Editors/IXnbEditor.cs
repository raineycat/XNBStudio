using PortableXNB;

namespace XNBStudioGUI.Editors;

public interface IXnbEditor
{
    public string EditorName { get; }
    public LoadedFile? File { get; set; }
    public int GetPriorityForType(string type);
}
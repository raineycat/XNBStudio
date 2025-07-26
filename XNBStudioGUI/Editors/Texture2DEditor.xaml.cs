using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PortableXNB.Formats;
using Image = SixLabors.ImageSharp.Image;

namespace XNBStudioGUI.Editors;

public partial class Texture2DEditor : IXnbEditor, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
   
    public string EditorName => "Texture editor";
    
    private LoadedFile? _file;
    public LoadedFile? File
    {
        get => _file;
        set
        {
            _file = value;
            OnPropertyChanged();
            UpdateContent();
        }
    }

    private Texture2D? _texture;

    public Texture2D? Texture
    {
        get => _texture;
        set
        {
            _texture = value;
            OnPropertyChanged();
        }
    }

    private Image? _previewImage;

    public Image? PreviewImage
    {
        get => _previewImage;
        set
        {
            _previewImage = value;
            OnPropertyChanged();
        }
    }
    
    public Texture2DEditor()
    {
        InitializeComponent();
        DataContext = this;
    }
    
    public int GetPriorityForType(string type)
    {
        if(type.StartsWith("Microsoft.Xna.Framework.Content.Texture2DReader"))
        {
            return 50;
        }

        return -1;
    }
    
    private void UpdateContent()
    {
        PreviewImage = null;
        TextureDataDisplay.Visibility = Visibility.Visible;
        ImageError.Visibility = Visibility.Hidden;
        
        if(File == null) return;

        if (Texture != null)
        {
            foreach (var mip in Texture.Mips)
            {
                // mip.Dispose();
            }

            Texture = null;
        }

        try
        {
            Texture = File.Assets[0] as Texture2D;
            PreviewImage = Texture!.Mips[0];
        }
        catch (Exception e)
        {
            ImageError.ExceptionData = e;
            ImageError.Visibility = Visibility.Visible;
            TextureDataDisplay.Visibility = Visibility.Hidden;
        }
    }
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void HandlePreviewImageSelected(object sender, SelectionChangedEventArgs e)
    {
        if(e.AddedItems.Count < 1) return;
        if (e.AddedItems[0] is Image img)
        {
            PreviewImage = img;
        }
    }
}
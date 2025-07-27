using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PortableXNB.Formats;
using SixLabors.ImageSharp;
using Color = System.Drawing.Color;
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

    private float _backgroundOpacity = 100f;
    public float BackgroundOpacity
    {
        get => _backgroundOpacity;
        set
        {
            _backgroundOpacity = value;
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

    private void CanExecuteIfPreviewLoaded(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = PreviewImage != null;
    }

    private void ExecutePrintCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if(PreviewImage == null) return;
        
        var win = Window.GetWindow(this);
        var path = win?.SaveFile("PNG files", "*.png", (File?.FileName ?? "texture") + ".png");
        if(path == null) return;

        PreviewImage.SaveAsPng(path);
        win?.ShowMessageBox("Exported texture!");
    }
}
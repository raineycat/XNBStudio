using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NAudio.Wave;
using NAudio.WaveFormRenderer;
using PortableXNB.Formats;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace XNBStudioGUI.Editors;

public partial class SoundEffectEditor : IXnbEditor, INotifyPropertyChanged
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
            if (_file is { Assets.Count: > 0 } && _file.Assets[0] is SoundEffect sfx) Sound = sfx;
            OnPropertyChanged();
            ResetAudioOut();
        }
    }

    private SoundEffect? _sound;

    public SoundEffect? Sound
    {
        get => _sound;
        set
        {
            _sound = value;
            OnPropertyChanged();
            ResetAudioOut();
            
            if (Sound != null)
            {
                _audioOut.Init(_sound);
            }
        }
    }

    private Image? _waveform;

    public Image? Waveform
    {
        get => _waveform;
        set
        {
            _waveform = value;
            OnPropertyChanged();
        }
    }

    private WaveOutEvent _audioOut;
    private WaveFormRenderer _renderer;
    private ImageBindingConverter _imageConv;

    public SoundEffectEditor()
    {
        InitializeComponent();
        DataContext = this;

        _audioOut = new WaveOutEvent();
        _renderer = new WaveFormRenderer();
        _imageConv = new ImageBindingConverter();

        WaveFormDisplay.LayoutUpdated += (_, _) => GenerateWaveform();
        _audioOut.PlaybackStopped += (_, _) => Sound?.ResetPosition();
    }
    
    public int GetPriorityForType(string type)
    {
        if(type.StartsWith("Microsoft.Xna.Framework.Content.SoundEffectReader"))
        {
            return 50;
        }

        return -1;
    }
    
    private void ResetAudioOut()
    {
        _audioOut.Stop();
    }
    
    private void GenerateWaveform()
    {
        if (Sound == null)
        {
            Waveform?.Dispose();
            Waveform = null;
            return;
        }

        if (Waveform != null)
        {
            // don't bother re-rendering it (because it's slow as fuck)
            return;
        }

        var wave = new RawSourceWaveStream(Sound.SampleBuffer, 0, Sound.SampleBuffer.Length, Sound.WaveFormat);
        var greenPen = new Pen(Color.SeaGreen);
        var settings = new StandardWaveFormRendererSettings
        {
            Width = (int)WaveFormDisplay.ActualWidth,
            BackgroundColor = Color.Black,
            BottomPeakPen = greenPen,
            TopPeakPen = greenPen
        };

        if (settings.Width == 0)
        {
            Waveform?.Dispose();
            Waveform = null;
            return;
        }

        Task.Run(() =>
        {
            Waveform = _renderer.Render(wave, settings);
            wave.Dispose();
            greenPen.Dispose();
            
            Dispatcher.Invoke(() =>
            {
                WaveFormDisplay.Source =
                    _imageConv.Convert(Waveform, typeof(ImageSource), null, CultureInfo.CurrentCulture) as ImageSource;
            });
        });
    }
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CanExecuteIfSoundLoaded(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = Sound != null;
    }

    private void ExecutePlayCommand(object sender, ExecutedRoutedEventArgs e)
    {
        _audioOut.Play();
    }

    private void ExecuteStopCommand(object sender, ExecutedRoutedEventArgs e)
    {
        _audioOut.Stop();
    }

    private void ExecutePrintCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if(Sound == null) return;

        var win = Window.GetWindow(this);
        var path = win?.SaveFile("Wave files", "*.wav", (File?.FileName ?? "sound_effect") + ".wav");
        if(path == null) return;

        using var writer = new WaveFileWriter(path, Sound.WaveFormat);
        writer.Write(Sound.SampleBuffer);

        win?.ShowMessageBox("Exported sound file!");
    }
}
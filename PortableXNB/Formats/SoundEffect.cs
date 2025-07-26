using NAudio.Wave;

namespace PortableXNB.Formats;

public class SoundEffect : IWaveProvider
{
    public WaveFormat WaveFormat { get; set; }
    public int LoopStart { get; set; }
    public int LoopLength { get; set; }
    public TimeSpan Duration { get; set; }
    public byte[] SampleBuffer { get; set; }
    
    public int Read(byte[] buffer, int offset, int count)
    {
        Array.Copy(SampleBuffer, offset, buffer, 0, count);
        return count;
    }
    
}
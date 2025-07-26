using NAudio.Wave;

namespace PortableXNB.Formats;

public class SoundEffect : IWaveProvider
{
    public WaveFormat WaveFormat { get; set; }
    public int LoopStart { get; set; }
    public int LoopLength { get; set; }
    public TimeSpan Duration { get; set; }
    public byte[] SampleBuffer { get; set; }

    private int _bufferPos = 0;
    public int Read(byte[] buffer, int offset, int count)
    {
        Array.Copy(SampleBuffer, _bufferPos, buffer, offset, count);
        _bufferPos += count;
        return count;
    }

    public void ResetPosition() => _bufferPos = 0;
}
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AgentFx.RealtimeAudioConsole.Audio;

/// <summary>
/// Service for playing audio through speakers
/// </summary>
public class AudioPlaybackService : IDisposable
{
    private readonly int _sampleRate;
    private readonly int _channels;
    private WaveOutEvent? _waveOut;
    private BufferedWaveProvider? _bufferedWaveProvider;
    private bool _disposed;

    public AudioPlaybackService(int sampleRate = 24000, int channels = 1)
    {
        _sampleRate = sampleRate;
        _channels = channels;
    }

    public void Initialize()
    {
        if (_waveOut != null)
        {
            throw new InvalidOperationException("Audio playback already initialized");
        }

        _waveOut = new WaveOutEvent();
        _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(_sampleRate, 16, _channels))
        {
            BufferDuration = TimeSpan.FromSeconds(10),
            DiscardOnBufferOverflow = true
        };

        _waveOut.Init(_bufferedWaveProvider);
        _waveOut.Play();
    }

    public void EnqueueAudio(byte[] audioData)
    {
        if (_bufferedWaveProvider == null)
        {
            throw new InvalidOperationException("Audio playback not initialized. Call Initialize() first.");
        }

        _bufferedWaveProvider.AddSamples(audioData, 0, audioData.Length);
    }

    public void ClearBuffer()
    {
        _bufferedWaveProvider?.ClearBuffer();
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _waveOut?.Stop();
        _waveOut?.Dispose();
        _bufferedWaveProvider = null;
    }
}

using NAudio.Wave;
using System.Buffers;

namespace AgentFx.RealtimeAudioConsole.Audio;

/// <summary>
/// Service for capturing audio from the microphone and streaming it in PCM16 format
/// </summary>
public class AudioCaptureService : IDisposable
{
    private readonly int _sampleRate;
    private readonly int _channels;
    private readonly int _deviceIndex;
    private WaveInEvent? _waveIn;
    private bool _disposed;

    public AudioCaptureService(int sampleRate = 24000, int channels = 1, int deviceIndex = -1)
    {
        _sampleRate = sampleRate;
        _channels = channels;
        _deviceIndex = deviceIndex;
    }

    public event EventHandler<byte[]>? AudioDataAvailable;

    public void StartCapture()
    {
        if (_waveIn != null)
        {
            throw new InvalidOperationException("Audio capture already started");
        }

        _waveIn = new WaveInEvent
        {
            DeviceNumber = _deviceIndex,
            WaveFormat = new WaveFormat(_sampleRate, 16, _channels),
            BufferMilliseconds = 100
        };

        _waveIn.DataAvailable += OnDataAvailable;
        _waveIn.StartRecording();
    }

    public void StopCapture()
    {
        if (_waveIn != null)
        {
            _waveIn.StopRecording();
            _waveIn.DataAvailable -= OnDataAvailable;
            _waveIn.Dispose();
            _waveIn = null;
        }
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (e.BytesRecorded > 0)
        {
            // Copy the buffer since WaveInEventArgs reuses the buffer
            var audioData = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, audioData, 0, e.BytesRecorded);
            AudioDataAvailable?.Invoke(this, audioData);
        }
    }

    public static void ListAudioDevices()
    {
        Console.WriteLine("\n=== Available Audio Input Devices ===");
        for (int i = 0; i < WaveInEvent.DeviceCount; i++)
        {
            var capabilities = WaveInEvent.GetCapabilities(i);
            Console.WriteLine($"Device {i}: {capabilities.ProductName} ({capabilities.Channels} channels)");
        }
        Console.WriteLine("=====================================\n");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StopCapture();
    }
}

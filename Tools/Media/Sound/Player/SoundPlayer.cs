using System;
using System.IO;
using PaintPower.Tools.Math;
using PaintPower.Tools.Media;
using PaintPower.Tools.Media.Player;
using PaintPower.Tools.Media.Sound;
using PaintPower.Tools.Media.Sound.Player.Backends;

namespace PaintPower.Tools.Media.Sound.Player;

public class SoundPlayer : MediaPlayer, IDisposable
{
    private IAudioBackend backend;
    private bool isPaused = false;

    private float _volume = 1f;
    private bool _loop = false;

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = System.Math.Clamp(value, 0f, 1f);
            backend.SetVolume(_volume);
        }
    }

    public bool Loop
    {
        get => _loop;
        set
        {
            _loop = value;
            backend.SetLooping(_loop);
        }
    }

    public SoundPlayer()
    {
#if WINDOWS
        backend = new WindowsAudioBackend();
#elif MACOS
        backend = new MacOSAudioBackend();
#elif LINUX
        backend = new LinuxAudioBackend();
#else
        backend = new DummyAudioBackend();
#endif
    }

    public override void LoadMedia(Media media)
    {
        base.LoadMedia(media);

        if (media.FilePath == null)
            throw new InvalidOperationException("Sound must have a file path.");

        byte[] wav = File.ReadAllBytes(media.FilePath);

        int channels = BitConverter.ToInt16(wav, 22);
        int sampleRate = BitConverter.ToInt32(wav, 24);
        int bitsPerSample = BitConverter.ToInt16(wav, 34);

        int dataOffset = BitConverter.ToInt32(wav, 16) + 20;
        int dataSize = wav.Length - dataOffset;

        byte[] pcm = new byte[dataSize];
        Array.Copy(wav, dataOffset, pcm, 0, dataSize);

        backend.LoadPcm(pcm, channels, sampleRate, bitsPerSample);
        backend.SetVolume(_volume);
        backend.SetLooping(_loop);
    }

    public override void Play()
    {
        backend.Play();
        isPaused = false;
    }

    public override void Pause()
    {
        backend.Pause();
        isPaused = true;
    }

    public override void Resume()
    {
        if (!isPaused) return;
        backend.Resume();
        isPaused = false;
    }

    public override void Stop()
    {
        backend.Stop();
        isPaused = false;
    }

    public void Dispose()
    {
        backend.Dispose();
    }
}

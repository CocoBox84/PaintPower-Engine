using System;

namespace PaintPower.Tools.Media.Sound.Player.Backends;

public interface IAudioBackend : IDisposable
{
    void LoadPcm(byte[] pcmData, int channels, int sampleRate, int bitsPerSample);

    void Play();
    void Pause();
    void Resume();
    void Stop();

    void SetVolume(float volume); // 0.0 – 1.0
    void SetLooping(bool loop);
}

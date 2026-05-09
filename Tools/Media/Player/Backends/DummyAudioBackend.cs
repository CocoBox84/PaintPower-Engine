namespace PaintPower.Tools.Media.Sound.Player.Backends;

public class DummyAudioBackend : IAudioBackend
{
    public void LoadPcm(byte[] pcmData, int channels, int sampleRate, int bitsPerSample) { }
    public void Play() { }
    public void Pause() { }
    public void Resume() { }
    public void Stop() { }
    public void Dispose() { }

    public void SetVolume(float volume) { } // 0.0 – 1.0
    public void SetLooping(bool loop) { }
}

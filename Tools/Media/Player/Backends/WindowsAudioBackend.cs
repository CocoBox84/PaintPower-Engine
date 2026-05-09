using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace PaintPower.Tools.Media.Sound.Player.Backends;

public class WindowsAudioBackend : IAudioBackend
{
    private IntPtr waveOut = IntPtr.Zero;
    private WaveHeader header;
    private byte[]? data;
    private GCHandle dataHandle;

    private bool looping = false;
    private float volume = 1f;

    public void LoadPcm(byte[] pcmData, int channels, int sampleRate, int bitsPerSample)
    {
        data = pcmData;

        WaveFormat format = new WaveFormat
        {
            wFormatTag = 1,
            nChannels = (ushort)channels,
            nSamplesPerSec = (uint)sampleRate,
            wBitsPerSample = (ushort)bitsPerSample,
            nBlockAlign = (ushort)(channels * bitsPerSample / 8),
            nAvgBytesPerSec = (uint)(sampleRate * channels * bitsPerSample / 8),
            cbSize = 0
        };

        WinMM.waveOutOpen(out waveOut, -1, ref format, IntPtr.Zero, IntPtr.Zero, 0);

        dataHandle = GCHandle.Alloc(data, GCHandleType.Pinned);

        header = new WaveHeader
        {
            lpData = dataHandle.AddrOfPinnedObject(),
            dwBufferLength = (uint)data.Length
        };

        WinMM.waveOutPrepareHeader(waveOut, ref header, Marshal.SizeOf(header));
        ApplyVolume();
    }

    public void Play()
{
    if (waveOut == IntPtr.Zero || data == null) return;

    WinMM.waveOutWrite(waveOut, ref header, Marshal.SizeOf(header));

    if (looping)
    {
        // Poll for completion in a background thread
        Task.Run(() =>
        {
            while (true)
            {
                if ((header.dwFlags & 0x00000001) != 0) // WHDR_DONE
                {
                    WinMM.waveOutWrite(waveOut, ref header, Marshal.SizeOf(header));
                }
                Thread.Sleep(10);
            }
        });
    }
}

    public void Pause()
    {
        if (waveOut == IntPtr.Zero) return;
        WinMM.waveOutPause(waveOut);
    }

    public void Resume()
    {
        if (waveOut == IntPtr.Zero) return;
        WinMM.waveOutRestart(waveOut);
    }

    public void Stop()
    {
        if (waveOut == IntPtr.Zero) return;
        WinMM.waveOutReset(waveOut);
    }

    public void SetVolume(float vol)
    {
        volume = System.Math.Clamp(vol, 0f, 1f);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        int v = (int)(volume * 0xFFFF);
        uint packed = (uint)(v | (v << 16));
        WinMM.waveOutSetVolume(waveOut, packed);
    }

    public void SetLooping(bool loop)
    {
        looping = loop;
    }

    public void Dispose()
    {
        if (waveOut != IntPtr.Zero)
        {
            WinMM.waveOutReset(waveOut);
            WinMM.waveOutClose(waveOut);
            waveOut = IntPtr.Zero;
        }

        if (dataHandle.IsAllocated)
            dataHandle.Free();
    }
}

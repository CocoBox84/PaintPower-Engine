using System;
using System.Runtime.InteropServices;

namespace PaintPower.Tools.Media.Sound.Player;

internal static class WinMM
{
    [DllImport("winmm.dll")]
    public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, ref WaveFormat lpFormat, IntPtr dwCallback, IntPtr dwInstance, int dwFlags);

    [DllImport("winmm.dll")]
    public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHeader lpWaveOutHdr, int uSize);

    [DllImport("winmm.dll")]
    public static extern int waveOutWrite(IntPtr hWaveOut, ref WaveHeader lpWaveOutHdr, int uSize);

    [DllImport("winmm.dll")]
    public static extern int waveOutPause(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutRestart(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutReset(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutClose(IntPtr hWaveOut);

    [DllImport("winmm.dll")]
    public static extern int waveOutSetVolume(IntPtr hWaveOut, uint dwVolume);
}

[StructLayout(LayoutKind.Sequential)]
public struct WaveFormat
{
    public ushort wFormatTag;
    public ushort nChannels;
    public uint nSamplesPerSec;
    public uint nAvgBytesPerSec;
    public ushort nBlockAlign;
    public ushort wBitsPerSample;
    public ushort cbSize;
}

[StructLayout(LayoutKind.Sequential)]
public struct WaveHeader
{
    public IntPtr lpData;
    public uint dwBufferLength;
    public uint dwBytesRecorded;
    public IntPtr dwUser;
    public uint dwFlags;
    public uint dwLoops;
    public IntPtr lpNext;
    public IntPtr reserved;
}

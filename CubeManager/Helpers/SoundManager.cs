using NAudio.Wave;

namespace CubeManager.Helpers;

public class SoundManager
{
    private AudioFileReader audioFileReader;
    private IWavePlayer waveOutDevice;

    public void PlayAudio(string audioFilePath)
    {
        DisposeWave();

        waveOutDevice = new WaveOut();
        audioFileReader = new AudioFileReader(audioFilePath);

        audioFileReader.Volume = 0.3f;
        waveOutDevice.Init(audioFileReader);
        waveOutDevice.Play();
    }

    private void DisposeWave()
    {
        if (waveOutDevice != null)
        {
            waveOutDevice.Stop();
            waveOutDevice.Dispose();
            waveOutDevice = null;
        }

        if (audioFileReader != null)
        {
            audioFileReader.Dispose();
            audioFileReader = null;
        }
    }
}
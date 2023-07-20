using System.IO;
using System.Windows;
using NAudio.Wave;

namespace CubeManager.Helpers;

public class SoundManager
{
    private AudioFileReader audioFileReader;
    private IWavePlayer waveOutDevice;

    public void PlayAudio(string audioFilePath)
    {
        DisposeWave();

        string tempFilePath;

        // If audioFilePath is null, play default sound from resources
        if (audioFilePath == null)
        {
            Stream resourceStream = Application
                .GetResourceStream(new Uri("pack://application:,,,/Resources/notification1.wav")).Stream;

            // Save resource data to a temporary file
            tempFilePath = Path.GetTempFileName();
            using (var fileStream = File.OpenWrite(tempFilePath))
            {
                resourceStream.CopyTo(fileStream);
            }
        }
        else
        {
            tempFilePath = audioFilePath;
        }

        waveOutDevice = new WaveOut();
        audioFileReader = new AudioFileReader(tempFilePath);

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
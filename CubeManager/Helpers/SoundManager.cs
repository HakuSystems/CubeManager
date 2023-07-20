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

        if (ConfigManager.Instance.Config.Settings.EnableSound == false) return;

        var tempFilePath = GetSoundFilePath(audioFilePath);

        waveOutDevice = new WaveOut();
        audioFileReader = new AudioFileReader(tempFilePath);

        audioFileReader.Volume = 0.3f;
        waveOutDevice.Init(audioFileReader);
        waveOutDevice.Play();
    }

    private string GetSoundFilePath(string audioFilePath)
    {
        // If audioFilePath is not null start with pack://, get resource from pack URI
        if (audioFilePath != null && audioFilePath.StartsWith("pack://")) return GetResourceFromPackUri(audioFilePath);

        // If audioFilePath is a valid file, return it
        if (File.Exists(audioFilePath)) return audioFilePath;

        // If nothing is appropriate, throw an exception
        throw new Exception($"File not found: {audioFilePath}");
    }

    private string GetResourceFromPackUri(string packUri)
    {
        var uri = new Uri(packUri, UriKind.RelativeOrAbsolute);
        var streamResourceInfo = Application.GetResourceStream(uri);
        if (streamResourceInfo == null) throw new Exception($"Failed to find resource: {packUri}");

        var tempFilePath = Path.GetTempFileName();
        using (var stream = streamResourceInfo.Stream)
        using (var fileStream = File.Open(tempFilePath, FileMode.Create))
        {
            stream.CopyTo(fileStream);
        }

        return tempFilePath;
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
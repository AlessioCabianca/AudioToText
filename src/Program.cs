using System.Diagnostics;
using AudioToText.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AudioToText;

public abstract class Program
{
    
    static async Task Main(string[] args)
    {
        var host = BuildHost(args);
        
        Console.WriteLine("Inserisci il percorso del file video/audio (.mp4):");
        var videoPath = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(videoPath))
        {
            Console.WriteLine("Errore: il percorso del file video non può essere vuoto.");
            return;
        }

        Console.WriteLine(
            "(Opzionale) Inserisci l'orario di inizio (in secondi) [premi Invio per usare l'inizio del video]:");
        var startTimeInput = Console.ReadLine();
        var startTime = string.IsNullOrEmpty(startTimeInput) ? 0 : int.Parse(startTimeInput);

        Console.WriteLine(
            "(Opzionale) Inserisci l'orario di fine (in secondi) [premi Invio per usare la fine del video]:");
        var endTimeInput = Console.ReadLine();
        int? endTime = string.IsNullOrEmpty(endTimeInput) ? null : int.Parse(endTimeInput);

        Console.WriteLine("(Opzionale) Inserisci il percorso per salvare il file audio temporaneo (.wav):");
        var audioOutputPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(audioOutputPath))
        {
            var directory = Path.GetDirectoryName(videoPath);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(videoPath);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var newFileName = $"{fileNameWithoutExtension}_{timestamp}.wav";
            audioOutputPath = Path.Combine(directory, newFileName);
        }

        try
        {
            ExtractAudioSegment(videoPath, audioOutputPath, startTime, endTime);

            var transcribeAudioResponse = await GetAudioService(host)
                .TranscribeAudio(audioOutputPath);

            Console.WriteLine("Trascrizione completata:");
            Console.WriteLine(transcribeAudioResponse.Text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore: {ex.Message}");
        }
    }

    private static IHost BuildHost(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddUserSecrets<Program>();
            })
            .ConfigureServices((_, services) =>
            {
                services.AddTransient<IAudioService, AzureOpenAiAudioService>();
            }).Build();

    private static void ExtractAudioSegment(string videoPath, string audioOutputPath, int? startTime, int? endTime)
    {
        startTime ??= 0;
        endTime ??= 5;

        var duration = (endTime.Value - startTime.Value).ToString();

        var ffmpegCommand =
            $"-i \"{videoPath}\" -ss {startTime.Value} -t {duration} -vn -acodec pcm_s16le -ar 16000 -ac 1 \"{audioOutputPath}\"";

        var processInfo = new ProcessStartInfo
        {
            FileName = "/opt/homebrew/bin/ffmpeg",
            Arguments = ffmpegCommand,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processInfo;
        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            var error = process.StandardError.ReadToEnd();
            throw new InvalidProgramException($"Errore FFmpeg: {error}");
        }
    }

    private static IAudioService GetAudioService(IHost host) => host.Services.GetRequiredService<IAudioService>();
}
namespace AudioToText.Contracts;

public interface IAudioService
{
    Task<TranscribeAudioResponse> TranscribeAudio(string audioPath);
}
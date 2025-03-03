using Microsoft.Extensions.Configuration;

namespace AudioToText.Tests;

public class AzureOpenAiAudioServiceTests
{
    private readonly AzureOpenAiAudioService _audioService;
    private readonly string _testAudioPath = "test_audio.wav";

    public AzureOpenAiAudioServiceTests()
    {
        if (!File.Exists(_testAudioPath))
        {
            throw new FileNotFoundException("Test audio file not found", _testAudioPath);
        }
        
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AzureOpenAiAudioServiceTests>()
            .Build();

        _audioService = new AzureOpenAiAudioService(configuration);
    }

    [Fact]
    public async Task TranscribeAudio_ShouldReturnText()
    {
        var response = await _audioService.TranscribeAudio(_testAudioPath);

        Assert.NotNull(response.Text);
        Assert.NotEmpty(response.Text);
        Assert.Equal("Ti saluto, Socrate.", response.Text);
    }
}
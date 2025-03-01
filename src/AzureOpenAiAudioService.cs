using AudioToText.Contracts;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

namespace AudioToText;

public class AzureOpenAiAudioService(IConfiguration configuration) : IAudioService
{
    private readonly string _azureEndpoint = configuration["AzureOpenAi:Endpoint"]!;
    private readonly string _azureApiKey = configuration["AzureOpenAi:ApiKey"]!;
    private readonly string _azureModelName = configuration["AzureOpenAi:ModelName"]!;

    public async Task<TranscribeAudioResponse> TranscribeAudio(string audioPath)
    {
        var openAiClient = new AzureOpenAIClient(new Uri(_azureEndpoint), new AzureKeyCredential(_azureApiKey));
        var audioClient = openAiClient.GetAudioClient(_azureModelName);

        var result = await audioClient.TranscribeAudioAsync(audioPath);
        return new()
        {
            Text = result.Value.Text
        };
    }
}
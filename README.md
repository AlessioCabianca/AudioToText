# 🔊 AudioToText

AudioToText is a .NET 8 console application that extracts audio from video files and transcribes it using Azure's OpenAI Whisper model.

## 🚀 Features 

- Extracts audio from video files using FFmpeg.
- Transcribes audio to text using Azure OpenAI Whisper model.

## 📦 Prerequisites 

- .NET 8 SDK
- FFmpeg installed and accessible in your system's PATH
- Azure OpenAI resource with access to the Whisper model

## 🛠️ Setup 

1. **Clone the repository**:

   ```bash
   git clone https://github.com/AlessioCabianca/AudioToText.git
   cd AudioToText
   ```

2. **Initialize user secrets**:

   ```bash
   dotnet user-secrets init
   ```

3. **Set Azure credentials**:

   ```bash
   dotnet user-secrets set "AzureOpenAi:ApiKey" "your_azure_api_key"
   dotnet user-secrets set "AzureOpenAi:Endpoint" "your_azure_endpoint"
   dotnet user-secrets set "AzureOpenAi:ModelName" "whisper"
   ```

   Replace `"your_azure_api_key"` and `"your_azure_endpoint"` with your actual Azure OpenAI credentials.

4. **Install ffmpeg**:

   ```bash
   brew install ffmpeg
   ```

## 💻 Usage

1. **Build the application**:

   ```bash
   dotnet build
   ```

2. **Run the application**:

   ```bash
   dotnet run
   ```

3. **Follow the on-screen prompts** to provide the path to your video/audio file and optional start/end times for audio extraction.

## 📚 Acknowledgements

This project utilizes the following libraries and services:

- FFmpeg for audio extraction
- Azure OpenAI Whisper model for transcription

namespace TrustyTickets.Ai;

public sealed class LlmConfig
{
    public string Provider { get; init; } = string.Empty; // "ollama" or "openai"

    // Ollama
    public string OllamaUrl { get; init; } = "http://localhost:11434";
    public string OllamaModel { get; init; } = "llama3:latest";

    // OpenAI
    public string OpenAiApiKey { get; init; } = string.Empty;
    public string OpenAiModel { get; init; } = "gpt-4o-mini";
    public string OpenAiBaseUrl { get; init; } = "https://api.openai.com";

    public static LlmConfig? FromEnvironment()
    {
        // Demo hardcoded LLM settings (edit this file for your environment).
        // Default: Ollama local.
        return new LlmConfig
        {
            Provider = "ollama",
            OllamaUrl = "http://localhost:11434",
            // Matches an existing local Ollama model from your `ollama list`
            OllamaModel = "llama3:latest",
            OpenAiApiKey = string.Empty
        };


    // --- Uncomment this block to use OpenAI instead ---
    /*
    return new LlmConfig
    {
        Provider = "openai",
        // REQUIRED: put your OpenAI API key here for the demo
        OpenAiApiKey = "YOUR_OPENAI_API_KEY",
        // Optional overrides
        OpenAiModel = "gpt-4o-mini",
        OpenAiBaseUrl = "https://api.openai.com",
        // Ollama fields not used in openai mode
        OllamaUrl = "http://localhost:11434",
        OllamaModel = "llama3:latest"
    };
    */
    }
}


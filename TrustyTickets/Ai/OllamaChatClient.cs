using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TrustyTickets.Ai;

public sealed class OllamaChatClient : ILlmChatClient
{
    private readonly HttpClient _http;
    private readonly LlmConfig _cfg;

    public OllamaChatClient(HttpClient http, LlmConfig cfg)
    {
        _http = http;
        _cfg = cfg;
    }

    public async Task<string> ChatAsync(IReadOnlyList<ChatMessage> messages, CancellationToken ct)
    {
        var url = $"{_cfg.OllamaUrl.TrimEnd('/')}/api/chat";

        var payload = new
        {
            model = _cfg.OllamaModel,
            messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
            stream = false,
            options = new
            {
                // Keep the output stable for parsing in a demo.
                temperature = 0.4,
                // Ask Ollama to return structured output when supported.
                format = "json"
            }
        };

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        using var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var text = await res.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(text);

        if (doc.RootElement.TryGetProperty("message", out var msg) &&
            msg.TryGetProperty("content", out var content))
        {
            return content.GetString() ?? "";
        }

        // Fallback if response shape differs.
        return text;
    }
}


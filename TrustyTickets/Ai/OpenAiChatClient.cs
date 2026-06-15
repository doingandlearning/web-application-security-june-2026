using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TrustyTickets.Ai;

public sealed class OpenAiChatClient : ILlmChatClient
{
    private readonly HttpClient _http;
    private readonly LlmConfig _cfg;

    public OpenAiChatClient(HttpClient http, LlmConfig cfg)
    {
        _http = http;
        _cfg = cfg;
    }

    public async Task<string> ChatAsync(IReadOnlyList<ChatMessage> messages, CancellationToken ct)
    {
        var url = $"{_cfg.OpenAiBaseUrl.TrimEnd('/')}/v1/chat/completions";

        using var req = new HttpRequestMessage(HttpMethod.Post, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cfg.OpenAiApiKey);
        req.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                model = _cfg.OpenAiModel,
                messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
                // Keep the output stable for parsing in a demo.
                temperature = 0.4,
                max_tokens = 400
            }),
            Encoding.UTF8,
            "application/json"
        );

        using var res = await _http.SendAsync(req, ct);
        res.EnsureSuccessStatusCode();

        var text = await res.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(text);

        var choices = doc.RootElement.GetProperty("choices");
        if (choices.GetArrayLength() > 0)
        {
            var msg = choices[0].GetProperty("message");
            if (msg.TryGetProperty("content", out var content))
                return content.GetString() ?? "";
        }

        return text;
    }
}


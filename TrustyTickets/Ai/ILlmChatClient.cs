namespace TrustyTickets.Ai;

public interface ILlmChatClient
{
    Task<string> ChatAsync(IReadOnlyList<ChatMessage> messages, CancellationToken ct);
}


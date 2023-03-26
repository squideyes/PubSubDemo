namespace SharedModels;

public class WebHookError : IWebHook
{
    public required Guid ContractId { get; init; }
    public required string? Message { get; init; }
    public required string? ErrorCode { get; init; }

    public MessageKind MessageKind => MessageKind.WebHookError;
}

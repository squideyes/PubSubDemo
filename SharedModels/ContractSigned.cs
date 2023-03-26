namespace SharedModels;

public class ContractSigned : IWebHook
{
    public required Guid ContractId { get; init; }
    public required ContractKind ContractKind { get; init; }

    public MessageKind MessageKind => MessageKind.ContractSigned;
}

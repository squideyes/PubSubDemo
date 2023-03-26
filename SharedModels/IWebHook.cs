namespace SharedModels
{
    public interface IWebHook
    {
        Guid ContractId { get; }

        MessageKind MessageKind { get; }
    }
}
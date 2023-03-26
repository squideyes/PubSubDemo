using Microsoft.Azure.Functions.Worker;
using SharedModels;
using System.Text.Json;

namespace PubSubDemo;

internal class EventHeader
{
    public EventHeader(FunctionContext context)
    {
        var data = context.BindingContext.BindingData;

        string GetString(string key) => data![key]!.ToString()!;

        var keyValues = GetKeyValues(GetString("UserProperties"));

        CorrelationId = Guid.Parse(GetString("CorrelationId"));
        MessageId = Guid.Parse(GetString("MessageId"));
        Subject = GetString("Label");
        ExpiresAt = DateTime.Parse(GetString("ExpiresAtUtc").Trim('"'));
        ContractId = Guid.Parse(keyValues["ContractId"]);
        MessageKind = Enum.Parse<MessageKind>(keyValues["MessageKind"]);
    }

    public Guid CorrelationId { get; }
    public Guid MessageId { get; }
    public string Subject { get; }
    public DateTime ExpiresAt { get; }
    public Guid ContractId { get; }
    public MessageKind MessageKind { get;}

    private static Dictionary<string, string> GetKeyValues(string json)
    {
        var keyValues = new Dictionary<string, string>();

        var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        foreach (JsonProperty property in root.EnumerateObject())
        {
            if (property.Value.ValueKind != JsonValueKind.String)
                continue;

            keyValues.Add(property.Name, property.Value.GetString()!);
        }

        return keyValues;
    }
}

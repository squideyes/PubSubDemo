using Azure.Messaging.ServiceBus;
using SharedModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MessageSender;

internal static class MessageHelper<T> 
    where T : IWebHook
{
    public static ServiceBusMessage GetMessage(T data)
    {
        var options = new JsonSerializerOptions();

        options.Converters.Add(new JsonStringEnumConverter());

        var json = JsonSerializer.Serialize(data, options);

        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json",
            CorrelationId = data.ContractId.ToString(),
            MessageId = Guid.NewGuid().ToString(),
            Subject = "Test",
            TimeToLive = TimeSpan.FromDays(5)
        };

        message.ApplicationProperties
            .Add("ContractId", data.ContractId);

        message.ApplicationProperties.Add(
            "MessageKind", data.MessageKind.ToString());

        return message;
    }
}

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SharedModels;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Text.Json.JsonSerializer;

namespace PubSubDemo;

public class WebHookSink
{
    private readonly ILogger logger;
    private readonly JsonSerializerOptions options;

    public WebHookSink(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<WebHookSink>();

        options = new JsonSerializerOptions();

        options.Converters.Add(new JsonStringEnumConverter());
    }

    [Function("WebHookReceived")]
    public async Task WebHookReceived(FunctionContext context,
        [ServiceBusTrigger("esignatures", "WebHookReceived",
            Connection ="AzureWebJobsServiceBus")] string json)
    {
        var header = new EventHeader(context);

        LogMessageReceived(header);

        switch(header.MessageKind)
        {
            case MessageKind.ContractSigned:
                await HandleContractSigned(header, json!);
                break;
            case MessageKind.WebHookError:
                await HandleWebHookError(header, json);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    private void LogMessageReceived(EventHeader header)
    {
        var sb = new StringBuilder();

        sb.Append('\"');
        sb.Append(header.MessageKind);
        sb.Append("\" message received (ContractId: ");
        sb.Append(header.ContractId);
        sb.Append(", Subject: ");
        sb.Append(header.Subject);
        sb.Append(')');

        logger.LogInformation(sb.ToString());
    }

    private async Task HandleContractSigned(
        EventHeader header, string json)
    {
        await Task.CompletedTask;

        var data = Deserialize<ContractSigned>(json, options);

        logger.LogInformation($"The \"{header.MessageKind}\" message was processed");
    }

    private async Task HandleWebHookError(
        EventHeader header, string json)
    {
        await Task.CompletedTask;

        var data = Deserialize<WebHookError>(json, options);

        logger.LogInformation($"The \"{header.MessageKind}\" message was processed");
    }
}

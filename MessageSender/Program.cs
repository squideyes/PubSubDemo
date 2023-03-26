using Azure.Messaging.ServiceBus;
using MessageSender;
using Microsoft.Extensions.Configuration;
using SharedModels;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var connString = config["ServiceBusConnString"];

var cts = new CancellationTokenSource();

ServiceBusClient client;
ServiceBusSender sender;

client = new ServiceBusClient(connString);

sender = client.CreateSender("esignatures");

var contractId = Guid.NewGuid();

var contractSigned = new ContractSigned()
{
    ContractId = contractId,
    ContractKind = ContractKind.Partner
};

var webHookError = new WebHookError()
{
    ContractId = contractId,
    ErrorCode = "sms-delivery-failed",
    Message = "Not authorized"
};

async Task SendMessageAsync<T>(T message)
    where T : IWebHook
{
    await sender.SendMessageAsync(
        MessageHelper<T>.GetMessage(message), cts!.Token);

    Console.WriteLine($"A \"{message.MessageKind}\" message was published.");
}

try
{
    await SendMessageAsync(contractSigned);

    await Task.Delay(1000);

    await SendMessageAsync(webHookError);
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.Write("Press any key to terminate...");

Console.ReadKey(true);
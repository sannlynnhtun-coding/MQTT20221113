// See https://aka.ms/new-console-template for more information
using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Server;
using System.Text.Json;

Console.WriteLine("Hello, World!");

var mqttFactory = new MqttFactory();
var client = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    //.WithClientId(Guid.NewGuid().ToString())
    .WithTcpServer("localhost", 1883)
    .WithCleanSession()
.Build();

client.ApplicationMessageReceivedAsync += e =>
{
    Console.WriteLine("Received application message.");
    e.DumpToConsole();

    return Task.CompletedTask;
};

await client.ConnectAsync(options);

Console.ReadLine();

var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("Group");
                    })
                .Build();

await client.SubscribeAsync(subscribeOptions, CancellationToken.None);

await client.DisconnectAsync();

internal static class ObjectExtensions
{
    public static TObject DumpToConsole<TObject>(this TObject @object)
    {
        var output = "NULL";
        if (@object != null)
        {
            output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
        return @object;
    }
}
// See https://aka.ms/new-console-template for more information
using MQTTnet;
using MQTTnet.Client;

Console.WriteLine("Hello, World!");

var mqttFactory = new MqttFactory();
var client = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    //.WithClientId(Guid.NewGuid().ToString())
    .WithTcpServer("localhost", 1883)
    .WithCleanSession()
    .Build();
await client.ConnectAsync(options);

Console.WriteLine("Please press a key to publish the message");
Console.ReadLine();

await PublishMessageAsync(client);

await client.DisconnectAsync();

async Task PublishMessageAsync(IMqttClient client)
{
    string messagePayloadId = "Hello";
    var message = new MqttApplicationMessageBuilder()
    .WithTopic("Group")
    .WithPayload(messagePayloadId)
    .Build();
    if (client.IsConnected)
    {
        await client.PublishAsync(message);
    }
}
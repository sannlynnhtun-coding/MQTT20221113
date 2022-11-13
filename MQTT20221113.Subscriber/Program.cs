// See https://aka.ms/new-console-template for more information
using MQTTnet.Client;
using MQTTnet;
using MQTTnet.Server;
using System.Text.Json;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

var mqttFactory = new MqttFactory();
var client = mqttFactory.CreateMqttClient();
var options = new MqttClientOptionsBuilder()
    .WithClientId(Guid.NewGuid().ToString())
    .WithTcpServer("test.mosquitto.org", 1883)
    .WithCleanSession()
.Build();

client.ApplicationMessageReceivedAsync += e =>
{
    Console.WriteLine("Received application message.");
    e.DumpToConsole();

    return Task.CompletedTask;
};

await client.ConnectAsync(options);

var subscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("Group");
                    })
                .Build();

await client.SubscribeAsync(subscribeOptions, CancellationToken.None);

Console.ReadLine();

Console.ReadKey();

await client.DisconnectAsync();

internal static class ObjectExtensions
{
    public static TObject DumpToConsole<TObject>(this TObject @object)
    {
        var output = "NULL";
        if (@object != null)
        {
            output = JsonConvert.SerializeObject(@object, Formatting.Indented);
        }

        Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");

        var response = JObject.FromObject(@object);
        if (response.ContainsKey("ApplicationMessage"))
        {
            var context = @object as MqttApplicationMessageReceivedEventArgs;

            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            Console.WriteLine(
                " TimeStamp: {0} -- Message: ClientId = {1}, Topic = {2}, Payload = {3}, QoS = {4}, Retain-Flag = {5}",

                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }
        return @object;
    }
}
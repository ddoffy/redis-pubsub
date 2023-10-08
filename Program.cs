


using StackExchange.Redis;

IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

const string channel = "test-channel";
var          token   = new CancellationTokenSource();

Subscribe(channel, token.Token);

for (var i = 0; i < 10; i++)
{
   await Publish(channel, $"Message {i}");
}

await Task.Delay(10_000);
Console.WriteLine("Cancelling...");
token.Cancel();
await Task.Delay(2000);
Console.WriteLine("Exiting...");
return;


async Task Publish(string channel, string msg)
{
   var subscriber = redis.GetSubscriber();
   await subscriber.PublishAsync(channel, msg);
   Console.WriteLine($"Sent: {msg} to {channel}");
   await Task.Delay(500);
}

async Task Subscribe(string channel, CancellationToken token)
{
   Console.WriteLine($"Subscribing to {channel}");
   var subscriber = redis.GetSubscriber();
   
   var channelQueueMsg = await subscriber.SubscribeAsync(channel);
   while (!token.IsCancellationRequested)
   {
      var msg = await channelQueueMsg.ReadAsync(token);
      Console.WriteLine($"Receieved: {msg.Message} from {msg.Channel}");
      await Task.Delay(100, token);
   }
   Console.WriteLine("Unsubscribing...");
}

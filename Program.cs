


using StackExchange.Redis;

IConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

const string channel = "test-ch";
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


async Task Publish(string ch, string msg)
{
   var subscriber = redis.GetSubscriber();
   await subscriber.PublishAsync(ch, msg);
   Console.WriteLine($"Sent: {msg} to {ch}");
   await Task.Delay(500);
}

async Task Subscribe(string ch, CancellationToken ct)
{
   Console.WriteLine($"Subscribing to {ch}");
   var subscriber = redis.GetSubscriber();
   
   var channelQueueMsg = await subscriber.SubscribeAsync(ch);
   while (!ct.IsCancellationRequested)
   {
      var msg = await channelQueueMsg.ReadAsync(ct);
      Console.WriteLine($"Received: {msg.Message} from {msg.Channel}");
      await Task.Delay(100, ct);
   }
   Console.WriteLine("Unsubscribing...");
}

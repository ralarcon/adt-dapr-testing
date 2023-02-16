using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {app.UseDeveloperExceptionPage();}

//Register Dapr pub/sub subscriptions
app.MapGet("/dapr/subscribe", () => {
    var sub = new DaprSubscription(PubsubName: "eventhubs-pubsub", Topic: "dt-output", Route: "dt-output", Metadata: new Dictionary<string, string>() { { "rawPayload", "false" }, {"requireAllProperties", "true"} });
    Console.WriteLine("Dapr pub/sub is subscribed to: " + sub);
    return Results.Json(new DaprSubscription[]{sub});
});

// Dapr subscription in /dapr/subscribe sets up this route
app.MapPost("/dt-output", (dynamic requestData) => {
    Console.WriteLine("Subscriber received : " + requestData.ToString());
    return Results.Ok(requestData);
});

await app.RunAsync();

public record DaprSubscription(
  [property: JsonPropertyName("pubsubname")] string PubsubName, 
  [property: JsonPropertyName("topic")] string Topic, 
  [property: JsonPropertyName("route")] string Route,
  [property: JsonPropertyName("metadata")] Dictionary<string, string> Metadata = null);
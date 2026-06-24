using System.ComponentModel;
using Microsoft.Extensions.AI;
using OllamaSharp;


var weatherFunction = AIFunctionFactory.Create(GetWeather);
var approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(weatherFunction);


var agent = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.1:latest").AsAIAgent(
    instructions: "You weather information specialist.",
    tools: [approvalRequiredWeatherFunction]);

var session = await agent.CreateSessionAsync();
var response = await agent.RunAsync("What's the weather like in New York?", session);

var functionApprovalRequests = response.Messages
    .SelectMany(x => x.Contents)
    .OfType<ToolApprovalRequestContent>()
    .ToList();

    var requestContent = functionApprovalRequests.First();
Console.WriteLine($"We require approval to execute '{requestContent.ToolCall.CallId}'");

var input = Console.ReadLine()?.ToLower();
var isApproved = input == "s" || input == "sim";


if (isApproved)
{
    Console.WriteLine("Executando ferramenta...");
    var approvalMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(true)]);
    var finalResponse = await agent.RunAsync(approvalMessage, session);
    Console.WriteLine($"\n[ASSISTENTE]: {finalResponse}");
}
else
{
    Console.WriteLine("Execução negada pelo usuário.");
    // Opcional: Avisar ao modelo que a permissão foi negada
    var deniedMessage = new ChatMessage(ChatRole.User, [requestContent.CreateResponse(false)]);
    await agent.RunAsync(deniedMessage, session);
}

[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
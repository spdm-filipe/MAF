using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "qwen2.5:latest");

var agent = ollama
    .AsAIAgent()
    .AsBuilder()
    .Use(inner => new LoggingAgentMiddleware(inner))
    .Build();


    var response = await agent.RunAsync("O que são records no C#? me responda em até 80 caracteres.");
Console.WriteLine($"\nResposta Final: {response.Messages.Last().Text}");

public class LoggingAgentMiddleware(AIAgent innerAgent) : DelegatingAIAgent(innerAgent)
{
    private readonly AIAgent _inner = innerAgent; // Guardamos a referência direta

    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var lastMsg = messages.LastOrDefault()?.Text;
        Console.WriteLine($"\n[LOG INTERCEPTOR] Pergunta: {lastMsg}");

        var response = await _inner.RunAsync(messages, session, options, cancellationToken);

        Console.WriteLine($"[LOG INTERCEPTOR] Resposta recebida do Gemma 4.");
        return response;
    }
}
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.1:latest");

var agent = ollama
    .AsAIAgent()
    .AsBuilder()
    .Use(inner => new WordLimitGuardMiddleware(inner, maxWords: 40))
    .Build();

Console.WriteLine("Enviando pergunta ao agente...\n");

var response = await agent.RunAsync("Explique o que é programação orientada a objetos.");

Console.WriteLine($"\nResposta final:\n{response.Messages.Last().Text}");

public class WordLimitGuardMiddleware(AIAgent innerAgent, int maxWords = 50)
    : DelegatingAIAgent(innerAgent)
{
    // Guardamos a referência direta ao agente interno para chamadas de reenvio
    private readonly AIAgent _inner = innerAgent;
    private readonly int _maxWords = maxWords;

    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Executa o agente normalmente e obtém a resposta inicial
        var response = await _inner.RunAsync(messages, session, options, cancellationToken);

        var text = response.Messages.LastOrDefault()?.Text ?? string.Empty;
        var wordCount = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

        Console.WriteLine($"[GUARD] Resposta com {wordCount} palavras (limite: {_maxWords}).");

        // Se a resposta está dentro do limite, retorna diretamente
        if (wordCount <= _maxWords)
            return response;

        // Caso contrário, monta uma nova mensagem pedindo uma versão mais curta
        Console.WriteLine("[GUARD] Limite ultrapassado. Solicitando resposta mais curta...");

        var originalQuestion = messages.LastOrDefault()?.Text ?? string.Empty;
        var retryMessages = new List<ChatMessage>
        {
            new(ChatRole.User, $"{originalQuestion} Responda em no máximo {_maxWords} palavras.")
        };

        // Reenvia ao agente interno com a instrução adicional
        return await _inner.RunAsync(retryMessages, session, options, cancellationToken);
    }
}



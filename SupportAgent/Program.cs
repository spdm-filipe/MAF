using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using SupportAgent;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;



var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
    
var openAiApiKey = config["OpenAi:ApiKey"];


// Cria o agente com o provedor de histórico customizado.
AIAgent agent = new OpenAIClient(openAiApiKey)
    .GetChatClient("gpt-4o-mini")
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "SupportAgent",
        ChatOptions = new()
        {
            Instructions = "Você é um assistente de suporte técnico especializado em erros de compilação em C#. " +
                           "Responda de forma objetiva e sempre em português."
        },
        ChatHistoryProvider = new SupportChatHistoryProvider()
    });

// Cria a sessão — o provedor usa o AgentSession para isolar o estado desta conversa.
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine("=== Turno 1 ===");
Console.WriteLine(await agent.RunAsync(
    "Estou recebendo o erro CS0246: 'The type or namespace name X could not be found'. O que isso significa?",
    session));

Console.WriteLine("\n=== Turno 2 ===");
Console.WriteLine(await agent.RunAsync(
    "E quais são as causas mais comuns para esse erro aparecer?",
    session));

Console.WriteLine("\n=== Turno 3 ===");
Console.WriteLine(await agent.RunAsync(
    "Qual foi o erro que mencionei no início desta conversa?",
    session));

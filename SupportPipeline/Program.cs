using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

// Configuração centralizada
const string classifierPath = "Agents/classifier.md";
const string resolverPath = "Agents/resolver.md";
const string ollamaUrl = "http://localhost:11434";
const string model = "qwen2.5:latest";

// Problema de suporte que será triado e resolvido
const string userProblem =
    "Meu computador não consegue acessar nenhum site desde hoje de manhã, " +
    "mas a rede local está funcionando e consigo pingar o gateway.";

var httpClient = new HttpClient
{
    BaseAddress = new Uri(ollamaUrl),
    Timeout = TimeSpan.FromMinutes(3)
};

var ollama = new OllamaApiClient(httpClient);
var classifierInstructions = File.ReadAllText(classifierPath);
var resolverInstructions = File.ReadAllText(resolverPath);

var classifierAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "ClassifierAgent",
    Description = "Agente responsável por classificar e priorizar tickets de suporte",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.3f, // Temperatura baixa para respostas mais determinísticas
        Instructions = classifierInstructions
    }
});

var resolverAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "ResolverAgent",
    Description = "Agente responsável por propor soluções com base na triagem realizada",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.6f, // Temperatura maior para respostas mais elaboradas
        Instructions = resolverInstructions
    }
});

// A sessão é criada pelo agente classificador
// Isso garante que o histórico seja gerenciado pelo mesmo provedor
var session = await classifierAgent.CreateSessionAsync();

Console.WriteLine("=== PROBLEMA RELATADO ===");
Console.WriteLine(userProblem);
Console.WriteLine();

// Passo 1: o classificador analisa o problema e registra o contexto na sessão
Console.WriteLine("=== TRIAGEM (ClassifierAgent) ===");
var classification = await classifierAgent.RunAsync(userProblem, session);
Console.WriteLine(classification);
Console.WriteLine();

// Passo 2: o resolver acessa a mesma sessão e propõe soluções com base na triagem
Console.WriteLine("=== SOLUÇÃO (ResolverAgent) ===");
var solution = await resolverAgent.RunAsync(
    "Com base na triagem acima, proponha as soluções para o problema.",
    session);
Console.WriteLine(solution);
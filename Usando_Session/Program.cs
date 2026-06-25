
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

const string agentMarkdownPath = "Agents/code_reviewer.md";
const string codePath = "Agents/code.txt";
const string ollamaUrl = "http://localhost:11434";
const string model = "qwen2.5-coder:7b";

var handler = new HttpClientHandler();
var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri(ollamaUrl),
    Timeout = TimeSpan.FromMinutes(2)
};

var ollama = new OllamaApiClient(httpClient);
var instructions = File.ReadAllText(agentMarkdownPath);
var code = File.ReadAllText(codePath);

var codeReviewerAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "CodeReviewerAgent",
    Description = "Agente especialista em revisão de código C# e .NET",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.3f,
        Instructions = instructions
    }
});

var codeReviewerHelperAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "CodeReviewerHelperAgent",
    Description = "Agente especialista auxiliar os revisores de código",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.7f,
        Instructions = "Sempre responda com o nome do revisor"
    }
});

var session = await codeReviewerAgent.CreateSessionAsync();

var prompt =
    $"Olá, eu sou o revisor André Baltieri, revise o seguinte código C# e forneça feedback construtivo, sugestões de melhoria e identifique possíveis bugs ou problemas de desempenho:\n\n{code}";
var review = await codeReviewerAgent.RunAsync(prompt, session);
Console.WriteLine(review);

var response = await codeReviewerHelperAgent.RunAsync("Qual é o nome do revisor?", session);
Console.WriteLine(response);
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

// Caminhos dos arquivos de entrada
const string systemPromptPath = "Agents/schema_reviewer.md";
const string schemaPath = "Agents/schema.sql";

// Endereço local do Ollama e nome do modelo a usar
const string ollamaUrl = "http://localhost:11434";
const string model = "llama3.1:latest"; // troque pelo modelo que você tem baixado

// Configura o cliente HTTP com timeout estendido para modelos locais
var httpClient = new HttpClient(new HttpClientHandler())
{
    BaseAddress = new Uri(ollamaUrl),
    Timeout = TimeSpan.FromMinutes(3)
};

// Lê os arquivos de entrada do disco
var instructions = File.ReadAllText(systemPromptPath);
var schema = File.ReadAllText(schemaPath);

// Cria o cliente Ollama e configura o agente MAF
var ollama = new OllamaApiClient(httpClient);

var agent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "SchemaReviewerAgent",
    Description = "Agente especialista em revisão de schemas de banco de dados relacionais",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.3f, // temperatura baixa para saída mais determinística e estruturada
        Instructions = instructions
    }
});

// Executa o agente e desserializa a saída no record SchemaReviewResult
Console.WriteLine("Analisando schema...\n");
var result = await agent.RunAsync<SchemaReviewResult>(schema);
var review = result.Result;

// Exibe o relatório completo no terminal
Console.WriteLine($"Score geral:  {review.OverallScore}/100");
Console.WriteLine($"Conceito:     {review.Grade}");
Console.WriteLine($"\nResumo:\n{review.Summary}");

Console.WriteLine("\nProblemas de normalização:");
foreach (var issue in review.NormalizationIssues)
    Console.WriteLine($"  - {issue}");

Console.WriteLine("\nÍndices recomendados:");
foreach (var index in review.RecommendedIndexes)
    Console.WriteLine($"  - {index}");

Console.WriteLine("\nSugestões estruturais:");
foreach (var suggestion in review.StructuralSuggestions)
    Console.WriteLine($"  - {suggestion}");

Console.WriteLine("\nPróximos passos:");
foreach (var step in review.NextSteps)
    Console.WriteLine($"  - {step}");

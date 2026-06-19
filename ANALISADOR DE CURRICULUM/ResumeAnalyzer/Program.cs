using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

// Caminhos dos arquivos de configuração do agente
const string agentMarkdownPath = "Agents/resume_analyzer.md";
const string resumePath = "Agents/resume.txt";

// Configurações do servidor Ollama e do modelo local
const string ollamaUrl = "http://localhost:11434";
const string model = "qwen2.5-coder:7b";

// HttpClient com timeout estendido para acomodar a inferência local
var handler = new HttpClientHandler();
var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri(ollamaUrl),
    Timeout = TimeSpan.FromMinutes(5)
};

// OllamaApiClient implementa IChatClient, tornando-o compatível com o MAF
var ollama = new OllamaApiClient(httpClient);

// Leitura dos arquivos em tempo de execução
var instructions = File.ReadAllText(agentMarkdownPath);
var resume = File.ReadAllText(resumePath);

// Criação do agente MAF usando o cliente Ollama como provedor
var resumeAnalyzerAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "ResumeAnalyzerAgent",
    Description = "Agente especialista em análise de currículos de desenvolvedores",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.3f, // Temperatura baixa para respostas mais estruturadas e consistentes
        Instructions = instructions
    }
});

// Execução do agente com o currículo como entrada
Console.WriteLine("Analisando currículo com IA local...\n");

var result = await resumeAnalyzerAgent.RunAsync(
    $"Analise o currículo abaixo e retorne o feedback no formato JSON definido nas suas instruções.\n\n{resume}");

Console.WriteLine(result);

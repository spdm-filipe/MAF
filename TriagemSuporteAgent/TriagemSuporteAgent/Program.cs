using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

// Caminhos para os arquivos de configuração do agente
const string agentYamlPath = "Agents/triage_agent.yaml";
const string ollamaUrl = "http://localhost:11434";
const string model = "llama3.1:latest";

// Configura o HttpClient com timeout generoso para modelos locais
var handler = new HttpClientHandler();
var httpClient = new HttpClient(handler)
{
    BaseAddress = new Uri(ollamaUrl),
    Timeout = TimeSpan.FromMinutes(3)
};

// Instancia o cliente Ollama que será usado como backend do agente
var ollama = new OllamaApiClient(httpClient);


// Carrega as instruções do agente a partir do arquivo YAML
var instructions = File.ReadAllText(agentYamlPath);

// Cria o agente declarativo usando OllamaSharp como backend de chat
var triageAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "TriageSentinelV1",
    Description = "Agente de triagem de chamados de suporte técnico",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Temperature = 0.3f, // Temperatura baixa para respostas consistentes e objetivas
        Instructions = instructions
    }
});

// Descrição do chamado de suporte a ser triado
var ticket = """
    Desde as 14h de hoje o endpoint /api/pagamentos está retornando 500
    para todos os clientes. O time de negócio reportou que nenhuma transação
    está sendo processada. Logs mostram NullReferenceException no serviço
    de integração com o gateway de pagamento.
    """;

Console.WriteLine("=== Triagem de Suporte ===\n");
Console.WriteLine($"Chamado recebido:\n{ticket}\n");
Console.WriteLine("Analisando...\n");

// Executa o agente em modo streaming e imprime cada token conforme chega
await foreach (var token in triageAgent.RunStreamingAsync(ticket))
    Console.Write(token);

Console.WriteLine("\n\n=== Fim da triagem ===");
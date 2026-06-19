using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

const string agentMarkDdownOPath = "Agents/code_reviewer.md";
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
var instructions = await File.ReadAllTextAsync(agentMarkDdownOPath);
var code = await File.ReadAllTextAsync(codePath);


var codeReviwerAgent = ollama.AsAIAgent(new ChatClientAgentOptions
{
    Name = "Code reviewer agent",
    Description = "Agente especialista em revisão de codigo c# e .net.",
    ChatOptions = new ChatOptions
    {
        ModelId = model,
        Instructions = instructions,
        Temperature = 0.2f,  
        //MaxOutputTokens = 2000
    }
});   

var result = await codeReviwerAgent.RunAsync(code);
Console.WriteLine(result);

//await foreach (var update in codeReviewerAgent.RunStreamingAsync(code)) 
    //Console.Write(update);
//
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

const string agentMarkdownPath = "Agents/code_reviewer.md";
const string codePath = "Agents/code.txt";
const string model = "gpt-4o-mini";

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = config["OpenAi:ApiKey"];
var openAi = new OpenAIClient(openAiApiKey);

var instructions = File.ReadAllText(agentMarkdownPath);
var code = File.ReadAllText(codePath);

var agent = openAi
    .GetChatClient(model)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "CodeReviewerAgent",
        Description = "Agente especialista em revisão de código C# e .NET",
        ChatOptions = new ChatOptions
        {
            ModelId = model,
            Temperature = 0.7f,
            Instructions = instructions
        }
    });

Console.WriteLine(await agent.RunAsync(code));
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

const string model = "gpt-4o-mini";

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = config["OpenAi:ApiKey"];
var openAi = new OpenAIClient(openAiApiKey);

var imageBytes = await File.ReadAllBytesAsync("imagens/casa.png");
var base64Image = Convert.ToBase64String(imageBytes);
var dataUri = $"data:image/png;base64,{base64Image}";


var agent = openAi
    .GetChatClient(model)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "ImageReviewerAgente",
        Description = "Agente especialista em revisão imagens",
        ChatOptions = new ChatOptions
        {
            ModelId = model,
            Temperature = 0.2f,
            Instructions = "Você vai receber uma imagem ou URL e vai dizer o que tem nela em português"
        }
    });

Microsoft.Extensions.AI.ChatMessage message = new(ChatRole.User, [
    new TextContent("What do you see in this image?"),
    new UriContent(dataUri, "image/jpeg")
]);


Console.WriteLine(await agent.RunAsync(message));

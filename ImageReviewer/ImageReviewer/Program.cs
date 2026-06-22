using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

const string model = "gpt-4o-mini";

// Lê a chave de API a partir dos User Secrets configurados localmente
var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var openAiApiKey = config["OpenAi:ApiKey"];
var openAi = new OpenAIClient(openAiApiKey);

// Lê a imagem do disco e converte para uma Data URI base64
var imageBytes = await File.ReadAllBytesAsync("Images/bandeira-brasil.png");
var base64Image = Convert.ToBase64String(imageBytes);
var dataUri = $"data:image/png;base64,{base64Image}";

// Cria o agente MAF a partir do ChatClient do OpenAI
var agent = openAi
    .GetChatClient(model)
    .AsAIAgent(new ChatClientAgentOptions
    {
        Name = "ImageReviewerAgente",
        Description = "Agente especialista em revisão de imagens",
        ChatOptions = new ChatOptions
        {
            ModelId = model,
            Temperature = 0.7f,
            // Instrução de sistema que define o comportamento do agente
            Instructions = "Você vai receber uma imagem ou URL e vai dizer o que tem nela"
        }
    });

// Monta a mensagem multimodal com texto e a imagem em base64
Microsoft.Extensions.AI.ChatMessage message = new(ChatRole.User, [
    new TextContent("What do you see in this image?"),
    new UriContent(dataUri, "image/png")
]);

// Executa o agente e imprime a resposta no console
Console.WriteLine(await agent.RunAsync(message));

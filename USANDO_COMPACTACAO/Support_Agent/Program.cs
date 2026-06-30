using Microsoft.Extensions.AI;
using OllamaSharp;

const string model = "llama3.2";

IChatClient client =
    new OllamaApiClient(new Uri("http://localhost:11434"), model);

List<ChatMessage> history = new();

history.Add(new ChatMessage(
    ChatRole.System,
    """
    Você é um agente de suporte técnico especializado em .NET e C#.
    Responda de forma direta e objetiva.
    """
));

Console.WriteLine("Agente iniciado.");

while (true)
{
    Console.Write("Você: ");

    var pergunta = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(pergunta))
        continue;

    if (pergunta == "sair")
        break;

    history.Add(new ChatMessage(ChatRole.User, pergunta));

    Console.Write("Agente: ");

    var resposta = await client.GetResponseAsync(history);

    Console.WriteLine(resposta.Text);

    history.Add(new ChatMessage(ChatRole.Assistant, resposta.Text));
}
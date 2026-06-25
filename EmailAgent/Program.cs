using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

[Description("Envia um e-mail corporativo para um destinatário.")]
static string EnviarEmail(
    [Description("Endereço de e-mail do destinatário.")] string destinatario,
    [Description("Assunto do e-mail.")] string assunto,
    [Description("Corpo do e-mail.")] string corpo)
{
    // Simulação: em produção, aqui estaria a chamada ao servidor SMTP
    Console.WriteLine("\n[SISTEMA] E-mail enviado:");
    Console.WriteLine($"  Para:     {destinatario}");
    Console.WriteLine($"  Assunto:  {assunto}");
    Console.WriteLine($"  Mensagem: {corpo}\n");

    return $"E-mail enviado com sucesso para {destinatario}.";
}

var funcaoEnviarEmail = AIFunctionFactory.Create(EnviarEmail);
var envioComAprovacao = new ApprovalRequiredAIFunction(funcaoEnviarEmail);

AIAgent agente = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.1:latest")
    .AsAIAgent(
        instructions: """
            Você é um assistente de e-mails corporativos.
            Quando o usuário pedir para enviar um e-mail, use a ferramenta EnviarEmail.
            Sempre confirme os dados antes de acionar a ferramenta.
            Responda em português.
            """,
        tools: [envioComAprovacao]);


        AgentSession sessao = await agente.CreateSessionAsync();

Console.WriteLine("Enviando solicitação ao agente...\n");

var resposta = await agente.RunAsync(
    "Envie um e-mail para joao@empresa.com informando que a reunião de sexta foi cancelada.",
    sessao);

    var pedidosAprovacao = resposta.Messages
    .SelectMany(m => m.Contents)
    .OfType<ToolApprovalRequestContent>()
    .ToList();

    if (!pedidosAprovacao.Any())
{
    Console.WriteLine("Nenhuma aprovação necessária.");
    Console.WriteLine($"[AGENTE]: {resposta}");
    return;
}

// Loop de aprovação: repete enquanto houver pedidos pendentes
while (pedidosAprovacao.Any())
{
    foreach (var pedido in pedidosAprovacao)
    {
        Console.WriteLine($"O agente quer chamar a função: {pedido.ToolCall.ToString()}");
        Console.WriteLine($"Argumentos: {pedido.ToolCall.CallId}");
        Console.Write("\nAutoriza a execução? (s/n): ");

        var entrada = Console.ReadLine()?.Trim().ToLower();
        var aprovado = entrada == "s" || entrada == "sim";

        if (aprovado)
            Console.WriteLine("Execução autorizada.\n");
        else
            Console.WriteLine("Execução negada.\n");

        // Envia a resposta de aprovação ou rejeição na mesma sessão
        var mensagemResposta = new ChatMessage(
            ChatRole.User,
            [pedido.CreateResponse(aprovado)]);

        resposta = await agente.RunAsync(mensagemResposta, sessao);
    }

    // Verifica se surgiram novos pedidos de aprovação após a rodada anterior
    pedidosAprovacao = resposta.Messages
        .SelectMany(m => m.Contents)
        .OfType<ToolApprovalRequestContent>()
        .ToList();
}

Console.WriteLine($"\n[AGENTE]: {resposta}");
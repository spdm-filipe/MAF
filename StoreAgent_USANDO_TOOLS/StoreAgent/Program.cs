
using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using OllamaSharp;

// Configuração do cliente e registro das tools
var chatClient = new OllamaApiClient(new Uri("http://localhost:11434"), "llama3.1:latest")
    .AsAIAgent(
        instructions: "Você é um assistente de loja virtual. Use as ferramentas disponíveis para responder perguntas sobre produtos.",
        tools:
        [
            AIFunctionFactory.Create(GetProductPrice),
            AIFunctionFactory.Create(CheckStock),
            AIFunctionFactory.Create(ListProductsByCategory) 
        ]);

//var result = await chatClient.RunAsync("Qual o preço do notebook gamer e tem em estoque?");
var result = await chatClient.RunAsync("Quais periféricos vocês têm? O mouse sem fio tem em estoque?");
Console.WriteLine(result);

[Description("Retorna o preço de um produto pelo nome.")]
static string GetProductPrice(
    [Description("Nome do produto a consultar.")] string productName)
{
    // Simula uma base de dados de preços
    var prices = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
    {
        ["notebook gamer"] = 4599.90m,
        ["mouse sem fio"]  = 129.90m,
        ["teclado mecânico"] = 349.90m,
        ["monitor 27\""]   = 1899.90m,
    };

    return prices.TryGetValue(productName, out var price)
        ? $"O preço de '{productName}' é R$ {price:F2}."
        : $"Produto '{productName}' não encontrado no catálogo.";
}

[Description("Verifica se um produto está disponível em estoque.")]
static string CheckStock(
    [Description("Nome do produto a verificar.")] string productName)
{
    // Simula um sistema de estoque
    var stock = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
    {
        ["notebook gamer"]   = 3,
        ["mouse sem fio"]    = 15,
        ["teclado mecânico"] = 0,
        ["monitor 27\""]     = 7,
    };

    if (!stock.TryGetValue(productName, out var qty))
        return $"Produto '{productName}' não encontrado no estoque.";

    return qty > 0
        ? $"'{productName}' está disponível: {qty} unidade(s) em estoque."
        : $"'{productName}' está sem estoque no momento.";
}

[Description("Lista os produtos disponíveis em uma categoria.")]
static string ListProductsByCategory(
    [Description("Categoria dos produtos (ex: periféricos, computadores, monitores).")] string category)
{
    var catalog = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
    {
        ["computadores"]  = ["notebook gamer"],
        ["periféricos"]   = ["mouse sem fio", "teclado mecânico"],
        ["monitores"]     = ["monitor 27\""],
    };

    return catalog.TryGetValue(category, out var products)
        ? $"Produtos em '{category}': {string.Join(", ", products)}."
        : $"Categoria '{category}' não encontrada.";
}
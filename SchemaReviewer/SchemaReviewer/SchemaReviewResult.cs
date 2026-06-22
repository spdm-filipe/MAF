// Contrato de saída: cada campo corresponde a uma chave no JSON retornado pelo agente
public sealed record SchemaReviewResult
{
    // Pontuação geral do schema de 0 a 100
    public int OverallScore { get; init; }

    // Letra de conceito: A, B, C, D ou F
    public string Grade { get; init; } = string.Empty;

    // Resumo executivo da revisão em 2 a 3 frases
    public string Summary { get; init; } = string.Empty;

    // Lista de problemas de normalização identificados
    public List<string> NormalizationIssues { get; init; } = [];

    // Lista de índices recomendados no formato "tabela.coluna: motivo"
    public List<string> RecommendedIndexes { get; init; } = [];

    // Sugestões de separação ou criação de novas tabelas
    public List<string> StructuralSuggestions { get; init; } = [];

    // Próximos passos práticos para o desenvolvedor
    public List<string> NextSteps { get; init; } = [];
}
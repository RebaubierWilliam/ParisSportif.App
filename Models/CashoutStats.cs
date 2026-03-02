using System.Text.Json.Serialization;

namespace ParisSportif.Models;

/// <summary>
/// Stats live extraites depuis FlashScore
/// </summary>
public class CashoutStats
{
    [JsonPropertyName("score")]
    public string? Score { get; set; }

    [JsonPropertyName("minute")]
    public string? Minute { get; set; }

    [JsonPropertyName("rows")]
    public List<StatRow> Rows { get; set; } = new List<StatRow>();
}

public class StatRow
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("home")]
    public string Home { get; set; } = "";

    [JsonPropertyName("away")]
    public string Away { get; set; } = "";
}

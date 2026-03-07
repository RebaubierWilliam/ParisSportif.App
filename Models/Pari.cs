using System.Text.Json.Serialization;

namespace ParisSportif.Models;

/// <summary>
/// Correspond à l'objet JS extrait par extracterPariDepuisCard()
/// </summary>
public class Pari
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Simple";

    [JsonPropertyName("sport")]
    public string Sport { get; set; } = "Autre";

    [JsonPropertyName("equipes")]
    public List<string> Equipes { get; set; } = new List<string>();

    [JsonPropertyName("score")]
    public string? Score { get; set; }

    [JsonPropertyName("isLive")]
    public bool IsLive { get; set; }

    [JsonPropertyName("minuteOuHeure")]
    public string MinuteOuHeure { get; set; } = "";

    [JsonPropertyName("marche")]
    public string Marche { get; set; } = "";

    [JsonPropertyName("selection")]
    public string Selection { get; set; } = "";

    [JsonPropertyName("cote")]
    public string Cote { get; set; } = "";

    [JsonPropertyName("coteTotale")]
    public string CoteTotale { get; set; } = "";

    [JsonPropertyName("mise")]
    public string Mise { get; set; } = "";

    [JsonPropertyName("gainsPotentiels")]
    public string GainsPotentiels { get; set; } = "";

    [JsonPropertyName("cashout")]
    public string? Cashout { get; set; }

    [JsonPropertyName("numeroPari")]
    public string NumeroPari { get; set; } = "";

    [JsonPropertyName("datePari")]
    public string DatePari { get; set; } = "";

    [JsonPropertyName("pourcentagesParis")]
    public List<string> PourcentagesParis { get; set; } = new List<string>();

    // ── Métadonnées C# (non présentes dans le JS) ──────────────────
    public DateTime DateExtraction { get; set; } = DateTime.Now;
    public string? PromptGenere { get; set; }
    // ── Helpers ────────────────────────────────────────────────────
    public string MatchLabel =>
        Equipes.Count >= 2 ? $"{Equipes[0]} vs {Equipes[1]}"
                           : string.Join(" ", Equipes);

    public string StatutLabel => IsLive ? "🔴 LIVE" : "⏳ À venir";

    /// <summary>Répartition des % de parieurs, ex: "1: 55% / N: 22% / 2: 23%"</summary>
    public string RepartitionLabel
    {
        get
        {
            if (PourcentagesParis == null || PourcentagesParis.Count == 0) return "-";
            var parts = new List<string>();
            var selLabels = (Selection ?? "").Split('/').Select(s => s.Trim()).ToList();
            for (int i = 0; i < PourcentagesParis.Count; i++)
            {
                var lbl = i < selLabels.Count ? selLabels[i] : (i + 1).ToString();
                parts.Add($"{lbl}: {PourcentagesParis[i]}");
            }
            return string.Join(" / ", parts);
        }
    }

    public string SportEmoji => Sport switch
    {
        "Football"    => "⚽",
        "Tennis"      => "🎾",
        "Basketball"  => "🏀",
        "Handball"    => "🤾",
        "Rugby"       => "🏉",
        "Hockey"      => "🏒",
        "Volley Ball" => "🏐",
        _             => "🎯"
    };
}

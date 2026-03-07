using System.Globalization;
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

    // ── Indicateur d'anomalie : % parieurs vs probabilité implicite bookmaker ──

    /// <summary>
    /// Pour chaque issue : diff = % parieurs − (100 / cote).
    /// Un grand écart positif = le public surparie cette issue vs ce que le book implique.
    /// Un grand écart négatif = le public sous-parie = potentiel signal de valeur.
    /// </summary>
    public record AnomalieIssue(string Sel, double Cote, double BettorPct, double ImpliedPct, double Diff);

    public List<AnomalieIssue> Anomalies
    {
        get
        {
            if (PourcentagesParis == null || PourcentagesParis.Count == 0)
                return new List<AnomalieIssue>();

            var coteParts = (Cote ?? "").Split('/').Select(s => s.Trim()).ToList();
            var selParts  = (Selection ?? "").Split('/').Select(s => s.Trim()).ToList();
            var result    = new List<AnomalieIssue>();

            for (int i = 0; i < PourcentagesParis.Count; i++)
            {
                var bPct    = ParsePct(PourcentagesParis[i]);
                var cote    = i < coteParts.Count ? ParseCote(coteParts[i]) : 0.0;
                if (cote <= 0 || bPct <= 0) continue;
                var implied = 100.0 / cote;
                var diff    = bPct - implied;
                var sel     = i < selParts.Count ? selParts[i] : (i + 1).ToString();
                result.Add(new AnomalieIssue(sel, cote, bPct, implied, diff));
            }
            return result;
        }
    }

    /// <summary>Probabilité implicite par issue, ex: "1: 54%  N: 24%  2: 20%"</summary>
    public string ProbabiliteImpliciteLabel
    {
        get
        {
            var coteParts = (Cote ?? "").Split('/').Select(s => s.Trim()).ToList();
            var selParts  = (Selection ?? "").Split('/').Select(s => s.Trim()).ToList();
            if (coteParts.Count == 0) return "-";

            var parts = new List<string>();
            for (int i = 0; i < coteParts.Count; i++)
            {
                var c = ParseCote(coteParts[i]);
                if (c <= 0) continue;
                var pct = 100.0 / c;
                var sel = i < selParts.Count ? selParts[i] : (i + 1).ToString();
                parts.Add($"{sel}: {pct:F0}%");
            }
            return parts.Count > 0 ? string.Join("\n", parts) : "-";
        }
    }

    /// <summary>Ecart max absolu — sert au tri de la colonne.</summary>
    public double AnomalieMax =>
        Anomalies.Any() ? Anomalies.Max(a => Math.Abs(a.Diff)) : -1.0;

    /// <summary>Label affiché dans le DataGrid, ex: "🔴 +28% [1]"</summary>
    public string AnomalieLabelMax
    {
        get
        {
            var list = Anomalies;
            if (!list.Any()) return "-";

            var top  = list.OrderByDescending(a => Math.Abs(a.Diff)).First();
            var abs  = Math.Abs(top.Diff);
            if (abs < 5) return "-";

            var emoji = abs >= 20 ? "🚨" : abs >= 12 ? "⚠️" : "💡";
            var sign  = top.Diff >= 0 ? "+" : "";
            return $"{emoji} {sign}{top.Diff:F0}%  [{top.Sel}]";
        }
    }

    /// <summary>Détail de toutes les issues, ex: "1: +28%  N: -5%  2: -18%"</summary>
    public string AnomalieDetail
    {
        get
        {
            var list = Anomalies;
            if (!list.Any()) return "-";
            return string.Join("  ", list.Select(a =>
            {
                var sign = a.Diff >= 0 ? "+" : "";
                return $"{a.Sel}: {sign}{a.Diff:F0}%";
            }));
        }
    }

    private static double ParseCote(string s) =>
        double.TryParse(s.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) && v > 1 ? v : 0;

    private static double ParsePct(string s) =>
        double.TryParse(s.Replace("%", "").Replace(",", ".").Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var v) ? v : 0;

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

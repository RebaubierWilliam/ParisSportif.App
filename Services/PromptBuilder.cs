using System.IO;
using ParisSportif.Models;

namespace ParisSportif.Services;

/// <summary>
/// Construit le prompt cashout : données du pari + protocole sport-spécifique.
/// </summary>
public static class PromptBuilder
{
    private static readonly string Sep = new('═', 52);

    /// <summary>
    /// Point d'entrée unique : route vers cashout (live) ou value bet (à venir).
    /// </summary>
    public static string Build(Pari p, CashoutStats? stats = null) =>
        p.IsLive ? BuildCashoutPrompt(p, stats) : BuildValueBetPrompt(p);

    public static string BuildCashoutPrompt(Pari p, CashoutStats? stats = null)
    {
        var context  = BuildContext(p, stats);
        var template = LoadCashoutTemplate(p.Sport);
        return context + "\n\n" + template;
    }

    public static string BuildValueBetPrompt(Pari p)
    {
        var lines = new List<string>
        {
            "ANALYSE VALUE BET — PARI À VENIR",
            Sep,
            "",
            "📋 PARI À ANALYSER",
            $"Match      : {p.MatchLabel}",
            $"Sport      : {p.Sport}",
            $"Sélection  : {p.Selection} @ cote {p.Cote}",
            $"Marché     : {p.Marche}",
            $"Mise prévue: {p.Mise}",
            $"Date       : {p.DatePari}",
            "",
            Sep,
            "UTILISE LE PROTOCOLE VALUE BET CI-DESSOUS",
            Sep,
        };

        var template = LoadValueBetTemplate(p.Sport);
        return string.Join("\n", lines) + "\n\n" + template;
    }

    // ── Contexte du pari injecté en tête du prompt ──────────────────────────
    private static string BuildContext(Pari p, CashoutStats? stats)
    {
        var co    = p.Cashout ?? "N/A";
        var lines = new List<string>
        {
            "ANALYSE CASHOUT EN TEMPS RÉEL",
            Sep,
            "",
            "📋 MON PARI",
            $"Match           : {p.MatchLabel}",
            $"Sport           : {p.Sport}",
            $"Sélection       : {p.Selection} @ cote {p.Cote}",
            $"Mise initiale   : {p.Mise}",
            $"Gains potentiels: {p.GainsPotentiels}",
            $"Cash Out actuel : {co}",
            $"{p.NumeroPari}  |  {p.DatePari}",
            $"Marché          : {p.Marche}",
            ""
        };

        if (stats is not null)
        {
            lines.Add("📊 STATS LIVE — FlashScore (extraction automatique)");
            lines.Add($"Score  : {stats.Score ?? p.Score ?? "N/A"}");
            lines.Add($"Minute : {stats.Minute ?? p.MinuteOuHeure}");

            if (stats.Rows.Count > 0)
            {
                lines.Add("");
                var h0 = Trunc(p.Equipes.ElementAtOrDefault(0) ?? "Eq.1", 10);
                var h1 = Trunc(p.Equipes.ElementAtOrDefault(1) ?? "Eq.2", 10);
                lines.Add("┌──────────────────────────────────────────────┐");
                lines.Add($"│ {"Statistique",-22} │ {h0,-10} │ {h1,-10} │");
                lines.Add("├──────────────────────────────────────────────┤");
                foreach (var r in stats.Rows)
                    lines.Add($"│ {Trunc(r.Name, 22),-22} │ {r.Home,10} │ {r.Away,-10} │");
                lines.Add("└──────────────────────────────────────────────┘");
            }
        }
        else
        {
            lines.Add("📊 SITUATION EN DIRECT");
            lines.Add(p.Score is not null ? $"Score  : {p.Score}" : "Avant le coup d'envoi");
            lines.Add($"Minute : {p.MinuteOuHeure}");
        }

        lines.Add("");
        lines.Add(Sep);
        lines.Add("UTILISE LE PROTOCOLE CASHOUT CI-DESSOUS POUR CE PARI");
        lines.Add(Sep);

        return string.Join("\n", lines);
    }

    // ── Chargement des templates ─────────────────────────────────────────────
    private static string LoadValueBetTemplate(string sport)
    {
        var fileName = (sport ?? "").ToLowerInvariant() switch
        {
            "football" or "foot" or "soccer" => "foot.md",
            "tennis"                          => "tennis.md",
            "basketball" or "basket"          => "basketball.md",
            _                                 => "Autres.md",
        };

        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", fileName);

        if (File.Exists(path))
            return File.ReadAllText(path, System.Text.Encoding.UTF8);

        var fallback = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", "Autres.md");

        return File.Exists(fallback)
            ? File.ReadAllText(fallback, System.Text.Encoding.UTF8)
            : "";
    }

    private static string LoadCashoutTemplate(string sport)
    {
        var fileName = (sport ?? "").ToLowerInvariant() switch
        {
            "football" or "foot" or "soccer" => "cashout_foot.md",
            "tennis"                          => "cashout_tennis.md",
            "basketball" or "basket"          => "cashout_basketball.md",
            _                                 => "cashout_autres.md",
        };

        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", fileName);

        if (File.Exists(path))
            return File.ReadAllText(path, System.Text.Encoding.UTF8);

        // Fallback générique
        var fallback = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", "cashout_autres.md");

        return File.Exists(fallback)
            ? File.ReadAllText(fallback, System.Text.Encoding.UTF8)
            : "";
    }

    private static string Trunc(string s, int max) =>
        s.Length <= max ? s : s[..max];
}

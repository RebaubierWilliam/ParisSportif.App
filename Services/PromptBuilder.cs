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
    public static string Build(Pari p) =>
        p.IsLive ? BuildCashoutPrompt(p) : BuildValueBetPrompt(p);

    /// <summary>
    /// Génère un prompt de scan value pour une liste de paris.
    /// Compare les cotes ParionsSport avec les gros bookmakers et détecte la value.
    /// </summary>
    public static string BuildValueScanPrompt(IEnumerable<Pari> paris)
    {
        // Priorise les paris à venir, puis live — limite à 15 pour que le LLM puisse traiter
        const int MaxParis = 15;
        var list = paris
            .OrderBy(p => p.IsLive ? 1 : 0)   // à venir en premier
            .Take(MaxParis)
            .ToList();

        var totalInput = paris.Count();
        var truncMsg = totalInput > MaxParis
            ? $" (limité à {MaxParis}/{totalInput} — à-venir en priorité)"
            : "";

        var lines = new List<string>
        {
            "SCAN VALUE — COMPARAISON MULTI-BOOKMAKERS",
            Sep,
            "",
            $"📋 {list.Count} PARIS À ANALYSER{truncMsg} — {DateTime.Now:dd/MM/yyyy HH:mm}",
            "",
        };

        // Tableau récapitulatif des paris
        lines.Add("┌────┬──────────────────────────────────┬───────────────────────┬──────────┬────────┬──────┐");
        lines.Add("│ #  │ Match                            │ Sélection             │ Marché   │ Cote   │ Mise │");
        lines.Add("├────┼──────────────────────────────────┼───────────────────────┼──────────┼────────┼──────┤");

        int i = 1;
        foreach (var p in list)
        {
            var match    = Trunc(p.MatchLabel, 32);
            var sel      = Trunc(p.Selection,  21);
            var marche   = Trunc(p.Marche,      8);
            var cote     = Trunc(p.Cote,         6);
            var mise     = Trunc(p.Mise,          4);
            var live     = p.IsLive ? " 🔴" : "";
            lines.Add($"│ {i,-2} │ {match,-32} │ {sel,-21} │ {marche,-8} │ {cote,-6} │ {mise,-4} │{live}");
            i++;
        }

        lines.Add("└────┴──────────────────────────────────┴───────────────────────┴──────────┴────────┴──────┘");
        lines.Add("");

        // Détail de chaque pari + requête de recherche pré-générée
        lines.Add("DÉTAIL DES PARIS + REQUÊTES DE RECHERCHE");
        lines.Add(Sep);
        i = 1;
        foreach (var p in list)
        {
            lines.Add($"");
            lines.Add($"#{i} — {p.MatchLabel}{(p.IsLive ? " 🔴 LIVE" : "")}");
            lines.Add($"   Sport      : {p.Sport}");
            lines.Add($"   Sélection  : {p.Selection}");
            lines.Add($"   Marché     : {p.Marche}");
            lines.Add($"   Cote PS    : {p.Cote}");
            lines.Add($"   Mise       : {p.Mise}  |  Gains potentiels : {p.GainsPotentiels}");
            if (p.IsLive && p.Score != null)
                lines.Add($"   Score live : {p.Score} ({p.MinuteOuHeure})");
            lines.Add($"   🔍 Recherche : {BuildSearchQuery(p)}");
            i++;
        }

        lines.Add("");
        lines.Add(Sep);
        lines.Add("UTILISE LE PROTOCOLE SCAN VALUE CI-DESSOUS POUR TOUS CES PARIS");
        lines.Add(Sep);

        var template = LoadScanTemplate();
        return string.Join("\n", lines) + "\n\n" + template;
    }

    public static string BuildCashoutPrompt(Pari p)
    {
        var context  = BuildContext(p);
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
    private static string BuildContext(Pari p)
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
            "",
            "📊 SITUATION EN DIRECT",
        };

        lines.Add(p.Score is not null ? $"Score  : {p.Score}" : "Avant le coup d'envoi");
        lines.Add($"Minute : {p.MinuteOuHeure}");

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

    private static string LoadScanTemplate()
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", "value_scan.md");

        return File.Exists(path)
            ? File.ReadAllText(path, System.Text.Encoding.UTF8)
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

    private static string BuildSearchQuery(Pari p)
    {
        var match = p.MatchLabel;
        return (p.Sport ?? "").ToLowerInvariant() switch
        {
            "football" or "foot" or "soccer"
                => $"\"{match}\" odds Pinnacle Bet365 Betclic site:oddsportal.com OR site:oddspedia.com",
            "tennis"
                => $"\"{match}\" tennis odds Pinnacle comparison",
            "basketball" or "basket"
                => $"\"{match}\" basketball odds Pinnacle Bet365",
            _
                => $"\"{match}\" odds Pinnacle Bet365 comparison",
        };
    }

    private static string Trunc(string s, int max) =>
        s.Length <= max ? s : s[..max];
}

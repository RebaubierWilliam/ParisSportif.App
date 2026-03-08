using System.IO;
using System.Text;
using ParisSportif.Models;

namespace ParisSportif.Services;

/// <summary>
/// Génère des prompts d'analyse value bet : complet (autonome) ou agent 2 phases.
/// </summary>
public static class PromptBuilder
{
    // ── Routage principal ─────────────────────────────────────────────────────

    /// <summary>Prompt complet avec protocole intégré (autonome).</summary>
    public static string BuildFull(Pari p) =>
        p.IsQuestion ? BuildFullQuestion(p)
                      : BuildFullValueBet(p);

    /// <summary>Prompt agent 2 phases : collecte structurée + analyse mathématique (autonome).</summary>
    public static string BuildAgent(Pari p) =>
        p.IsQuestion ? BuildAgentQuestion(p)
                      : BuildAgentValueBet(p);

    // ── Prompts complets — protocole intégré ────────────────────────────────

    private static string BuildFullValueBet(Pari p)
    {
        var sep   = new string('═', 52);
        var lines = new List<string>
        {
            "ANALYSE VALUE BET",
            sep,
            "",
            "📋 PARI À ANALYSER",
            $"Match      : {p.MatchLabel}",
            $"Sport      : {p.Sport}",
            $"Sélection  : {p.Selection} @ cote {p.Cote}",
            $"Marché     : {p.Marche}",
            $"Mise prévue: {p.Mise}",
            $"Date       : {p.DatePari}",
        };
        lines.AddRange(new[] { "", sep, "UTILISE LE PROTOCOLE VALUE BET CI-DESSOUS", sep });

        var template = LoadValueBetTemplate(p.Sport);
        return string.Join("\n", lines) + "\n\n" + template;
    }

    private static string BuildFullQuestion(Pari p)
    {
        var sep = new string('═', 52);
        var lines = new List<string>
        {
            "ANALYSE PARI QUESTION — PERFORMANCE INDIVIDUELLE",
            sep,
            "",
            "📋 PARI À ANALYSER",
            $"Match      : {p.MatchLabel}",
            $"Sport      : {p.Sport}",
            $"Question   : {p.Marche}",
            $"Sélection  : {p.Selection} @ cote {p.Cote}",
            $"Mise prévue: {p.Mise}",
            $"Date       : {p.DatePari}",
        };

        lines.AddRange(new[] { "", sep, "UTILISE LE PROTOCOLE QUESTION CI-DESSOUS", sep });

        var template = LoadTemplate("question.md");
        return string.Join("\n", lines) + "\n\n" + template;
    }

    // ── Prompts agent 2 phases — collecte + analyse ──────────────────────────

    private static string BuildAgentValueBet(Pari p)
    {
        var sep   = new string('═', 52);
        var lines = new List<string>
        {
            "ANALYSE VALUE BET — MODE AGENT 2 PHASES",
            sep,
            "",
            $"Match      : {p.MatchLabel}",
            $"Sport      : {p.Sport}",
            $"Sélection  : {p.Selection} @ cote {p.Cote}",
            $"Marché     : {p.Marche}",
            $"Mise       : {p.Mise} → Gains {p.GainsPotentiels}",
            $"Date       : {p.DatePari} {p.MinuteOuHeure}",
            $"N° {p.NumeroPari}",
        };

        lines.AddRange(new[] { "", sep, "EXÉCUTE LES 2 PHASES CI-DESSOUS DANS L'ORDRE", sep });

        var template = LoadAgentTemplate(p.Sport);
        return string.Join("\n", lines) + "\n\n" + template;
    }

    private static string BuildAgentQuestion(Pari p)
    {
        var sep = new string('═', 52);
        var lines = new List<string>
        {
            "ANALYSE PARI QUESTION — MODE AGENT 2 PHASES",
            sep,
            "",
            $"Match      : {p.MatchLabel}",
            $"Sport      : {p.Sport}",
            $"Question   : {p.Marche}",
            $"Sélection  : {p.Selection} @ cote {p.Cote}",
            $"Mise       : {p.Mise} → Gains {p.GainsPotentiels}",
            $"Date       : {p.DatePari}",
        };

        lines.AddRange(new[] { "", sep, "EXÉCUTE LES 2 PHASES CI-DESSOUS DANS L'ORDRE", sep });

        var template = LoadTemplate("agent_question.md");
        return string.Join("\n", lines) + "\n\n" + template;
    }

    // ── Scan Value (multi-paris) ────────────────────────────────────────────

    public static string BuildValueScanPrompt(IEnumerable<Pari> paris)
    {
        const int MaxParis = 15;
        var list = paris.Take(MaxParis).ToList();

        var lines = new List<string>
        {
            $"SCAN VALUE — {list.Count} paris — {DateTime.Now:dd/MM HH:mm}",
            "",
            $"{"#",-2}  {"Match",-32}  {"Sport",-12}  {"Sélection",-22}  {"Cote",-6}  Mise",
            new string('─', 85),
        };

        int i = 1;
        foreach (var p in list)
        {
            lines.Add($"{i,-2}  {Trunc(p.MatchLabel, 32),-32}  {Trunc(p.Sport, 12),-12}  {Trunc(p.Selection, 22),-22}  {p.Cote,-6}  {p.Mise}");
            i++;
        }

        lines.Add("");
        lines.Add("🔍 MCP web-scraper — cotes comparées :");
        i = 1;
        foreach (var p in list)
        {
            var q = Encode(p.MatchLabel.Replace(" vs ", " "));
            lines.Add($"• #{i} {p.MatchLabel} → fetch_page(\"https://www.oddsportal.com/search/results/?q={q}\") puis extract_table()");
            i++;
        }

        lines.Add("");
        lines.Add("→ Protocole scan value.");

        return string.Join("\n", lines);
    }

    // ── Templates — chargement par sport ────────────────────────────────────

    private static string LoadValueBetTemplate(string sport)
    {
        var fileName = (sport ?? "").ToLowerInvariant() switch
        {
            "football" or "foot" or "soccer" => "foot.md",
            "tennis"                          => "tennis.md",
            "basketball" or "basket"          => "basketball.md",
            _                                 => "Autres.md",
        };
        var t = LoadTemplate(fileName);
        return t.Length > 0 ? t : LoadTemplate("Autres.md");
    }

    private static string LoadAgentTemplate(string sport)
    {
        var fileName = (sport ?? "").ToLowerInvariant() switch
        {
            "football" or "foot" or "soccer" => "agent_foot.md",
            "tennis"                          => "agent_tennis.md",
            "basketball" or "basket"          => "agent_basketball.md",
            _                                 => "agent_autres.md",
        };
        var t = LoadTemplate(fileName);
        return t.Length > 0 ? t : LoadTemplate("agent_autres.md");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Tente d'extraire le nom du joueur depuis le libellé du marché.
    /// Ex: "M. Mbappé marque ?" → "M. Mbappé"
    /// Ex: "Buteur - A. Dupont" → "A. Dupont"
    /// </summary>
    private static string? ExtraireNomJoueur(string marche)
    {
        if (string.IsNullOrWhiteSpace(marche)) return null;

        var mots = new[] { "marque", "réalise", "reçoit", "scorer", "scores", "to score", "inscrit" };
        foreach (var mot in mots)
        {
            var idx = marche.IndexOf(mot, StringComparison.OrdinalIgnoreCase);
            if (idx > 0) return marche[..idx].Trim().TrimEnd('-', '–', ':').Trim();
        }

        var prefixes = new[] { "buteur", "essai", "passeur", "carton", "scorer" };
        foreach (var prefix in prefixes)
        {
            if (marche.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                var rest = marche[prefix.Length..].TrimStart('-', '–', ':', ' ');
                if (rest.Length > 2) return rest.TrimEnd('?', ' ');
            }
        }

        return null;
    }

    private static string LoadTemplate(string fileName)
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", fileName);
        return File.Exists(path)
            ? File.ReadAllText(path, Encoding.UTF8)
            : "";
    }

    private static string Encode(string s) => Uri.EscapeDataString(s);

    private static string Trunc(string s, int max) =>
        s.Length <= max ? s : s[..max];
}

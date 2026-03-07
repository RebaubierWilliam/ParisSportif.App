using System.IO;
using System.Text;
using ParisSportif.Models;

namespace ParisSportif.Services;

/// <summary>
/// Génère des prompts compacts (user message) + un system prompt global pour Projet Claude.
/// Architecture token-économe : protocoles dans le system prompt (caché), données dans le message.
/// </summary>
public static class PromptBuilder
{
    public static string Build(Pari p) =>
        p.IsLive ? BuildCashoutPrompt(p) : BuildValueBetPrompt(p);

    // ── Prompts compacts — user message ──────────────────────────────────────

    public static string BuildCashoutPrompt(Pari p)
    {
        var co    = p.Cashout ?? "N/A";
        var score = p.Score is not null ? $"Score {p.Score} | " : "";

        var lines = new List<string>
        {
            $"CASHOUT — {p.Sport} 🔴 {p.MinuteOuHeure}",
            $"Match     : {p.MatchLabel} | {score}Cash-out : {co}",
            $"Sélection : {p.Selection} @ {p.Cote} | Mise {p.Mise} → Gains {p.GainsPotentiels}",
            $"Marché    : {p.Marche} | {p.NumeroPari} | {p.DatePari}",
        };

        lines.AddRange(McpScrapingLines(p, isLive: true));
        lines.Add("");
        lines.Add($"→ Protocole cashout {p.Sport.ToLower()}.");

        return string.Join("\n", lines);
    }

    public static string BuildValueBetPrompt(Pari p)
    {
        var lines = new List<string>
        {
            $"VALUE BET — {p.Sport}",
            $"Match     : {p.MatchLabel} | {p.DatePari} {p.MinuteOuHeure}",
            $"Sélection : {p.Selection} @ {p.Cote} | Mise {p.Mise} → Gains {p.GainsPotentiels}",
            $"Marché    : {p.Marche} | {p.NumeroPari}",
        };

        lines.AddRange(McpScrapingLines(p, isLive: false));
        lines.Add("");
        lines.Add($"→ Protocole value bet {p.Sport.ToLower()}.");

        return string.Join("\n", lines);
    }

    public static string BuildValueScanPrompt(IEnumerable<Pari> paris)
    {
        const int MaxParis = 15;
        var list = paris.OrderBy(p => p.IsLive ? 1 : 0).Take(MaxParis).ToList();

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
            var live = p.IsLive ? " 🔴" : "";
            lines.Add($"{i,-2}  {Trunc(p.MatchLabel, 32),-32}  {Trunc(p.Sport, 12),-12}  {Trunc(p.Selection, 22),-22}  {p.Cote,-6}  {p.Mise}{live}");
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

    // ── System prompt global — à coller dans les instructions du Projet Claude ─

    public static string BuildGlobalSystemPrompt()
    {
        var sb = new StringBuilder();
        sb.AppendLine("# ASSISTANT PARIS SPORTIFS");
        sb.AppendLine("## Outils disponibles : MCP web-scraper (fetch_page, extract_text, extract_table)");
        sb.AppendLine("## Règle absolue : scrape les données AVANT d'analyser. Ne jamais inventer de statistiques.");
        sb.AppendLine();

        foreach (var (title, file) in ProtocolFiles())
        {
            var content = LoadTemplate(file);
            if (!string.IsNullOrEmpty(content))
            {
                sb.AppendLine("---");
                sb.AppendLine($"## {title}");
                sb.AppendLine(content);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    // ── Instructions MCP scraper (injectées dans chaque message) ─────────────

    private static IEnumerable<string> McpScrapingLines(Pari p, bool isLive)
    {
        var q     = Encode(p.MatchLabel);
        var sport = (p.Sport ?? "").ToLowerInvariant();

        yield return "";
        yield return "🔍 MCP web-scraper :";

        // Sofascore — toujours pertinent
        yield return $"• fetch_page(\"https://www.sofascore.com/search/#q={q}\")";
        yield return isLive
            ? "  → score live, xG, tirs, possession, momentum"
            : sport is "tennis"
                ? "  → forme récente, H2H, stats par set, ranking"
                : "  → forme récente (5 derniers), H2H, stats attaque/défense";

        // Fotmob pour momentum foot en live
        if (isLive && sport is "football" or "foot" or "soccer")
        {
            yield return $"• fetch_page(\"https://www.fotmob.com/search?term={q}\")";
            yield return "  → momentum graph, xG comparé";
        }

        // Oddsportal pour cotes pré-match
        if (!isLive)
        {
            var qOdds = Encode(p.MatchLabel.Replace(" vs ", " "));
            yield return $"• fetch_page(\"https://www.oddsportal.com/search/results/?q={qOdds}\")";
            yield return "  → cotes Pinnacle / Betclic / Winamax / Bet365 → extract_table()";
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static IEnumerable<(string title, string file)> ProtocolFiles() =>
        new (string, string)[]
        {
            ("CASHOUT FOOTBALL",        "cashout_foot.md"),
            ("CASHOUT TENNIS",          "cashout_tennis.md"),
            ("CASHOUT BASKETBALL",      "cashout_basketball.md"),
            ("CASHOUT AUTRES SPORTS",   "cashout_autres.md"),
            ("VALUE BET FOOTBALL",      "foot.md"),
            ("VALUE BET TENNIS",        "tennis.md"),
            ("VALUE BET BASKETBALL",    "basketball.md"),
            ("VALUE BET AUTRES SPORTS", "Autres.md"),
            ("SCAN VALUE MULTI-PARIS",  "value_scan.md"),
        };

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

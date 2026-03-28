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

    /// <summary>Prompt Mamouth 2 phases séparées (&lt;800 mots chacune).</summary>
    public static (string Phase1, string Phase2) BuildMamouth(Pari p)
    {
        var header = BuildMamouthHeader(p);
        var template = LoadMamouthTemplate(p.Sport);
        var parts = template.Split(new[] { "===PHASE2===" }, 2, StringSplitOptions.None);

        var phase1 = header + "\n\n" + parts[0].Trim();
        var phase2 = parts.Length > 1
            ? parts[1].Trim()
            : "Phase 2 non trouvée dans le template.";

        return (phase1, phase2);
    }


    // ── Templates — chargement par sport ────────────────────────────────────

    // ── Mamouth — header + template ──────────────────────────────────────────

    private static string BuildMamouthHeader(Pari p)
    {
        var lines = new List<string>
        {
            $"Match : {p.MatchLabel}",
            $"Sport : {p.Sport}",
        };
        if (p.IsQuestion)
            lines.Add($"Question : {p.Marche}");
        lines.Add($"Selection : {p.Selection} @ cote {p.Cote}");
        lines.Add($"Marche : {p.Marche}");
        lines.Add($"Date : {p.DatePari} {p.MinuteOuHeure}");
        return string.Join("\n", lines);
    }

    private static string LoadMamouthTemplate(string sport)
    {
        var fileName = (sport ?? "").ToLowerInvariant() switch
        {
            "football" or "foot" or "soccer"                          => "mamouth_foot.md",
            "tennis"                                                   => "mamouth_tennis.md",
            "basketball" or "basket"                                   => "mamouth_basketball.md",
            "hockey" or "hockey sur glace" or "ice hockey"            => "mamouth_hockey.md",
            "volleyball" or "volley" or "volley-ball" or "volley ball" => "mamouth_volleyball.md",
            "tennis de table" or "ping pong" or "table tennis" or "tt" => "mamouth_tennis_table.md",
            "rugby" or "rugby union" or "rugby league" or " rugby à xv" or "rugby a xv" or "rugby à xiii" or "rugby a xiii" => "mamouth_rugby.md",
            "handball" or "hand"                                       => "mamouth_handball.md",    
            _                                                          => "mamouth_autres.md",
        };
        var t = LoadTemplate(fileName);
        return t.Length > 0 ? t : LoadTemplate("mamouth_autres.md");
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

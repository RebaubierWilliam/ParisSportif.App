using ParisSportif.Models;

namespace ParisSportif.Services;

/// <summary>
/// Construit les prompts cashout en C# (équivalent de buildCashoutPrompt JS).
/// </summary>
public static class PromptBuilder
{
    private static readonly string Sep = new('═', 52);

    public static string BuildCashoutPrompt(Pari p, CashoutStats? stats = null)
    {
        var eq  = p.MatchLabel;
        var co  = p.Cashout ?? "N/A";
        var lines = new List<string>();

        lines.Add("ANALYSE CASHOUT EN TEMPS RÉEL");
        lines.Add(Sep);
        lines.Add("");
        lines.Add("📋 MON PARI");
        lines.Add($"Match           : {eq}");
        lines.Add($"Sport           : {p.Sport}");
        lines.Add($"Sélection       : {p.Selection} @ cote {p.Cote}");
        lines.Add($"Mise initiale   : {p.Mise}");
        lines.Add($"Gains potentiels: {p.GainsPotentiels}");
        lines.Add($"Cash Out actuel : {co}");
        lines.Add($"{p.NumeroPari}  |  {p.DatePari}");
        lines.Add($"Marché          : {p.Marche}");
        lines.Add("");

        if (stats is not null)
        {
            lines.Add("📊 STATS LIVE — FlashScore (extraction automatique)");
            lines.Add($"Score  : {stats.Score ?? p.Score ?? "N/A"}");
            lines.Add($"Minute : {stats.Minute ?? p.MinuteOuHeure}");

            if (stats.Rows.Count > 0)
            {
                lines.Add("");
                var h0 = (p.Equipes.ElementAtOrDefault(0) ?? "Eq.1")[..Math.Min(10, (p.Equipes.ElementAtOrDefault(0) ?? "Eq.1").Length)];
                var h1 = (p.Equipes.ElementAtOrDefault(1) ?? "Eq.2")[..Math.Min(10, (p.Equipes.ElementAtOrDefault(1) ?? "Eq.2").Length)];
                lines.Add("┌──────────────────────────────────────────────┐");
                lines.Add($"│ {"Statistique",-22} │ {h0,-10} │ {h1,-10} │");
                lines.Add("├──────────────────────────────────────────────┤");
                foreach (var r in stats.Rows)
                    lines.Add($"│ {r.Name[..Math.Min(22, r.Name.Length)],-22} │ {r.Home,10} │ {r.Away,-10} │");
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
        lines.Add("OBJECTIF : Cashout MAINTENANT ou laisser courir ?");
        lines.Add(Sep);
        lines.Add("");
        lines.Add("═══ PROBABILITÉS ═══");
        lines.Add($"P_pari    = 1 / {p.Cote} = _____%");
        lines.Add($"P_cashout = {co} / {p.GainsPotentiels} = _____%");
        lines.Add($"P_actuelle({p.Selection}) = ______%");
        lines.Add("");
        lines.Add("═══ EXPECTED VALUE ═══");
        lines.Add($"EV(laisser) = P_act × {p.GainsPotentiels} − (1−P_act) × {p.Mise}");
        lines.Add($"EV(cashout) = {co} − {p.Mise}  ← gain certain");
        lines.Add("Règle : EV(laisser) > EV(cashout) ET P_act > P_cashout → LAISSER COURIR");
        lines.Add("");
        lines.Add("═══ FACTEURS QUALITATIFS ═══");
        lines.Add($"□ Ma sélection ({p.Selection}) domine les stats clés (xG, tirs) ?");
        lines.Add("□ Occasions récentes EN MA FAVEUR ?");
        lines.Add("□ Mon équipe en danger (carton rouge, blessure, pression) ?");
        lines.Add("□ Le score favorise-t-il ma sélection ?");
        lines.Add("□ Risque réel de retournement CONTRE moi ?");
        lines.Add("");
        lines.Add("═══ VERDICT FINAL ═══");
        lines.Add("DÉCISION : [ CASHOUT  /  LAISSER COURIR ]");
        lines.Add($"Cash Out : {co}  →  Gains pot. : {p.GainsPotentiels}");
        lines.Add("Justification : [à remplir par le modèle]");

        return string.Join("\n", lines);
    }
}

using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using ParisSportif.Models;

namespace ParisSportif.Services;

/// <summary>
/// Intégration The Odds API — récupère les cotes des bookmakers et compare avec ParionsSport.
/// Optimisé pour limiter la consommation (500 req/mois gratuit).
/// </summary>
public class OddsApiService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.the-odds-api.com/v4";

    private const string Bookmakers = "pinnacle,betclic,winamax,betfair_ex_eu,bet365,unibet_eu";

    private List<OddsSport>? _sportsCache;
    private int? _requestsRemaining;

    public event Action<string>? LogMessage;
    public int? RequestsRemaining => _requestsRemaining;

    public OddsApiService(string apiKey)
    {
        _apiKey = apiKey;
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("User-Agent", "ParisSportif/1.0");
    }

    public async Task<List<OddsSport>> GetSportsAsync()
    {
        if (_sportsCache != null) return _sportsCache;

        var url = $"{BaseUrl}/sports?apiKey={_apiKey}";
        var json = await _http.GetStringAsync(url);
        _sportsCache = JsonSerializer.Deserialize<List<OddsSport>>(json) ?? new();
        Log($"📡 {_sportsCache.Count} sports disponibles sur The Odds API");
        return _sportsCache;
    }

    public async Task<List<OddsEvent>> GetOddsAsync(string sportKey)
    {
        var url = $"{BaseUrl}/sports/{sportKey}/odds" +
                  $"?apiKey={_apiKey}" +
                  $"&regions=eu" +
                  $"&markets=h2h" +
                  $"&oddsFormat=decimal" +
                  $"&bookmakers={Bookmakers}";

        var response = await _http.GetAsync(url);
        TrackQuota(response);

        if (!response.IsSuccessStatusCode)
        {
            Log($"⚠️ Odds API erreur {(int)response.StatusCode} pour {sportKey}");
            return new();
        }

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<OddsEvent>>(json) ?? new();
    }

    /// <summary>
    /// Point d'entrée : analyse les paris, retourne un résultat par match avec toutes les issues.
    /// </summary>
    public async Task<List<MatchOddsResult>> ScanValueAsync(IEnumerable<Pari> paris)
    {
        var list = paris.ToList();
        if (list.Count == 0) return new();

        var sports = await GetSportsAsync();
        var grouped = list.GroupBy(p => (p.Sport ?? "").ToLowerInvariant()).ToList();
        Log($"📊 {list.Count} paris à scanner sur {grouped.Count} sport(s)");

        var results = new List<MatchOddsResult>();

        foreach (var group in grouped)
        {
            var sportKeys = MapSportToApiKeys(group.Key, sports);
            if (sportKeys.Count == 0)
            {
                Log($"⚠️ Sport '{group.Key}' non mappé sur The Odds API");
                foreach (var p in group)
                    results.Add(new MatchOddsResult
                    {
                        MatchLabel = p.MatchLabel,
                        Selection = p.Selection,
                        CotePS = ParseDecimal(p.Cote),
                        Verdict = "N/D"
                    });
                continue;
            }

            var allEvents = new List<OddsEvent>();
            foreach (var key in sportKeys)
            {
                var events = await GetOddsAsync(key);
                allEvents.AddRange(events);
                Log($"📡 {key} : {events.Count} matchs | Quota : {_requestsRemaining?.ToString() ?? "?"}");
            }

            foreach (var pari in group)
            {
                var ev = FindBestMatch(pari, allEvents);
                if (ev == null)
                {
                    results.Add(new MatchOddsResult
                    {
                        MatchLabel = pari.MatchLabel,
                        Selection = pari.Selection,
                        CotePS = ParseDecimal(pari.Cote),
                        Verdict = "Non trouvé"
                    });
                    continue;
                }

                results.Add(BuildFullComparison(pari, ev));
            }
        }

        return results.OrderByDescending(r => r.DiffPercent).ToList();
    }

    // ── Construction du résultat complet ──────────────────────────────────

    private MatchOddsResult BuildFullComparison(Pari pari, OddsEvent ev)
    {
        // Parser les cotes PS multi-valeurs ("1.50 / 2.50 / 1.80")
        var cotesPS = ParseMultiDecimals(pari.Cote);
        var selectionsPS = pari.Selection.Split(new[] { " / ", "/" }, StringSplitOptions.TrimEntries);
        var cotePS = cotesPS.Length > 0 ? cotesPS[0] : 0;
        Log($"🔍 DEBUG BuildFullComparison: Cote='{pari.Cote}' → cotesPS=[{string.Join(", ", cotesPS)}] | Selection='{pari.Selection}' → selectionsPS=[{string.Join(", ", selectionsPS)}]");

        var result = new MatchOddsResult
        {
            MatchLabel = pari.MatchLabel,
            Selection = pari.Selection,
            CotePS = cotePS,
        };

        // Collecter les noms d'outcomes (depuis le premier bookmaker qui a des h2h)
        var firstH2h = ev.Bookmakers
            .SelectMany(b => b.Markets.Where(m => m.Key == "h2h"))
            .FirstOrDefault();
        if (firstH2h == null)
        {
            result.Verdict = "Pas de cotes h2h";
            return result;
        }

        result.OutcomeNames = firstH2h.Outcomes.Select(o => o.Name).ToList();

        // Mapper les cotes PS extraites sur les outcomes API
        // Si on a le même nombre de cotes que d'outcomes, aligner directement
        result.CotesPSParOutcome = new decimal[result.OutcomeNames.Count];
        Log($"🔍 DEBUG: OutcomeNames=[{string.Join(", ", result.OutcomeNames)}] count={result.OutcomeNames.Count} | cotesPS.Length={cotesPS.Length}");

        if (cotesPS.Length == result.OutcomeNames.Count)
        {
            // Alignement direct : les cotes PS sont dans le même ordre que les sélections
            // Mais il faut vérifier si l'ordre correspond aux outcomes API
            // On essaie de matcher selectionsPS[i] avec OutcomeNames[j]
            var mapped = new bool[result.OutcomeNames.Count];
            for (int s = 0; s < selectionsPS.Length && s < cotesPS.Length; s++)
            {
                int bestIdx = MatchOutcomeIndex(selectionsPS[s], result.OutcomeNames, mapped);
                if (bestIdx >= 0)
                {
                    result.CotesPSParOutcome[bestIdx] = cotesPS[s];
                    mapped[bestIdx] = true;
                }
            }
        }
        else if (cotesPS.Length == 1)
        {
            // Pari simple : une seule cote, possiblement plusieurs sélections (ex: "CHA Hornets / N / MIA Heat")
            // On essaie de matcher chaque sélection PS avec un outcome API
            var used = new bool[result.OutcomeNames.Count];
            int matchedIdx = -1;
            for (int s = 0; s < selectionsPS.Length; s++)
            {
                int idx = MatchOutcomeIndex(selectionsPS[s], result.OutcomeNames, used);
                if (idx >= 0 && matchedIdx < 0)
                    matchedIdx = idx; // On prend le premier match trouvé
            }
            // Si aucun match par nom, fallback sur l'outcome 0 (home team)
            if (matchedIdx < 0 && result.OutcomeNames.Count > 0)
                matchedIdx = 0;
            Log($"🔍 DEBUG: cotesPS==1 branch → matchedIdx={matchedIdx}");
            if (matchedIdx >= 0)
                result.CotesPSParOutcome[matchedIdx] = cotesPS[0];
        }

        // Collecter les bookmakers et leurs cotes pour chaque outcome
        foreach (var bm in ev.Bookmakers)
        {
            var market = bm.Markets.FirstOrDefault(m => m.Key == "h2h");
            if (market == null) continue;

            result.BookmakerNames.Add(ShortBookName(bm.Title));

            var prices = new decimal[result.OutcomeNames.Count];
            for (int i = 0; i < result.OutcomeNames.Count; i++)
            {
                var outcome = market.Outcomes.FirstOrDefault(o => o.Name == result.OutcomeNames[i]);
                prices[i] = outcome?.Price ?? 0;
            }
            result.CotesGrid.Add(prices);
        }

        // Identifier l'outcome correspondant à la sélection du user (pour le verdict principal)
        // En mode multi-cotes, on prend le premier qui a une cote PS
        result.SelectedOutcomeIndex = -1;
        if (selectionsPS.Length == 1)
        {
            result.SelectedOutcomeIndex = MatchOutcomeIndex(selectionsPS[0], result.OutcomeNames, new bool[result.OutcomeNames.Count]);
        }
        else
        {
            // Multi-cotes : pas de sélection unique, on prend le max diff
            // On le calculera après
        }

        // Calculer best cote et diff — cherche le meilleur diff parmi les outcomes qui ont une cote PS
        decimal bestDiff = decimal.MinValue;
        int bestDiffOutcome = -1;

        for (int o = 0; o < result.OutcomeNames.Count; o++)
        {
            var psPrice = result.CotesPSParOutcome[o];
            if (psPrice <= 0) continue;

            // Pinnacle ou best comme référence
            decimal? pinnacle = null;
            decimal bestMkt = 0;
            string? bestMktBook = null;

            for (int b = 0; b < result.BookmakerNames.Count; b++)
            {
                var p = result.CotesGrid[b][o];
                if (p <= 0) continue;
                if (p > bestMkt) { bestMkt = p; bestMktBook = result.BookmakerNames[b]; }
                if (result.BookmakerNames[b].Contains("Pinnacle", StringComparison.OrdinalIgnoreCase))
                    pinnacle = p;
            }

            var refPrice = pinnacle ?? (bestMkt > 0 ? bestMkt : (decimal?)null);
            if (refPrice.HasValue && refPrice.Value > 0)
            {
                var diff = Math.Round((psPrice - refPrice.Value) / refPrice.Value * 100, 1);
                if (diff > bestDiff)
                {
                    bestDiff = diff;
                    bestDiffOutcome = o;
                    result.BestCoteSelection = bestMkt > 0 ? bestMkt : null;
                    result.BestBookSelection = bestMktBook;
                }
            }
        }

        // Si on a un pari simple, forcer le selected outcome
        if (result.SelectedOutcomeIndex < 0 && bestDiffOutcome >= 0)
            result.SelectedOutcomeIndex = bestDiffOutcome;

        if (bestDiff > decimal.MinValue)
        {
            result.DiffPercent = bestDiff;
            result.Verdict = bestDiff switch
            {
                > 5m  => "✅✅ VALUE FORTE",
                > 1m  => "✅ VALUE",
                > -1m => "⚠️ NEUTRE",
                _     => "❌ SURCOTE"
            };
            // En mode simple, CotePS = la cote de la sélection
            if (result.SelectedOutcomeIndex >= 0)
                result.CotePS = result.CotesPSParOutcome[result.SelectedOutcomeIndex];
        }
        else
        {
            result.Verdict = result.CotesPSParOutcome.Any(c => c > 0) ? "Pas de réf." : "Cotes PS non parsées";
        }

        return result;
    }

    /// <summary>
    /// Trouve l'index de l'outcome API qui correspond le mieux à un label PS.
    /// </summary>
    private static int MatchOutcomeIndex(string selPS, List<string> outcomeNames, bool[] used)
    {
        var selNorm = Normalize(selPS);

        // Cas spécial nul/draw
        if (selNorm.Contains("nul") || selNorm.Contains("draw") || selNorm == "x" || selNorm == "n")
        {
            for (int i = 0; i < outcomeNames.Count; i++)
                if (!used[i] && outcomeNames[i] == "Draw") return i;
        }

        // Fuzzy match
        for (int i = 0; i < outcomeNames.Count; i++)
        {
            if (used[i]) continue;
            var nameNorm = Normalize(outcomeNames[i]);
            if (FuzzyMatch(nameNorm, selNorm) || FuzzyMatch(selNorm, nameNorm))
                return i;
        }
        return -1;
    }


    private static string ShortBookName(string title) => title switch
    {
        "Pinnacle" => "Pinnacle",
        _ when title.Contains("Bet365", StringComparison.OrdinalIgnoreCase) => "Bet365",
        _ when title.Contains("Betclic", StringComparison.OrdinalIgnoreCase) => "Betclic",
        _ when title.Contains("Winamax", StringComparison.OrdinalIgnoreCase) => "Winamax",
        _ when title.Contains("Unibet", StringComparison.OrdinalIgnoreCase) => "Unibet",
        _ when title.Contains("Betfair", StringComparison.OrdinalIgnoreCase) => "Betfair",
        _ => title.Length > 10 ? title[..10] : title
    };

    // ── Mapping sport PS → clés API ──────────────────────────────────────

    private static List<string> MapSportToApiKeys(string sportPS, List<OddsSport> available)
    {
        var groups = sportPS switch
        {
            "football" or "foot" or "soccer" => new[] { "Soccer" },
            "tennis" => new[] { "Tennis" },
            "basketball" or "basket" => new[] { "Basketball" },
            "handball" => new[] { "Handball" },
            "rugby" => new[] { "Rugby League", "Rugby Union" },
            "hockey" => new[] { "Ice Hockey" },
            _ => Array.Empty<string>()
        };

        return available
            .Where(s => s.Active && groups.Any(g =>
                s.Group.Equals(g, StringComparison.OrdinalIgnoreCase)))
            .Select(s => s.Key)
            .ToList();
    }

    // ── Fuzzy matching ───────────────────────────────────────────────────

    private static OddsEvent? FindBestMatch(Pari pari, List<OddsEvent> events)
    {
        if (pari.Equipes.Count < 2) return null;

        var home = Normalize(pari.Equipes[0]);
        var away = Normalize(pari.Equipes[1]);

        OddsEvent? best = null;
        int bestScore = 0;

        foreach (var ev in events)
        {
            var evHome = Normalize(ev.HomeTeam);
            var evAway = Normalize(ev.AwayTeam);

            int score = 0;
            if (FuzzyMatch(home, evHome)) score += 10;
            else if (FuzzyMatch(home, evAway)) score += 8;

            if (FuzzyMatch(away, evAway)) score += 10;
            else if (FuzzyMatch(away, evHome)) score += 8;

            if (score > bestScore)
            {
                bestScore = score;
                best = ev;
            }
        }

        return bestScore >= 10 ? best : null;
    }

    private static bool FuzzyMatch(string a, string b)
    {
        if (a == b) return true;
        if (a.Contains(b) || b.Contains(a)) return true;

        var wordsA = a.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var wordsB = b.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        int common = wordsA.Count(wa => wordsB.Any(wb => wb == wa || wb.Contains(wa) || wa.Contains(wb)));
        return common >= 1 && (common >= 2 || wordsA.Length == 1 || wordsB.Length == 1);
    }

    private static string Normalize(string s) =>
        s.ToLowerInvariant()
         .Replace("-", " ")
         .Replace("'", "")
         .Replace(".", "")
         .Replace("fc ", "").Replace(" fc", "")
         .Replace("as ", "").Replace(" as", "")
         .Replace("ac ", "").Replace(" ac", "")
         .Replace("rc ", "").Replace(" rc", "")
         .Trim();

    // ── Utilitaires ──────────────────────────────────────────────────────

    private void TrackQuota(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("x-requests-remaining", out var vals))
        {
            var val = vals.FirstOrDefault();
            if (int.TryParse(val, out var remaining))
                _requestsRemaining = remaining;
        }
    }

    private static decimal ParseDecimal(string s)
    {
        s = s.Trim().Replace(",", ".");
        return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0;
    }

    /// <summary>Parse "1.50 / 2.50 / 1.80" ou "1,50" en tableau de decimals.</summary>
    private static decimal[] ParseMultiDecimals(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return Array.Empty<decimal>();
        return s.Split(new[] { " / ", "/", " - ", "|" }, StringSplitOptions.TrimEntries)
                .Select(ParseDecimal)
                .Where(v => v > 0)
                .ToArray();
    }

    private void Log(string msg) => LogMessage?.Invoke(msg);
}

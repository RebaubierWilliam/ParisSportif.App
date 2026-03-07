using System.Text.Json.Serialization;

namespace ParisSportif.Models;

/// <summary>
/// Réponse de The Odds API — /v4/sports
/// </summary>
public class OddsSport
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";

    [JsonPropertyName("group")]
    public string Group { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("active")]
    public bool Active { get; set; }
}

/// <summary>
/// Un événement avec ses cotes — /v4/sports/{sport}/odds
/// </summary>
public class OddsEvent
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("sport_key")]
    public string SportKey { get; set; } = "";

    [JsonPropertyName("sport_title")]
    public string SportTitle { get; set; } = "";

    [JsonPropertyName("commence_time")]
    public DateTime CommenceTime { get; set; }

    [JsonPropertyName("home_team")]
    public string HomeTeam { get; set; } = "";

    [JsonPropertyName("away_team")]
    public string AwayTeam { get; set; } = "";

    [JsonPropertyName("bookmakers")]
    public List<BookmakerOdds> Bookmakers { get; set; } = new();
}

public class BookmakerOdds
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("markets")]
    public List<OddsMarket> Markets { get; set; } = new();
}

public class OddsMarket
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = "";

    [JsonPropertyName("outcomes")]
    public List<OddsOutcome> Outcomes { get; set; } = new();
}

public class OddsOutcome
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}

/// <summary>
/// Résultat complet de la comparaison pour un match.
/// Contient TOUTES les issues (1/N/2) pour TOUS les bookmakers.
/// </summary>
public class MatchOddsResult
{
    public string MatchLabel { get; set; } = "";
    public string Selection { get; set; } = "";
    public decimal CotePS { get; set; }

    /// <summary>Cotes PS extraites pour chaque outcome (index aligné avec OutcomeNames)</summary>
    public decimal[] CotesPSParOutcome { get; set; } = Array.Empty<decimal>();

    /// <summary>Noms des issues dans l'ordre (ex: ["OM", "Nul", "PSG"])</summary>
    public List<string> OutcomeNames { get; set; } = new();

    /// <summary>Noms courts des bookmakers dans l'ordre des colonnes</summary>
    public List<string> BookmakerNames { get; set; } = new();

    /// <summary>
    /// Cotes : BookmakerNames[i] → OutcomeNames[j] → price
    /// Accès : CotesGrid[bookIndex][outcomeIndex]
    /// </summary>
    public List<decimal[]> CotesGrid { get; set; } = new();

    /// <summary>Index de l'outcome correspondant à la sélection du user (-1 si non trouvé)</summary>
    public int SelectedOutcomeIndex { get; set; } = -1;

    /// <summary>Meilleure cote marché pour la sélection du user</summary>
    public decimal? BestCoteSelection { get; set; }
    public string? BestBookSelection { get; set; }

    /// <summary>Diff% de la cote PS vs Pinnacle (ou best sharp) pour la sélection</summary>
    public decimal DiffPercent { get; set; }
    public string Verdict { get; set; } = "";
}

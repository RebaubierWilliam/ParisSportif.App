using Microsoft.Data.Sqlite;
using ParisSportif.Models;
using System.IO;
using System.Text.Json;

namespace ParisSportif.Services;

public class HistoriqueService
{
    private readonly string _dbPath;

    public HistoriqueService()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var folder  = Path.Combine(appData, "ParisSportif");
        Directory.CreateDirectory(folder);
        _dbPath = Path.Combine(folder, "historique.db");
        InitDb();
    }

    private void InitDb()
    {
        using var conn = OpenConnection();
        conn.Execute(@"
            CREATE TABLE IF NOT EXISTS Paris (
                Id             INTEGER PRIMARY KEY AUTOINCREMENT,
                DateExtraction TEXT    NOT NULL,
                NumeroPari     TEXT    NOT NULL,
                Sport          TEXT,
                Equipes        TEXT,
                Selection      TEXT,
                Cote           TEXT,
                Mise           TEXT,
                GainsPotentiels TEXT,
                Cashout        TEXT,
                IsLive         INTEGER DEFAULT 0,
                Score          TEXT,
                Minute         TEXT,
                PromptGenere   TEXT,
                FlashScoreStats TEXT,
                UNIQUE(NumeroPari)
            );");
    }

    public void UpsertParis(IEnumerable<Pari> paris)
    {
        using var conn = OpenConnection();
        using var tx   = conn.BeginTransaction();

        foreach (var p in paris)
        {
            conn.Execute(@"
                INSERT INTO Paris
                    (DateExtraction, NumeroPari, Sport, Equipes, Selection, Cote,
                     Mise, GainsPotentiels, Cashout, IsLive, Score, Minute, PromptGenere, FlashScoreStats)
                VALUES
                    (@de, @np, @sport, @eq, @sel, @cote,
                     @mise, @gp, @co, @live, @score, @min, @prompt, @fs)
                ON CONFLICT(NumeroPari) DO UPDATE SET
                    Cashout        = excluded.Cashout,
                    IsLive         = excluded.IsLive,
                    Score          = excluded.Score,
                    Minute         = excluded.Minute,
                    PromptGenere   = excluded.PromptGenere,
                    FlashScoreStats= excluded.FlashScoreStats;",
                new {
                    de    = p.DateExtraction.ToString("o"),
                    np    = p.NumeroPari,
                    sport = p.Sport,
                    eq    = JsonSerializer.Serialize(p.Equipes),
                    sel   = p.Selection,
                    cote  = p.Cote,
                    mise  = p.Mise,
                    gp    = p.GainsPotentiels,
                    co    = p.Cashout,
                    live  = p.IsLive ? 1 : 0,
                    score = p.Score,
                    min   = p.MinuteOuHeure,
                    prompt = p.PromptGenere,
                    fs    = p.FlashScoreStats is not null
                             ? JsonSerializer.Serialize(p.FlashScoreStats)
                             : (object?)null
                }, tx);
        }

        tx.Commit();
    }

    public List<Pari> GetHistorique(int limit = 100)
    {
        using var conn = OpenConnection();
        var rows = conn.Query("SELECT * FROM Paris ORDER BY DateExtraction DESC LIMIT @limit", new { limit });
        var result = new List<Pari>();

        foreach (var row in rows)
        {
            var p = new Pari
            {
                NumeroPari      = row.NumeroPari ?? "",
                Sport           = row.Sport ?? "",
                Equipes         = JsonSerializer.Deserialize<List<string>>(row.Equipes ?? "[]") ?? new List<string>(),
                Selection       = row.Selection ?? "",
                Cote            = row.Cote ?? "",
                Mise            = row.Mise ?? "",
                GainsPotentiels = row.GainsPotentiels ?? "",
                Cashout         = row.Cashout,
                IsLive          = row.IsLive == 1,
                Score           = row.Score,
                MinuteOuHeure   = row.Minute ?? "",
                PromptGenere    = row.PromptGenere,
                DateExtraction  = DateTime.Parse(row.DateExtraction ?? DateTime.Now.ToString("o")),
                FlashScoreStats = row.FlashScoreStats is not null
                    ? JsonSerializer.Deserialize<CashoutStats>(row.FlashScoreStats)
                    : null
            };
            result.Add(p);
        }

        return result;
    }

    private SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection($"Data Source={_dbPath}");
        conn.Open();
        return conn;
    }
}

// ── Extension minimale pour SqliteConnection (pas de Dapper) ──────────────
internal static class SqliteExtensions
{
    internal static void Execute(this SqliteConnection conn, string sql,
        object? parameters = null, SqliteTransaction? tx = null)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = sql;
        BindParameters(cmd, parameters);
        cmd.ExecuteNonQuery();
    }

    internal static IEnumerable<dynamic> Query(this SqliteConnection conn, string sql, object? parameters = null)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        BindParameters(cmd, parameters);

        using var reader = cmd.ExecuteReader();
        var results = new List<dynamic>();
        while (reader.Read())
        {
            var dict = new System.Dynamic.ExpandoObject() as IDictionary<string, object?>;
            for (int i = 0; i < reader.FieldCount; i++)
                dict[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            results.Add(dict);
        }
        return results;
    }

    private static void BindParameters(SqliteCommand cmd, object? parameters)
    {
        if (parameters is null) return;
        foreach (var prop in parameters.GetType().GetProperties())
        {
            var param = cmd.CreateParameter();
            param.ParameterName = "@" + prop.Name;
            param.Value = prop.GetValue(parameters) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }
    }
}

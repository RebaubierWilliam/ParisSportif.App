using Microsoft.Web.WebView2.Wpf;
using ParisSportif.Models;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ParisSportif.Services;

/// <summary>
/// Injecte les scripts JS dans WebView2 et reçoit les résultats via postMessage.
/// </summary>
public class ExtractionService
{
    private readonly WebView2 _webView;
    private readonly string   _extracteurScript;

    public event Action<List<Pari>>?      ParisExtracted;
    public event Action<CashoutStats>?    FlashScoreStatsReceived;
    public event Action<string>?          LogMessage;

    public ExtractionService(WebView2 webView)
    {
        _webView          = webView;
        _extracteurScript = LoadEmbeddedScript("extracteur_mesParis.js");
    }

    /// <summary>Branche le handler WebMessageReceived une seule fois.</summary>
    public void Attach()
    {
        _webView.WebMessageReceived += OnWebMessage;
    }

    public void Detach()
    {
        _webView.WebMessageReceived -= OnWebMessage;
    }

    /// <summary>Injecte le script extracteur sur la page actuelle.</summary>
    public async Task InjectExtracteurAsync()
    {
        if (_webView.CoreWebView2 is null) return;
        Log("⚙️ Injection du script extracteur…");
        await _webView.CoreWebView2.ExecuteScriptAsync(_extracteurScript);
    }

    /// <summary>Injecte le script FlashScore avec les données du pari pré-encodées.</summary>
    public async Task InjectFlashScoreExtractorAsync(Pari pari)
    {
        if (_webView.CoreWebView2 is null) return;

        var betJson = JsonSerializer.Serialize(pari);
        var b64     = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(betJson));

        var baseScript = LoadEmbeddedScript("flashscore_extractor.js");
        // Remplace le placeholder __BET_B64__ par les données réelles
        var script = baseScript.Replace("__BET_B64__", b64);

        Log("⚙️ Injection du script FlashScore…");
        await _webView.CoreWebView2.ExecuteScriptAsync(script);
    }

    // ── Réception des messages JS → C# ─────────────────────────────────────
    private void OnWebMessage(object? sender,
        Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
    {
        try
        {
            var raw = e.WebMessageAsJson;
            // Le message arrive comme chaîne JSON encodée en JSON (double-sérialisée)
            // On désérialise d'abord le wrapper string
            var json = JsonSerializer.Deserialize<string>(raw) ?? raw;

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var type = root.GetProperty("type").GetString();

            switch (type)
            {
                case "PARIS_EXTRACTED":
                    var paris = JsonSerializer.Deserialize<List<Pari>>(
                        root.GetProperty("data").GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<Pari>();
                    Log($"✅ {paris.Count} paris reçu(s)");
                    ParisExtracted?.Invoke(paris);
                    break;

                case "FS_STATS":
                    var stats = JsonSerializer.Deserialize<CashoutStats>(
                        root.GetProperty("data").GetRawText(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new CashoutStats();
                    Log($"✅ Stats FlashScore reçues ({stats.Rows.Count} ligne(s))");
                    FlashScoreStatsReceived?.Invoke(stats);
                    break;

                case "LOG":
                    Log($"[JS] {root.GetProperty("message").GetString()}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Log($"❌ Erreur parsing message: {ex.Message}");
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private static string LoadEmbeddedScript(string fileName)
    {
        var asm  = Assembly.GetExecutingAssembly();
        var name = asm.GetManifestResourceNames()
                      .FirstOrDefault(n => n.EndsWith(fileName))
                   ?? throw new FileNotFoundException($"Script embarqué introuvable : {fileName}");

        using var stream = asm.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private void Log(string msg) => LogMessage?.Invoke(msg);
}

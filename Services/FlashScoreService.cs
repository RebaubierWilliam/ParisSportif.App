using Microsoft.Web.WebView2.Wpf;
using ParisSportif.Models;
using System.Text.RegularExpressions;

namespace ParisSportif.Services;

/// <summary>
/// Gère la navigation FlashScore et l'injection automatique sur les pages de match.
/// </summary>
public partial class FlashScoreService
{
    private readonly WebView2          _webView;
    private readonly ExtractionService _extraction;

    private Pari? _pendingPari;

    public event Action<string>? LogMessage;

    // Regex : URL FlashScore d'une page de match
    // Exemples : flashscore.com/match/AbCdEfGh/
    //            flashscore.fr/football/france/ligue-1/match/AbCdEfGh/
    [GeneratedRegex(@"flashscore\.(com|fr|co\.uk)/.*(/match/|#[a-zA-Z0-9]{8})", RegexOptions.IgnoreCase)]
    private static partial Regex MatchPageRegex();

    public FlashScoreService(WebView2 webView, ExtractionService extraction)
    {
        _webView    = webView;
        _extraction = extraction;
    }

    public void Attach()
    {
        _webView.CoreWebView2InitializationCompleted += OnCoreReady;
    }

    private void OnCoreReady(object? s,
        Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
    {
        _webView.CoreWebView2.NavigationCompleted += OnNavigationCompleted;
    }

    /// <summary>
    /// Navigue vers FlashScore avec une recherche du match et mémorise le pari en attente.
    /// </summary>
    public void NavigateToMatch(Pari pari)
    {
        _pendingPari = pari;
        var q   = Uri.EscapeDataString(string.Join(" ", pari.Equipes) + " live flashscore");
        var url = $"https://www.google.com/search?q={q}";
        _webView.CoreWebView2?.Navigate(url);
        Log($"🔍 Recherche : {pari.MatchLabel}");
    }

    /// <summary>
    /// Navigue directement vers la page FlashScore du sport correspondant.
    /// </summary>
    public void NavigateToFlashScoreLive(Pari pari)
    {
        _pendingPari = pari;
        var sportPath = pari.Sport.ToLower() switch
        {
            "football"    => "football",
            "tennis"      => "tennis",
            "basketball"  => "basketball",
            "handball"    => "handball",
            "volley ball" => "volleyball",
            "rugby"       => "rugby",
            "hockey"      => "hockey",
            _             => "football"
        };
        _webView.CoreWebView2?.Navigate($"https://www.flashscore.com/{sportPath}/");
        Log($"📺 Ouverture FlashScore {pari.Sport}");
    }

    private async void OnNavigationCompleted(object? sender,
        Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
    {
        if (!e.IsSuccess || _webView.CoreWebView2 is null) return;

        var url = _webView.CoreWebView2.Source;

        // Auto-injection si on est sur une page de match FlashScore ET qu'on a un pari en attente
        if (_pendingPari is not null && MatchPageRegex().IsMatch(url))
        {
            Log($"🎯 Page de match détectée → injection automatique");
            await _extraction.InjectFlashScoreExtractorAsync(_pendingPari);
            // On ne clear pas _pendingPari : l'utilisateur peut naviguer vers d'autres sections
        }
    }

    /// <summary>
    /// Appelé manuellement si l'utilisateur navigue lui-même vers le match.
    /// </summary>
    public async Task InjectManuallyAsync()
    {
        if (_pendingPari is null)
        {
            Log("⚠️ Aucun pari sélectionné. Clique d'abord sur 'Cashout ?' dans la liste.");
            return;
        }
        await _extraction.InjectFlashScoreExtractorAsync(_pendingPari);
    }

    public void SetPendingPari(Pari pari) => _pendingPari = pari;

    private void Log(string msg) => LogMessage?.Invoke(msg);
}

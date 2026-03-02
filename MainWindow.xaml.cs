using Microsoft.Web.WebView2.Core;
using ParisSportif.Models;
using ParisSportif.Services;
using ParisSportif.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;

namespace ParisSportif;

public partial class MainWindow : Window
{
    private readonly MainViewModel     _vm;
    private readonly ExtractionService _extractionPS;   // pour ParionsSport
    private readonly ExtractionService _extractionFS;   // pour FlashScore
    private readonly FlashScoreService _flashScore;
    private readonly HistoriqueService _historique;

    private ICollectionView? _parisView;

    public MainWindow()
    {
        InitializeComponent();

        _historique   = new HistoriqueService();
        _extractionPS = new ExtractionService(WebViewPS);
        _extractionFS = new ExtractionService(WebViewFS);
        _flashScore   = new FlashScoreService(WebViewFS, _extractionFS);

        _vm = new MainViewModel(_historique, _flashScore);
        DataContext = _vm;

        // Branchement des événements
        _extractionPS.ParisExtracted          += _vm.OnParisExtracted;
        _extractionPS.LogMessage              += _vm.Log;
        _extractionFS.FlashScoreStatsReceived += _vm.OnFlashScoreStats;
        _extractionFS.LogMessage              += _vm.Log;
        _flashScore.LogMessage                += _vm.Log;

        // Initialisation WebView2 (async via événement)
        WebViewPS.CoreWebView2InitializationCompleted += OnPSCoreReady;
        WebViewFS.CoreWebView2InitializationCompleted += OnFSCoreReady;

        // Profile persistant (même session entre les lancements)
        InitWebViewEnvironmentAsync();
    }

    // ── Initialisation WebView2 avec profil persistant ─────────────────────
    private async void InitWebViewEnvironmentAsync()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var profileFolder = Path.Combine(appData, "ParisSportif", "WebView2Profile");
        Directory.CreateDirectory(profileFolder);

        var env = await CoreWebView2Environment.CreateAsync(
            userDataFolder: profileFolder);

        await WebViewPS.EnsureCoreWebView2Async(env);
        await WebViewFS.EnsureCoreWebView2Async(env);
    }

    // ── Handlers CoreWebView2 prêt ─────────────────────────────────────────
    private void OnPSCoreReady(object? s, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (!e.IsSuccess) { _vm.Log($"❌ WebView2 PS erreur : {e.InitializationException?.Message}"); return; }

        _extractionPS.Attach();
        _flashScore.Attach();

        // Auto-injection quand on arrive sur la page "Mes paris"
        WebViewPS.CoreWebView2.NavigationCompleted += async (_, args) =>
        {
            if (!args.IsSuccess) return;
            var url = WebViewPS.CoreWebView2.Source;
            TbUrl.Text = url;

            if (url.Contains("mes-paris", StringComparison.OrdinalIgnoreCase))
            {
                _vm.Log("📄 Page Mes paris détectée → injection automatique");
                await _extractionPS.InjectExtracteurAsync();
            }
        };

        // Filtre live via CollectionView
        _parisView = CollectionViewSource.GetDefaultView(_vm.Paris);
        _parisView.Filter = item => item is Pari p && (!_vm.ShowLiveOnly || p.IsLive);
        _vm.PropertyChanged += (_, e2) =>
        {
            if (e2.PropertyName == nameof(_vm.ShowLiveOnly))
                _parisView?.Refresh();
        };
    }

    private void OnFSCoreReady(object? s, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (!e.IsSuccess) return;
        _extractionFS.Attach();
        _flashScore.Attach();
    }

    // ── Gestionnaires de boutons ────────────────────────────────────────────

    private async void BtnExtraire_Click(object sender, RoutedEventArgs e)
        => await _extractionPS.InjectExtracteurAsync();

    private void BtnMesParis_Click(object sender, RoutedEventArgs e)
        => WebViewPS.CoreWebView2?.Navigate("https://www.parionssport.fdj.fr/mes-paris");

    private async void BtnInjecterFS_Click(object sender, RoutedEventArgs e)
        => await _flashScore.InjectManuallyAsync();

    private void BtnEffacerPrompt_Click(object sender, RoutedEventArgs e)
        => _vm.CurrentPrompt = "";

    private void BtnCashout_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        _vm.LancerFlashScoreCommand.Execute(null);
    }

    private void BtnPrompt_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        _vm.GenererPromptCashoutCommand.Execute(null);
    }
}

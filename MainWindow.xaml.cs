using Microsoft.Web.WebView2.Core;
using ParisSportif.Models;
using ParisSportif.Services;
using ParisSportif.ViewModels;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ParisSportif;

public partial class MainWindow : Window
{
    private readonly MainViewModel     _vm;
    private readonly ExtractionService _extractionPS;
    private ICollectionView? _parisView;

    public MainWindow()
    {
        InitializeComponent();

        _extractionPS = new ExtractionService(WebViewPS);
        _vm = new MainViewModel();
        DataContext = _vm;

        _extractionPS.ParisExtracted += OnParisExtracted;
        _extractionPS.LogMessage     += _vm.Log;

        WebViewPS.CoreWebView2InitializationCompleted += OnPSCoreReady;
        WebViewDS.CoreWebView2InitializationCompleted += OnDSCoreReady;
        InitWebViewEnvironmentAsync();
    }

    // ── Initialisation WebView2 avec profil persistant ─────────────────────
    private async void InitWebViewEnvironmentAsync()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var profileFolder = Path.Combine(appData, "ParisSportif", "WebView2Profile");
        Directory.CreateDirectory(profileFolder);

        var env = await CoreWebView2Environment.CreateAsync(userDataFolder: profileFolder);
        await WebViewPS.EnsureCoreWebView2Async(env);

        var dsFolder = Path.Combine(appData, "ParisSportif", "WebView2DeepSeek");
        Directory.CreateDirectory(dsFolder);
        var envDS = await CoreWebView2Environment.CreateAsync(userDataFolder: dsFolder);
        await WebViewDS.EnsureCoreWebView2Async(envDS);

    }

    // ── Handler CoreWebView2 prêt ──────────────────────────────────────────
    private void OnPSCoreReady(object? s, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (!e.IsSuccess) { _vm.Log($"❌ WebView2 erreur : {e.InitializationException?.Message}"); return; }

        WebViewPS.CoreWebView2.Navigate("https://www.parionssport.fdj.fr/");
        _extractionPS.Attach();

        WebViewPS.CoreWebView2.NewWindowRequested += (_, args) =>
        {
            args.Handled = true;
            WebViewPS.CoreWebView2.Navigate(args.Uri);
        };

        _parisView = CollectionViewSource.GetDefaultView(_vm.Paris);
        _parisView.Filter = item => item is Pari p && (!_vm.ShowLiveOnly || p.IsLive);
        _vm.PropertyChanged += (_, e2) =>
        {
            if (e2.PropertyName == nameof(_vm.ShowLiveOnly))
                _parisView?.Refresh();
        };
    }

    // ── Extraction terminée → mise à jour VM + ouverture onglet Paris ──────
    private void OnParisExtracted(List<Pari> paris)
    {
        _vm.OnParisExtracted(paris);
        Dispatcher.Invoke(() =>
        {
            TabParis.Header = $"📊 Paris extraits ({paris.Count})";
            MainTabs.SelectedItem = TabParis;
        });
    }

    // ── Bouton Extraire ────────────────────────────────────────────────────
    private async void BtnExtraire_Click(object sender, RoutedEventArgs e)
        => await _extractionPS.InjectExtracteurAsync();

    // ── Instructions Projet : system prompt global à coller dans claude.ai ────
    private void BtnSystemPrompt_Click(object sender, RoutedEventArgs e)
    {
        var prompt = PromptBuilder.BuildGlobalSystemPrompt();
        Clipboard.SetText(prompt);
        _vm.Log("📋 Instructions Projet copiées — colle dans les instructions de ton Projet claude.ai");
    }

    // ── Scan Value : génère un prompt multi-paris pour comparaison bookmakers ─
    private void BtnScanValue_Click(object sender, RoutedEventArgs e)
    {
        var paris = (_parisView?.Cast<Pari>() ?? _vm.Paris).ToList();
        if (paris.Count == 0) { _vm.Log("⚠️ Aucun pari à analyser."); return; }

        var prompt = PromptBuilder.BuildValueScanPrompt(paris);
        Clipboard.SetText(prompt);

        // Crée un onglet dédié au scan
        const string scanTag = "__VALUE_SCAN__";
        var existing = MainTabs.Items
            .OfType<TabItem>()
            .FirstOrDefault(t => t.Tag is string id && id == scanTag);
        if (existing != null)
        {
            // Met à jour le contenu si l'onglet existe déjà
            if (existing.Content is Grid g &&
                g.Children.OfType<TextBox>().FirstOrDefault() is TextBox tb)
                tb.Text = prompt;
            MainTabs.SelectedItem = existing;
            _vm.Log($"🔍 Scan Value mis à jour ({paris.Count} paris) — prompt copié.");
            return;
        }

        var textBox = new TextBox
        {
            Text              = prompt,
            FontFamily        = new FontFamily("Consolas"),
            FontSize          = 12,
            AcceptsReturn     = true,
            TextWrapping      = TextWrapping.Wrap,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background        = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E)),
            Foreground        = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
            Padding           = new Thickness(14),
            BorderThickness   = new Thickness(0),
        };

        var copyBtn = new Button
        {
            Content = "📋 Copier",
            Padding = new Thickness(12, 6, 12, 6),
            Style   = (Style)FindResource("BtnPrimary"),
        };
        copyBtn.Click += (_, _) => { Clipboard.SetText(textBox.Text); _vm.Log("📋 Scan Value copié."); };

        var dsBtn = new Button
        {
            Content = "🤖 → DeepSeek",
            Padding = new Thickness(12, 6, 12, 6),
            Margin  = new Thickness(8, 0, 0, 0),
            Style   = (Style)FindResource("BtnSecondary"),
            ToolTip = "Bascule sur l'onglet DeepSeek (le prompt est déjà dans le presse-papier)",
        };
        dsBtn.Click += (_, _) => { MainTabs.SelectedItem = TabDeepSeek; _vm.Log("🤖 Basculé sur DeepSeek — colle avec le bouton 'Coller le prompt'."); };

        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 6, 10, 6) };
        toolbar.Children.Add(copyBtn);
        toolbar.Children.Add(dsBtn);

        var content = new Grid();
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(toolbar, 0);
        Grid.SetRow(textBox, 1);
        content.Children.Add(toolbar);
        content.Children.Add(textBox);

        TabItem tabItem = null!;

        var closeBtn = new Button
        {
            Content         = "✕",
            FontSize        = 10,
            Width           = 18,
            Height          = 18,
            Padding         = new Thickness(0),
            Margin          = new Thickness(8, 0, 0, 0),
            Background      = Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor          = Cursors.Hand,
            VerticalAlignment = VerticalAlignment.Center,
        };
        closeBtn.Click += (_, _) => MainTabs.Items.Remove(tabItem);

        var header = new StackPanel { Orientation = Orientation.Horizontal };
        header.Children.Add(new TextBlock
        {
            Text              = $"🔍 Scan Value ({paris.Count})",
            VerticalAlignment = VerticalAlignment.Center,
        });
        header.Children.Add(closeBtn);

        tabItem = new TabItem { Header = header, Content = content, Tag = scanTag };
        MainTabs.Items.Add(tabItem);
        MainTabs.SelectedItem = tabItem;

        _vm.Log($"🔍 Scan Value généré pour {paris.Count} paris — copié dans le presse-papier.");
    }

    // ── Bouton Prompt MCP : prompt compact avec instructions web-scraper ──────
    private void BtnPrompt_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        OpenPromptTab(pari, PromptBuilder.Build(pari), pari.NumeroPari, $"📋 {pari.MatchLabel}");
        _vm.Log($"📋 Prompt MCP généré pour {pari.MatchLabel} — copié dans le presse-papier");
    }

    // ── Bouton Prompt Complet : prompt autonome avec protocole intégré ────────
    private void BtnPromptComplet_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        OpenPromptTab(pari, PromptBuilder.BuildFull(pari), pari.NumeroPari + "_full", $"📄 {pari.MatchLabel}");
        _vm.Log($"📄 Prompt complet généré pour {pari.MatchLabel} — copié dans le presse-papier");
    }

    // ── Création/activation d'un onglet prompt ────────────────────────────────
    private void OpenPromptTab(Pari pari, string prompt, string tag, string headerText)
    {
        var existing = MainTabs.Items
            .OfType<TabItem>()
            .FirstOrDefault(t => t.Tag is string id && id == tag);
        if (existing != null) { MainTabs.SelectedItem = existing; return; }

        Clipboard.SetText(prompt);

        var textBox = new TextBox
        {
            Text              = prompt,
            FontFamily        = new FontFamily("Consolas"),
            FontSize          = 12,
            AcceptsReturn     = true,
            TextWrapping      = TextWrapping.Wrap,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background        = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E)),
            Foreground        = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
            Padding           = new Thickness(14),
            BorderThickness   = new Thickness(0),
        };

        var copyBtn = new Button
        {
            Content = "📋 Copier",
            Padding = new Thickness(12, 6, 12, 6),
            Style   = (Style)FindResource("BtnPrimary"),
        };
        copyBtn.Click += (_, _) => { Clipboard.SetText(textBox.Text); _vm.Log("📋 Prompt copié."); };

        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 6, 10, 6) };
        toolbar.Children.Add(copyBtn);

        var content = new Grid();
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(toolbar, 0);
        Grid.SetRow(textBox, 1);
        content.Children.Add(toolbar);
        content.Children.Add(textBox);

        TabItem tabItem = null!;
        var closeBtn = new Button
        {
            Content = "✕", FontSize = 10, Width = 18, Height = 18,
            Padding = new Thickness(0), Margin = new Thickness(8, 0, 0, 0),
            Background = Brushes.Transparent, BorderThickness = new Thickness(0),
            Cursor = Cursors.Hand, VerticalAlignment = VerticalAlignment.Center,
        };
        closeBtn.Click += (_, _) => MainTabs.Items.Remove(tabItem);

        var header = new StackPanel { Orientation = Orientation.Horizontal };
        header.Children.Add(new TextBlock
        {
            Text = headerText, VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 220, TextTrimming = TextTrimming.CharacterEllipsis,
        });
        header.Children.Add(closeBtn);

        tabItem = new TabItem { Header = header, Content = content, Tag = tag };
        MainTabs.Items.Add(tabItem);
        MainTabs.SelectedItem = tabItem;
    }

    // ── DeepSeek : initialisation ──────────────────────────────────────────
    private void OnDSCoreReady(object? s, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (!e.IsSuccess) { _vm.Log($"❌ DeepSeek WebView2 erreur : {e.InitializationException?.Message}"); return; }

        WebViewDS.CoreWebView2.Navigate("https://chat.deepseek.com");
        WebViewDS.CoreWebView2.NewWindowRequested += (_, args) =>
        {
            args.Handled = true;
            WebViewDS.CoreWebView2.Navigate(args.Uri);
        };
    }

    // ── Bouton Coller DeepSeek : injecte le presse-papier + tente d'activer Search ─
    private async void BtnCollerDeepSeek_Click(object sender, RoutedEventArgs e)
    {
        var text = Clipboard.GetText();
        if (string.IsNullOrEmpty(text)) { _vm.Log("⚠️ Presse-papier vide."); return; }

        var escaped = System.Text.Json.JsonSerializer.Serialize(text);

        // Tente d'activer le mode Search de DeepSeek (bouton toggle dans la toolbar)
        await WebViewDS.ExecuteScriptAsync("""
            (function() {
                var searchBtn = document.querySelector('[aria-label*="earch" i]')
                             || document.querySelector('[data-testid*="search" i]')
                             || Array.from(document.querySelectorAll('button, div[role="button"]'))
                                    .find(el => el.textContent.trim().toLowerCase() === 'search');
                if (searchBtn && searchBtn.getAttribute('aria-pressed') !== 'true')
                    searchBtn.click();
            })()
            """);

        var script = $$"""
            (function() {
                var text = {{escaped}};
                var el = document.querySelector('textarea')
                      || document.querySelector('[contenteditable="true"]');
                if (!el) return 'NOT_FOUND';
                el.focus();
                document.execCommand('selectAll');
                document.execCommand('insertText', false, text);
                return 'OK';
            })()
            """;

        var result = await WebViewDS.ExecuteScriptAsync(script);
        if (result == "\"OK\"")
            _vm.Log("📋 Prompt collé dans DeepSeek — vérifie que 🔍 Search est activé avant d'envoyer.");
        else
            _vm.Log("⚠️ Zone de texte DeepSeek introuvable — colle manuellement (Ctrl+V).");
    }

}

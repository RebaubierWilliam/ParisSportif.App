using Microsoft.Web.WebView2.Core;
using ParisSportif.Models;
using ParisSportif.Services;
using ParisSportif.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ParisSportif;

public partial class MainWindow : Window
{
    private readonly MainViewModel     _vm;
    private readonly ExtractionService _extractionPS;

    public MainWindow()
    {
        InitializeComponent();

        _extractionPS = new ExtractionService(WebViewPS);
        _vm = new MainViewModel();
        DataContext = _vm;

        _extractionPS.ParisExtracted += OnParisExtracted;
        _extractionPS.HtmlDumped     += OnHtmlDumped;
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

    // ── Bouton Debug HTML ────────────────────────────────────────────────
    private async void BtnDebugHtml_Click(object sender, RoutedEventArgs e)
        => await _extractionPS.InjectHtmlDumpAsync();

    private void OnHtmlDumped(string html)
    {
        Dispatcher.Invoke(() =>
        {
            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                $"parionssport_debug_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            File.WriteAllText(path, html);
            _vm.Log($"🐛 HTML sauvegardé : {path}");
        });
    }

    // ── Bouton Prompt Complet : prompt autonome avec protocole intégré ────────
    private void BtnPromptComplet_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        OpenPromptTab(pari, PromptBuilder.BuildFull(pari), pari.NumeroPari + "_full", $"📄 {pari.MatchLabel}");
        _vm.Log($"📄 Prompt complet généré pour {pari.MatchLabel} — copié dans le presse-papier");
    }

    // ── Bouton Prompt Agent : prompt 2 phases collecte + analyse ─────────────
    private void BtnPromptAgent_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        OpenPromptTab(pari, PromptBuilder.BuildAgent(pari), pari.NumeroPari + "_agent", $"🔬 {pari.MatchLabel}");
        _vm.Log($"🔬 Prompt agent généré pour {pari.MatchLabel} — copié dans le presse-papier");
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

    // ── Bouton Tips Scanner : scanne le web pour collecter tous les tips ────
    private async void BtnTipsScannerDeepSeek_Click(object sender, RoutedEventArgs e)
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", "tips_scanner.md");

        if (!File.Exists(path))
        {
            _vm.Log("⚠️ Fichier tips_scanner.md introuvable.");
            return;
        }

        var prompt  = File.ReadAllText(path, System.Text.Encoding.UTF8);
        var escaped = System.Text.Json.JsonSerializer.Serialize(prompt);

        Clipboard.SetText(prompt);

        // Tente d'activer le mode Search de DeepSeek
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
            _vm.Log("🌐 Tips Scanner injecté dans DeepSeek — vérifie que 🔍 Search est activé avant d'envoyer.");
        else
            _vm.Log("🌐 Tips Scanner copié — colle manuellement (Ctrl+V) dans DeepSeek.");
    }

    // ── Bouton Tips Continue : relance la recherche et reconstruit le tableau ─
    private async void BtnTipsContinueDeepSeek_Click(object sender, RoutedEventArgs e)
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources", "Prompts", "tips_continue.md");

        if (!File.Exists(path))
        {
            _vm.Log("⚠️ Fichier tips_continue.md introuvable.");
            return;
        }

        var prompt  = File.ReadAllText(path, System.Text.Encoding.UTF8);
        var escaped = System.Text.Json.JsonSerializer.Serialize(prompt);

        Clipboard.SetText(prompt);

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
            _vm.Log("🔄 Tips Continue injecté — DeepSeek va chercher plus de tips et reconstruire le tableau complet.");
        else
            _vm.Log("🔄 Tips Continue copié — colle manuellement (Ctrl+V) dans DeepSeek.");
    }

    // ── Bouton "Chercher d'autres sources + relancer phase 2" ──────────────
    private async void BtnAutresSourcesDeepSeek_Click(object sender, RoutedEventArgs e)
    {
        const string prompt = "Essaye de chercher d'autres sources pour augmenter la confiance et refais la phase 2";
        var escaped = System.Text.Json.JsonSerializer.Serialize(prompt);

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
            _vm.Log("🔄 Prompt 'autres sources + phase 2' injecté dans DeepSeek.");
        else
            _vm.Log("⚠️ Zone de texte DeepSeek introuvable — colle manuellement (Ctrl+V).");
    }

}

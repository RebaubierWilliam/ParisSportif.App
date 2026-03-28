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
    private string _mamouthPhase1 = "";
    private string _mamouthPhase2 = "";
    private bool   _mamouthInitStarted;

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
        MainTabs.SelectionChanged += OnMainTabsSelectionChanged;
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

    // ── Mamouth : init au premier affichage de l'onglet ─────────────────────
    private async void OnMainTabsSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (MainTabs.SelectedItem != TabMamouth) return;
        if (_mamouthInitStarted) return;
        _mamouthInitStarted = true;

        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var mmFolder = Path.Combine(appData, "ParisSportif", "WebView2Mamouth");
            Directory.CreateDirectory(mmFolder);
            var envMM = await CoreWebView2Environment.CreateAsync(userDataFolder: mmFolder);
            await WebViewMM.EnsureCoreWebView2Async(envMM);

            WebViewMM.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
            WebViewMM.CoreWebView2.Settings.IsWebMessageEnabled = true;
            WebViewMM.CoreWebView2.Navigate("https://mammouth.ai/app/a/default");

            // Laisser les popups OAuth (Google, etc.) s'ouvrir normalement
            // Seuls les liens classiques sont redirigés dans le même WebView
            WebViewMM.CoreWebView2.NewWindowRequested += (_, args) =>
            {
                var uri = args.Uri.ToLowerInvariant();
                if (uri.Contains("accounts.google") || uri.Contains("oauth")
                    || uri.Contains("login") || uri.Contains("signin")
                    || uri.Contains("auth"))
                    return; // popup OAuth → laisser ouvrir naturellement

                args.Handled = true;
                WebViewMM.CoreWebView2.Navigate(args.Uri);
            };
            _vm.Log("🦣 Mamouth chargé.");
        }
        catch (Exception ex)
        {
            _mamouthInitStarted = false;
            _vm.Log($"❌ Mamouth WebView2 erreur : {ex.Message}");
        }
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


    // ── Bouton Prompt Mamouth : 2 phases <800 mots chacune ─────────────────
    private void BtnPromptMamouth_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;
        var (phase1, phase2) = PromptBuilder.BuildMamouth(pari);
        _mamouthPhase1 = phase1;
        _mamouthPhase2 = phase2;
        Clipboard.SetText(phase1);
        MamouthMatchLabel.Text = $"🦣 {pari.MatchLabel} — P1: {CountWords(phase1)} mots | P2: {CountWords(phase2)} mots";
        MainTabs.SelectedItem = TabMamouth;
        _vm.Log($"🦣 Phase 1 copiée pour {pari.MatchLabel} — Ctrl+V ou 📋 Phase 1 quand Mamouth est chargé");
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

    // ── Création/activation d'un onglet Mamouth (Phase 1 + Phase 2 séparées) ─
    private void OpenMamouthPromptTab(Pari pari, string phase1, string phase2)
    {
        var tag = pari.NumeroPari + "_mamouth";
        var existing = MainTabs.Items
            .OfType<TabItem>()
            .FirstOrDefault(t => t.Tag is string id && id == tag);
        if (existing != null) { MainTabs.SelectedItem = existing; return; }

        Clipboard.SetText(phase1);

        var textBox = new TextBox
        {
            Text              = phase1,
            FontFamily        = new FontFamily("Consolas"),
            FontSize          = 12,
            AcceptsReturn     = true,
            TextWrapping      = TextWrapping.Wrap,
            IsReadOnly        = true,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Background        = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x2E)),
            Foreground        = new SolidColorBrush(Color.FromRgb(0xCD, 0xD6, 0xF4)),
            Padding           = new Thickness(14),
            BorderThickness   = new Thickness(0),
        };

        var p1Btn = new Button
        {
            Content = "📋 Phase 1 — Collecte",
            Padding = new Thickness(12, 6, 12, 6),
            Style   = (Style)FindResource("BtnPrimary"),
        };
        p1Btn.Click += (_, _) =>
        {
            Clipboard.SetText(phase1);
            textBox.Text = phase1;
            _vm.Log("📋 Phase 1 copiée — colle dans Mamouth et envoie.");
        };

        var p2Btn = new Button
        {
            Content = "📋 Phase 2 — Analyse",
            Padding = new Thickness(12, 6, 12, 6),
            Margin  = new Thickness(8, 0, 0, 0),
            Style   = (Style)FindResource("BtnPrimary"),
        };
        p2Btn.Click += (_, _) =>
        {
            Clipboard.SetText(phase2);
            textBox.Text = phase2;
            _vm.Log("📋 Phase 2 copiée — colle dans Mamouth APRÈS la réponse de la Phase 1.");
        };

        var wordInfo = new TextBlock
        {
            Text = $"P1: {CountWords(phase1)} mots | P2: {CountWords(phase2)} mots",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(12, 0, 0, 0),
            FontSize = 11,
            Foreground = new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
        };

        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 6, 10, 6) };
        toolbar.Children.Add(p1Btn);
        toolbar.Children.Add(p2Btn);
        toolbar.Children.Add(wordInfo);

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
            Text = $"🦣 {pari.MatchLabel}", VerticalAlignment = VerticalAlignment.Center,
            MaxWidth = 220, TextTrimming = TextTrimming.CharacterEllipsis,
        });
        header.Children.Add(closeBtn);

        tabItem = new TabItem { Header = header, Content = content, Tag = tag };
        MainTabs.Items.Add(tabItem);
        MainTabs.SelectedItem = tabItem;
    }

    private static int CountWords(string text) =>
        text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;

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

    // ── Mamouth : injection de texte dans le chat ────────────────────────────
    private async Task InjectTextIntoMamouth(string text)
    {
        if (WebViewMM.CoreWebView2 == null)
        {
            Clipboard.SetText(text);
            _vm.Log("⚠️ Mamouth pas encore chargé — prompt copié, colle manuellement (Ctrl+V).");
            return;
        }

        var escaped = System.Text.Json.JsonSerializer.Serialize(text);

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

        var result = await WebViewMM.ExecuteScriptAsync(script);
        if (result != "\"OK\"")
        {
            Clipboard.SetText(text);
            _vm.Log("⚠️ Zone de texte Mamouth introuvable — prompt copié, colle manuellement (Ctrl+V).");
        }
    }

    // ── Bouton Coller Phase 1 Mamouth ────────────────────────────────────────
    private async void BtnCollerP1Mamouth_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_mamouthPhase1))
        { _vm.Log("⚠️ Aucun prompt Mamouth généré — clique d'abord 🦣 sur un pari."); return; }
        await InjectTextIntoMamouth(_mamouthPhase1);
        _vm.Log("📋 Phase 1 injectée dans Mamouth — envoie puis clique Phase 2.");
    }

    // ── Bouton Coller Phase 2 Mamouth ────────────────────────────────────────
    private async void BtnCollerP2Mamouth_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_mamouthPhase2))
        { _vm.Log("⚠️ Aucun prompt Mamouth généré — clique d'abord 🦣 sur un pari."); return; }
        await InjectTextIntoMamouth(_mamouthPhase2);
        _vm.Log("📋 Phase 2 injectée dans Mamouth — envoie pour obtenir l'analyse.");
    }

    // ── Bouton Consolider Mamouth : améliorer la confiance ──────────────────
    private async void BtnConsoliderMamouth_Click(object sender, RoutedEventArgs e)
    {
        const string prompt = """
            La Phase 1 s'est terminee avec une confiance insuffisante (< 21/25).
            Consolide les donnees : pour chaque [MANQUANT], effectue 3 nouvelles recherches web (sources differentes, langues differentes).
            Remplace chaque [MANQUANT] trouve par la valeur reelle.
            Recalcule le score de confiance (/25) avec les memes criteres et malus.
            Affiche le bilan complet mis a jour avec toutes les sources.
            """;
        await InjectTextIntoMamouth(prompt);
        _vm.Log("🔄 Prompt consolidation injecté dans Mamouth.");
    }

    // ── Bouton "Chercher d'autres sources + relancer phase 2" ──────────────
    private async void BtnAutresSourcesDeepSeek_Click(object sender, RoutedEventArgs e)
    {
        const string prompt = """
            La Phase 1 s'est terminee avec une confiance insuffisante (< 21/25).

            MISSION : Consolider toutes les donnees collectees et atteindre une confiance >= 21/25, puis executer la Phase 2 complete.

            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            ETAPE 1 — AUDIT DES LACUNES
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            Lister TOUTES les donnees marquees [DONNEE MANQUANTE CONFIRMEE] ou avec un score faible, classees par impact decroissant sur la confiance (scoring > classement > forme > H2H > absences > cotes).

            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            ETAPE 2 — RECHERCHE DE NOUVELLES SOURCES (obligatoire)
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            Minimum 10 nouvelles recherches web ciblant les donnees a fort impact.
            Pour chaque donnee manquante, dans cet ordre strict :
              1. Sources NON encore consultees (elargir au-dela des sources habituelles)
              2. Changer de langue : anglais ET langue locale du pays
              3. Comptes officiels X/Twitter des clubs, federations, journalistes specialises
              4. Google Actualites avec termes varies ("[equipe] stats", "[equipe] resultats", "[equipe] news")
              5. Calcul manuel a partir des donnees brutes si disponibles
            INTERDICTION d'inventer ou d'estimer sans source verifiable.

            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            ETAPE 3 — CONSOLIDATION COMPLETE
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            Mettre a jour TOUS les tableaux Phase 1 avec les nouvelles donnees trouvees.
            Remplacer chaque [DONNEE MANQUANTE CONFIRMEE] par la valeur reelle si trouvee.
            Recalculer le score de confiance (/25) selon les memes criteres et malus.

            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            ETAPE 4 — REEVALUATION ET DECISION
            ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
            Afficher le bilan Phase 1 consolide complet (tous tableaux mis a jour + sources).
            Afficher le nouveau score de confiance detaille (criteres + malus).

            SI Confiance_nouvelle >= 21/25 :
              → ✅ CONFIANCE ATTEINTE — executer la Phase 2 complete immediatement.

            SI Confiance_nouvelle < 21/25 :
              → ⚠️ Indiquer le score atteint, les donnees encore manquantes et leur impact residuel.
              → Executer quand meme la Phase 2 en signalant clairement chaque limitation.
            """;

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

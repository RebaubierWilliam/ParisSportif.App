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
    private OddsApiService? _oddsApi;

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

    // ── Bouton Prompt : crée un onglet Analyse dédié par match ─────────────
    private void BtnPrompt_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not Pari pari) return;
        _vm.SelectedPari = pari;

        // Si l'onglet existe déjà pour ce match, l'activer
        var existing = MainTabs.Items
            .OfType<TabItem>()
            .FirstOrDefault(t => t.Tag is string id && id == pari.NumeroPari);
        if (existing != null) { MainTabs.SelectedItem = existing; return; }

        // Générer le prompt
        var prompt = PromptBuilder.Build(pari, null);
        Clipboard.SetText(prompt);

        // ── Contenu de l'onglet ───────────────────────────────────────────
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

        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin      = new Thickness(10, 6, 10, 6),
        };
        toolbar.Children.Add(copyBtn);

        var content = new Grid();
        content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        Grid.SetRow(toolbar, 0);
        Grid.SetRow(textBox, 1);
        content.Children.Add(toolbar);
        content.Children.Add(textBox);

        // ── En-tête avec bouton ✕ ─────────────────────────────────────────
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
            Text              = $"📋 {pari.MatchLabel}",
            VerticalAlignment = VerticalAlignment.Center,
            MaxWidth          = 220,
            TextTrimming      = TextTrimming.CharacterEllipsis,
        });
        header.Children.Add(closeBtn);

        tabItem = new TabItem { Header = header, Content = content, Tag = pari.NumeroPari };
        MainTabs.Items.Add(tabItem);
        MainTabs.SelectedItem = tabItem;

        _vm.Log($"📋 Prompt généré pour {pari.MatchLabel} — copié dans le presse-papier");
    }

    // ── Scan Odds API : récupère les cotes réelles et compare ──────────────
    private async void BtnScanOddsApi_Click(object sender, RoutedEventArgs e)
    {
        var paris = (_parisView?.Cast<Pari>() ?? _vm.Paris).ToList();
        if (paris.Count == 0) { _vm.Log("⚠️ Aucun pari à analyser."); return; }

        // Demander la clé API si pas encore saisie
        if (string.IsNullOrWhiteSpace(_vm.OddsApiKey))
        {
            var dialog = new Window
            {
                Title = "Clé API The Odds API",
                Width = 450, Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                ResizeMode = ResizeMode.NoResize,
            };
            var sp = new StackPanel { Margin = new Thickness(16) };
            sp.Children.Add(new TextBlock { Text = "Entre ta clé API (the-odds-api.com) :", Margin = new Thickness(0, 0, 0, 8) });
            var tb = new TextBox { FontFamily = new FontFamily("Consolas") };
            sp.Children.Add(tb);
            var ok = new Button { Content = "OK", Width = 80, Margin = new Thickness(0, 12, 0, 0), HorizontalAlignment = HorizontalAlignment.Right };
            ok.Click += (_, _) => { dialog.DialogResult = true; };
            sp.Children.Add(ok);
            dialog.Content = sp;

            if (dialog.ShowDialog() != true || string.IsNullOrWhiteSpace(tb.Text))
            { _vm.Log("⚠️ Clé API requise pour le scan."); return; }

            _vm.OddsApiKey = tb.Text.Trim();
        }

        // Initialiser le service si nécessaire
        _oddsApi ??= new OddsApiService(_vm.OddsApiKey);
        _oddsApi.LogMessage -= _vm.Log;
        _oddsApi.LogMessage += _vm.Log;

        _vm.Log($"📊 Scan Odds API en cours pour {paris.Count} paris...");

        List<MatchOddsResult> results;
        try
        {
            results = await Task.Run(() => _oddsApi.ScanValueAsync(paris));
        }
        catch (Exception ex)
        {
            _vm.Log($"❌ Erreur Odds API : {ex.Message}");
            return;
        }

        if (_oddsApi.RequestsRemaining.HasValue)
            _vm.OddsQuotaLabel = $"Quota: {_oddsApi.RequestsRemaining}";

        var valueBets = results.Where(r => r.DiffPercent > 1).ToList();
        var text = BuildOddsDisplayText(results, _oddsApi.RequestsRemaining);

        // Créer/mettre à jour l'onglet résultats
        const string oddsTag = "__ODDS_API__";
        var existing = MainTabs.Items
            .OfType<TabItem>()
            .FirstOrDefault(t => t.Tag is string id && id == oddsTag);
        if (existing != null)
        {
            if (existing.Content is Grid g &&
                g.Children.OfType<TextBox>().FirstOrDefault() is TextBox existingTb)
                existingTb.Text = text;
            MainTabs.SelectedItem = existing;
            _vm.Log($"📊 Scan Odds API mis à jour — {valueBets.Count} value bet(s) détecté(s).");
            return;
        }

        var textBox = new TextBox
        {
            Text              = text,
            FontFamily        = new FontFamily("Consolas"),
            FontSize          = 12,
            AcceptsReturn     = true,
            IsReadOnly        = true,
            TextWrapping      = TextWrapping.NoWrap,
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
        copyBtn.Click += (_, _) => { Clipboard.SetText(textBox.Text); _vm.Log("📋 Résultats Odds API copiés."); };

        var refreshBtn = new Button
        {
            Content = "🔄 Rafraîchir",
            Padding = new Thickness(12, 6, 12, 6),
            Margin  = new Thickness(8, 0, 0, 0),
            Style   = (Style)FindResource("BtnSecondary"),
        };
        refreshBtn.Click += (_, _) => BtnScanOddsApi_Click(sender, e);

        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 6, 10, 6) };
        toolbar.Children.Add(copyBtn);
        toolbar.Children.Add(refreshBtn);

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
            Text              = $"📊 Odds API ({results.Count})",
            VerticalAlignment = VerticalAlignment.Center,
        });
        header.Children.Add(closeBtn);

        tabItem = new TabItem { Header = header, Content = content, Tag = oddsTag };
        MainTabs.Items.Add(tabItem);
        MainTabs.SelectedItem = tabItem;

        _vm.Log($"📊 Scan Odds API terminé — {valueBets.Count} value bet(s) sur {results.Count} paris. Quota : {_oddsApi.RequestsRemaining?.ToString() ?? "?"}");
    }

    // ── Formatage du résultat Odds API ──────────────────────────────────────
    private static string BuildOddsDisplayText(List<MatchOddsResult> results, int? quota)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"📊 SCAN ODDS API — {results.Count} paris analysés — {DateTime.Now:dd/MM/yyyy HH:mm}");
        sb.AppendLine($"   Quota restant : {quota?.ToString() ?? "?"}");
        sb.AppendLine();

        int idx = 1;
        foreach (var r in results)
        {
            // En-tête du match
            var diffStr = r.DiffPercent != 0 ? $"{(r.DiffPercent > 0 ? "+" : "")}{r.DiffPercent:F1}%" : "";
            sb.AppendLine(new string('═', 90));
            sb.AppendLine($"#{idx}  {r.MatchLabel}  |  Mon pari: {r.Selection} @ {r.CotePS:F2} PS  |  {r.Verdict} {diffStr}");
            sb.AppendLine(new string('═', 90));

            if (r.BookmakerNames.Count == 0 || r.OutcomeNames.Count == 0)
            {
                sb.AppendLine($"   → {r.Verdict}");
                sb.AppendLine();
                idx++;
                continue;
            }

            const int colW = 10;
            const int labelW = 14;

            // Ligne d'en-tête : bookmakers
            sb.Append("".PadRight(labelW));
            sb.Append("│");
            sb.Append(" PS".PadRight(colW));
            foreach (var bk in r.BookmakerNames)
                sb.Append("│").Append($" {Trunc(bk, colW - 2)}".PadRight(colW));
            sb.AppendLine("│");

            // Séparateur
            sb.Append(new string('─', labelW));
            sb.Append("┼");
            sb.Append(new string('─', colW));
            for (int b = 0; b < r.BookmakerNames.Count; b++)
                sb.Append("┼").Append(new string('─', colW));
            sb.AppendLine("┤");

            // Une ligne par outcome (cotes + diff)
            bool hasAnyCotePS = r.CotesPSParOutcome.Any(c => c > 0);
            for (int o = 0; o < r.OutcomeNames.Count; o++)
            {
                bool isSel = (o == r.SelectedOutcomeIndex);
                var marker = isSel ? "★ " : "  ";
                var label = $"{marker}{Trunc(r.OutcomeNames[o], labelW - 3)}";
                sb.Append(label.PadRight(labelW));

                // Colonne PS : afficher si on a une cote PS pour cet outcome
                var psPrice = o < r.CotesPSParOutcome.Length ? r.CotesPSParOutcome[o] : 0;
                sb.Append("│");
                sb.Append(psPrice > 0 ? $" {psPrice,6:F2}  ".PadRight(colW) : "   -     ".PadRight(colW));

                // Colonnes bookmakers
                for (int b = 0; b < r.BookmakerNames.Count; b++)
                {
                    var price = r.CotesGrid[b][o];
                    sb.Append("│");
                    sb.Append(price > 0 ? $" {price,6:F2}  ".PadRight(colW) : "   -     ".PadRight(colW));
                }
                sb.AppendLine("│");

                // Ligne de diff vs PS (pour chaque outcome qui a une cote PS)
                if (psPrice > 0)
                {
                    sb.Append("  diff vs PS".PadRight(labelW));
                    sb.Append("│");
                    sb.Append("".PadRight(colW)); // colonne PS = vide
                    for (int b = 0; b < r.BookmakerNames.Count; b++)
                    {
                        var price = r.CotesGrid[b][o];
                        sb.Append("│");
                        if (price > 0)
                        {
                            var diff = psPrice - price;
                            var sign = diff >= 0 ? "+" : "";
                            sb.Append($" {sign}{diff:F2}   ".PadRight(colW));
                        }
                        else
                            sb.Append("".PadRight(colW));
                    }
                    sb.AppendLine("│");
                }
            }

            // Séparateur bas
            sb.Append(new string('─', labelW));
            sb.Append("┴");
            sb.Append(new string('─', colW));
            for (int b = 0; b < r.BookmakerNames.Count; b++)
                sb.Append("┴").Append(new string('─', colW));
            sb.AppendLine("┘");

            // Best cote pour la sélection
            if (r.BestCoteSelection.HasValue && r.SelectedOutcomeIndex >= 0)
                sb.AppendLine($"   Best marché: {r.BestCoteSelection:F2} ({r.BestBookSelection})  |  PS: {r.CotePS:F2}  |  Diff: {(r.DiffPercent > 0 ? "+" : "")}{r.DiffPercent:F1}%");

            sb.AppendLine();
            idx++;
        }

        // Récap trié par value
        var ranked = results.Where(r => r.SelectedOutcomeIndex >= 0).OrderByDescending(r => r.DiffPercent).ToList();
        if (ranked.Count > 0)
        {
            sb.AppendLine(new string('═', 90));
            sb.AppendLine("RÉCAPITULATIF — trié par value décroissante");
            sb.AppendLine(new string('═', 90));
            sb.AppendLine();

            int ri = 1;
            foreach (var r in ranked)
            {
                var diffStr2 = $"{(r.DiffPercent > 0 ? "+" : "")}{r.DiffPercent:F1}%";
                sb.AppendLine($"  {ri,2}. {r.MatchLabel,-30}  {r.Selection,-18}  PS:{r.CotePS:F2}  Best:{r.BestCoteSelection?.ToString("F2") ?? "N/D"}  {diffStr2,7}  {r.Verdict}");
                ri++;
            }
        }

        return sb.ToString();
    }

    private static string Trunc(string s, int max) =>
        s.Length <= max ? s : s[..max];

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

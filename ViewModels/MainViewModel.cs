using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ParisSportif.Models;
using ParisSportif.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace ParisSportif.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly HistoriqueService _historique;
    public  readonly FlashScoreService FlashScore;   // accès depuis code-behind

    // ── Collections ────────────────────────────────────────────────────────
    [ObservableProperty]
    private ObservableCollection<Pari> _paris = new ObservableCollection<Pari>();

    [ObservableProperty]
    private Pari? _selectedPari;

    // ── État UI ────────────────────────────────────────────────────────────
    [ObservableProperty]
    private string _statusLog = "Prêt.";

    [ObservableProperty]
    private string _currentPrompt = "";

    [ObservableProperty]
    private int    _tabIndex = 0;      // 0=ParionsSport 1=FlashScore 2=Analyse

    [ObservableProperty]
    private bool   _showLiveOnly = false;

    // ── Compteurs ──────────────────────────────────────────────────────────
    public int TotalParis  => Paris.Count;
    public int LiveCount   => Paris.Count(p => p.IsLive);

    // ── Constructor ────────────────────────────────────────────────────────
    public MainViewModel(HistoriqueService historique, FlashScoreService flashScore)
    {
        _historique = historique;
        FlashScore  = flashScore;
    }

    // ── Mise à jour des paris depuis ExtractionService ─────────────────────
    public void OnParisExtracted(List<Pari> paris)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            Paris.Clear();
            foreach (var p in paris) Paris.Add(p);
            OnPropertyChanged(nameof(TotalParis));
            OnPropertyChanged(nameof(LiveCount));
            Log($"✅ {paris.Count} paris extraits ({paris.Count(p => p.IsLive)} en direct)");
        });
    }

    public void OnFlashScoreStats(CashoutStats stats)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (SelectedPari is null) return;
            SelectedPari.FlashScoreStats = stats;

            CurrentPrompt = PromptBuilder.BuildCashoutPrompt(SelectedPari, stats);
            Clipboard.SetText(CurrentPrompt);
            TabIndex = 2;   // bascule sur l'onglet Analyse
            Log($"✅ Stats FlashScore reçues ({stats.Rows.Count} ligne(s)) — prompt copié");
        });
    }

    // ── Commandes ──────────────────────────────────────────────────────────

    /// Sauvegarde tous les paris extraits en SQLite
    [RelayCommand]
    private void Sauvegarder()
    {
        if (!Paris.Any()) { Log("⚠️ Aucun pari à sauvegarder."); return; }
        _historique.UpsertParis(Paris);
        Log($"💾 {Paris.Count} paris sauvegardés.");
    }

    /// Charge l'historique SQLite dans la liste
    [RelayCommand]
    private void ChargerHistorique()
    {
        var hist = _historique.GetHistorique();
        Paris.Clear();
        foreach (var p in hist) Paris.Add(p);
        OnPropertyChanged(nameof(TotalParis));
        OnPropertyChanged(nameof(LiveCount));
        Log($"📂 {hist.Count} paris chargés depuis l'historique.");
    }

    /// Copie le prompt dans le presse-papier
    [RelayCommand]
    private void CopierPrompt()
    {
        if (string.IsNullOrEmpty(CurrentPrompt)) return;
        Clipboard.SetText(CurrentPrompt);
        Log("📋 Prompt copié dans le presse-papier.");
    }

    /// Génère le prompt cashout pour le pari sélectionné (sans stats FlashScore)
    [RelayCommand]
    private void GenererPromptCashout()
    {
        if (SelectedPari is null) { Log("⚠️ Sélectionne un pari d'abord."); return; }
        CurrentPrompt = PromptBuilder.BuildCashoutPrompt(SelectedPari, SelectedPari.FlashScoreStats);
        Clipboard.SetText(CurrentPrompt);
        TabIndex = 2;
        Log($"📋 Prompt cashout généré pour {SelectedPari.MatchLabel}");
    }

    /// Ouvre FlashScore pour le pari sélectionné et prépare l'injection automatique
    [RelayCommand]
    private void LancerFlashScore()
    {
        if (SelectedPari is null) { Log("⚠️ Sélectionne un pari d'abord."); return; }
        FlashScore.SetPendingPari(SelectedPari);
        FlashScore.NavigateToMatch(SelectedPari);
        TabIndex = 1;   // bascule sur l'onglet FlashScore
        Log($"🔍 Recherche FlashScore : {SelectedPari.MatchLabel}");
    }

    /// Filtrage live only
    partial void OnShowLiveOnlyChanged(bool value)
    {
        // Déclenche un re-filter via CollectionView dans le code-behind si besoin
        OnPropertyChanged(nameof(Paris));
    }

    public void Log(string msg)
    {
        Application.Current.Dispatcher.Invoke(() =>
            StatusLog = $"[{DateTime.Now:HH:mm:ss}] {msg}");
    }
}

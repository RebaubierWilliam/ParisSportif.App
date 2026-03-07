using CommunityToolkit.Mvvm.ComponentModel;
using ParisSportif.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace ParisSportif.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // ── Collections ────────────────────────────────────────────────────────
    [ObservableProperty]
    private ObservableCollection<Pari> _paris = new();

    [ObservableProperty]
    private Pari? _selectedPari;

    // ── État UI ────────────────────────────────────────────────────────────
    [ObservableProperty]
    private string _statusLog = "Prêt.";

    [ObservableProperty]
    private bool _showLiveOnly = false;

    // ── Odds API ─────────────────────────────────────────────────────────
    [ObservableProperty]
    private string _oddsApiKey = "";

    [ObservableProperty]
    private string _oddsQuotaLabel = "";

    // ── Compteurs ──────────────────────────────────────────────────────────
    public int TotalParis => Paris.Count;
    public int LiveCount  => Paris.Count(p => p.IsLive);

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

    partial void OnShowLiveOnlyChanged(bool value) => OnPropertyChanged(nameof(Paris));

    public void Log(string msg)
    {
        Application.Current.Dispatcher.Invoke(() =>
            StatusLog = $"[{DateTime.Now:HH:mm:ss}] {msg}");
    }
}

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using LoLTracker.Properties;
using ReactiveUI;

namespace LoLTracker.ViewModels;

public partial class MainViewModel : ReactiveObject
{
    // HACK:
    private static readonly Bitmap AllyIcon = Bitmap.DecodeToWidth(AssetLoader.OpenAndGetAssembly(new Uri("avares://LoLTracker/Assets/ally.jpg")).stream, 512);
    private static readonly Bitmap EnemyIcon = Bitmap.DecodeToWidth(AssetLoader.OpenAndGetAssembly(new Uri("avares://LoLTracker/Assets/enemy.jpg")).stream, 512);

    private string riotId;

    [RegularExpression(@"^.*#[a-zA-Z0-9]{2,5}$", ErrorMessageResourceName = nameof(Resources.UseCorrectRiotId), ErrorMessageResourceType = typeof(Resources))]
    public string RiotId
    {
        get => riotId;
        set => this.RaiseAndSetIfChanged(ref riotId, value);
    }

    private double winProbability;
    public double WinProbability
    {
        get => winProbability;
        set => this.RaiseAndSetIfChanged(ref winProbability, value);
    }

    private readonly ObservableAsPropertyHelper<string> winProbabilityColor;
    public string WinProbabilityColor => winProbabilityColor.Value;

    public ObservableCollection<PlayerViewModel> AllyTeam { get; set; }
    public ObservableCollection<PlayerViewModel> EnemyTeam { get; set; }

    private string allyTeamIcon;
    public string AllyTeamIcon
    {
        get => allyTeamIcon;
        set => this.RaiseAndSetIfChanged(ref allyTeamIcon, value);
    }

    private string enemyTeamIcon;
    public string EnemyTeamIcon
    {
        get => enemyTeamIcon;
        set => this.RaiseAndSetIfChanged(ref enemyTeamIcon, value);
    }

    private bool isStatisticsLoaded;
    public bool IsStatisticsLoaded
    {
        get => isStatisticsLoaded;
        set => this.RaiseAndSetIfChanged(ref isStatisticsLoaded, value);
    }

    private bool isLoading;
    public bool IsLoading
    {
        get => isLoading;
        set => this.RaiseAndSetIfChanged(ref isLoading, value);
    }

    public ICommand UpdateCommand { get; }

    public MainViewModel()
    {
        winProbabilityColor = this.WhenAnyValue(x => x.WinProbability)
            .Select(x => x switch
            {
                < 0.3 => "Red",
                < 0.5 => "Orange",
                < 0.7 => "Yellow",
                _ => "Green"
            })
            .ToProperty(this, x => x.WinProbabilityColor);

        AllyTeam = new ObservableCollection<PlayerViewModel>();
        EnemyTeam = new ObservableCollection<PlayerViewModel>();

        var canExecute = this.WhenAnyValue(
            x => x.RiotId, x => x.IsLoading,
            (riotId, isLoading) => IsRiotIdValid(riotId) && !isLoading);

        UpdateCommand = ReactiveCommand.CreateFromTask(UpdateDataAsync, canExecute);
        IsStatisticsLoaded = false;
        IsLoading = false;
    }

    private bool IsRiotIdValid(string riotId)
    {
        // Проверка на null или пустую строку
        if (string.IsNullOrWhiteSpace(riotId))
            return false;

        // Регулярное выражение для проверки формата
        var regex = RiotIdValidationRegex();
        return regex.IsMatch(riotId);
    }

    private async Task UpdateDataAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            IsLoading = true;
            // Sample logic for the data update
            try
            {
                await Task.Delay(1000);

                WinProbability = GetWinProbability(RiotId);
                AllyTeamIcon = "avares://LoLTracker/Assets/ally_team_icon.png";
                EnemyTeamIcon = "avares://LoLTracker/Assets/enemy_team_icon.png";

                AllyTeam.Clear();
                EnemyTeam.Clear();
                for (int i = 0; i < 5; i++)
                {
                    AllyTeam.Add(new PlayerViewModel { Nickname = $"Ally{i+1}", ChampionIcon = AllyIcon, Efficiency = $"{Random.Shared.NextDouble(),1:F2}" });
                }
                for (int i = 0; i < 5; i++)
                {
                    EnemyTeam.Add(new PlayerViewModel { Nickname = $"Enemy{i+1}", ChampionIcon = EnemyIcon, Efficiency = $"{Random.Shared.NextDouble(),1:F2}" });
                }

                IsStatisticsLoaded = true;
            }
            finally
            {
                IsLoading = false;
            }
        });
    }

    private double GetWinProbability(string riotId)
    {
        return 0.75;
    }

    [GeneratedRegex(@"^.*#[a-zA-Z0-9]{2,5}$")]
    private static partial Regex RiotIdValidationRegex();
}
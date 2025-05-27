using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Diagnostics;
using LoLTracker.Properties;
using LoLTracker.Services;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using ReactiveUI;
using Splat;

namespace LoLTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // HACK:
    private static readonly Bitmap AllyIcon = Bitmap.DecodeToWidth(AssetLoader.OpenAndGetAssembly(new Uri("avares://LoLTracker/Assets/ally.jpg")).stream, 512);
    private static readonly Bitmap EnemyIcon = Bitmap.DecodeToWidth(AssetLoader.OpenAndGetAssembly(new Uri("avares://LoLTracker/Assets/enemy.jpg")).stream, 512);

    private string riotId = string.Empty;
    private readonly StatisticsService stats;

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

    private readonly ObservableAsPropertyHelper<double> loseProbability;
    public double LoseProbability => loseProbability.Value;

    public ObservableCollection<PlayerViewModel> AllyTeam { get; set; }
    public ObservableCollection<PlayerViewModel> EnemyTeam { get; set; }

    private double allyTeamEfficiency;
    public double AllyTeamEfficiency
    {
        get => allyTeamEfficiency;
        set => this.RaiseAndSetIfChanged(ref allyTeamEfficiency, value);
    }

    private double enemyTeamEfficiency;
    public double EnemyTeamEfficiency
    {
        get => enemyTeamEfficiency;
        set => this.RaiseAndSetIfChanged(ref enemyTeamEfficiency, value);
    }

    private string allyTeamIcon;
    public string AllyTeamIcon
    {
        get => allyTeamIcon;
        [MemberNotNull(nameof(allyTeamIcon))] set => this.RaiseAndSetIfChanged(ref allyTeamIcon!, value);
    }

    private string enemyTeamIcon;
    public string EnemyTeamIcon
    {
        get => enemyTeamIcon;
        [MemberNotNull(nameof(enemyTeamIcon))] set => this.RaiseAndSetIfChanged(ref enemyTeamIcon!, value);
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
        stats = Locator.Current.GetService<StatisticsService>() ?? ThrowHelper.ThrowInvalidOperationException<StatisticsService>();
        loseProbability = this.WhenAnyValue(x => x.WinProbability)
                              .Select(x => 1 - x)
                              .ToProperty(this, x => x.LoseProbability);

        AllyTeam = [];
        EnemyTeam = [];

        var canExecute = this.WhenAnyValue(
            x => x.RiotId, x => x.IsLoading,
            (riotId, isLoading) => IsRiotIdValid(riotId) && !isLoading);

        UpdateCommand = ReactiveCommand.CreateFromTask(UpdateDataAsync, canExecute);
        IsStatisticsLoaded = false;
        IsLoading = false;
        AllyTeamIcon = "avares://LoLTracker/Assets/ally_team_icon.png";
        EnemyTeamIcon = "avares://LoLTracker/Assets/enemy_team_icon.png";
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
                var result = await stats.CalculateStats(RiotId);
                //await Task.Delay(1000);
                WinProbability = result.WinProbability;

                AllyTeamEfficiency = result.AllyTeam.TotalEfficiency;
                EnemyTeamEfficiency = result.EnemyTeam.TotalEfficiency;

                AllyTeam.Clear();
                foreach (var player in result.AllyTeam.Players)
                {
                    var vm = new PlayerViewModel
                    {
                        Nickname = player.PlayerName,
                        ChampionName = player.ChampionName,
                        ChampionIcon = AllyIcon,
                        Efficiency = $"{player.Efficiency,1:F2}"
                    };
                    AllyTeam.Add(vm);
                }
                EnemyTeam.Clear();
                foreach (var player in result.EnemyTeam.Players)
                {
                    var vm = new PlayerViewModel
                    {
                        Nickname = player.PlayerName,
                        ChampionName = player.ChampionName,
                        ChampionIcon = EnemyIcon,
                        Efficiency = $"{player.Efficiency,1:F2}"
                    };
                    EnemyTeam.Add(vm);
                }

                IsStatisticsLoaded = true;
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(Resources.Error, Resources.ExceptionDetails + ex.Message, ButtonEnum.Ok);
                await box.ShowAsync();
            }
            finally
            {
                IsLoading = false;
            }
        });
    }

    [GeneratedRegex(@"^.*#[a-zA-Z0-9]{2,5}$")]
    private static partial Regex RiotIdValidationRegex();
}
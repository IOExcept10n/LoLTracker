using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Diagnostics;
using LoLTracker.Properties;
using LoLTracker.Services;
using ReactiveUI;
using Splat;

namespace LoLTracker.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private string riotId = string.Empty;
    private readonly StatisticsService stats;
    private readonly IconsService icons;

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
        icons = Locator.Current.GetService<IconsService>() ?? ThrowHelper.ThrowInvalidOperationException<IconsService>();
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
                        ChampionIcon = await icons.GetAllyIconAsync(player),
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
                        ChampionIcon = await icons.GetEnemyIconAsync(player),
                        Efficiency = $"{player.Efficiency,1:F2}"
                    };
                    EnemyTeam.Add(vm);
                }

                IsStatisticsLoaded = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                throw new InvalidOperationException("Couldn't process a request.", ex);
#else
                var box = MsBox.Avalonia.MessageBoxManager.GetMessageBoxStandard(Resources.Error, Resources.ExceptionDetails + ex.Message, MsBox.Avalonia.Enums.ButtonEnum.Ok);
                await box.ShowAsync();
#endif
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
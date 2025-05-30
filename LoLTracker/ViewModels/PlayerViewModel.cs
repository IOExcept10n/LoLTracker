using Avalonia.Media.Imaging;
using ReactiveUI;

namespace LoLTracker.ViewModels;

public class PlayerViewModel : ViewModelBase
{
    private Bitmap? championIcon;
    private string championName = string.Empty;
    private string efficiency = string.Empty;
    private string nickname = string.Empty;

    public Bitmap? ChampionIcon
    {
        get => championIcon;
        set => this.RaiseAndSetIfChanged(ref championIcon, value);
    }

    public string ChampionName
    {
        get => championName;
        set => this.RaiseAndSetIfChanged(ref championName, value);
    }

    public string Efficiency
    {
        get => efficiency;
        set => this.RaiseAndSetIfChanged(ref efficiency, value);
    }

    public string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }
}
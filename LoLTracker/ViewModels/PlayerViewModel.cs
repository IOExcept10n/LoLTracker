using Avalonia.Media.Imaging;
using ReactiveUI;

namespace LoLTracker.ViewModels;

public class PlayerViewModel : ViewModelBase
{
    private string nickname;
    public string Nickname
    {
        get => nickname;
        set => this.RaiseAndSetIfChanged(ref nickname, value);
    }

    private string championName;
    public string ChampionName
    {
        get => championName;
        set => this.RaiseAndSetIfChanged(ref championName, value);
    }

    private Bitmap championIcon;
    public Bitmap ChampionIcon
    {
        get => championIcon;
        set => this.RaiseAndSetIfChanged(ref championIcon, value);
    }

    private string efficiency;
    public string Efficiency
    {
        get => efficiency;
        set => this.RaiseAndSetIfChanged(ref efficiency, value);
    }
}
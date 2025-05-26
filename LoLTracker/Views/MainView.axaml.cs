using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Diagnostics;
using DynamicData.Binding;
using LoLTracker.Services;
using Splat;

namespace LoLTracker.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }
}

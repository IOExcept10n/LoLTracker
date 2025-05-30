using Avalonia.Controls;
using Avalonia.Input;
using LoLTracker.ViewModels;

namespace LoLTracker.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void OnRiotInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && DataContext is MainViewModel viewModel && viewModel.UpdateCommand.CanExecute(null))
        {
            viewModel.UpdateCommand.Execute(null);
        }
    }
}
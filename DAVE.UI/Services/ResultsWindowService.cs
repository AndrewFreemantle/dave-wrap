using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using DAVE.ViewModels;
using DAVE.Views;

namespace DAVE.Services;

/// <summary>
/// Abstracts opening the results window so view models can trigger it
/// without taking a direct dependency on Avalonia's Window/View types.
/// </summary>
public interface IResultsWindowService
{
    void ShowResults(ResultsWindowViewModel viewModel);
}

public class ResultsWindowService : IResultsWindowService
{
    public void ShowResults(ResultsWindowViewModel viewModel)
    {
        var window = new ResultsWindow { DataContext = viewModel };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } owner })
            window.Show(owner);
        else
            window.Show();
    }
}

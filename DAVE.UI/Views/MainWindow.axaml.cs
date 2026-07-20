using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using DAVE.ViewModels;

namespace DAVE.Views;

public partial class MainWindow : Window
{

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    private MainWindowViewModel ViewModel => ((MainWindowViewModel)DataContext)!;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnDragEnter(object? sender, DragEventArgs e) { }
    private void OnDragLeave(object? sender, DragEventArgs e) { }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        // Check if we can accept the data
        if (e.DataTransfer.Formats.Contains(DataFormat.File))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (!e.DataTransfer.Formats.Contains(DataFormat.File)) return;

        var files = e.DataTransfer.TryGetFiles();
        if (files != null)
        {
            foreach (var file in files)
            {
                // Process each dropped file
                ViewModel.HandleFile(file);
            }
        }
    }
}

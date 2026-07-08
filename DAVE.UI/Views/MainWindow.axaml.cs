using System;
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
        Console.WriteLine(@"Files dropped...");

        if (e.DataTransfer.Formats.Contains(DataFormat.File))
        {
            var files = e.DataTransfer.TryGetFiles();
            if (files != null)
            {
                foreach (var file in files)
                {
                    // Process each dropped file
                    Console.WriteLine($@"Dropped: {file.Name} - ");
                    ViewModel.HandleFile(file);
                }
            }
        }
    }
}

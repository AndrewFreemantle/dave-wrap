using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using DAVE.ViewModels;

namespace DAVE.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnDragEnter(object? sender, DragEventArgs e){}
    private void OnDragLeave(object? sender, DragEventArgs e){}

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

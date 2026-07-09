using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DAVE.ViewModels;

namespace DAVE.Views;

public partial class ResultsWindow : Window
{
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
    private ResultsWindowViewModel ViewModel => ((ResultsWindowViewModel)DataContext)!;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

    public ResultsWindow()
    {
        InitializeComponent();
        Opened += OnOpened;
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        Console.WriteLine(@"Results - OnOpened()");
        ViewModel.Verify();
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

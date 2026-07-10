using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using DAVE.Models;
using DAVE.Services;
using DAVE.Views;

namespace DAVE.ViewModels;

public partial class MainWindowViewModel(IResultsWindowService resultsWindowService) : ViewModelBase
{
    /// <summary>
    /// Parameterless constructor for the XAML previewer / design-time DataContext.
    /// </summary>
    public MainWindowViewModel() : this(new ResultsWindowService()) { }

    public string Status
    {
        get
        {
            if (HasCurrent && HasPrevious) return "Current & Previous - ready...";
            if (HasCurrent) return "Current - ready...";
            if (HasPrevious) return "Previous. Awaiting current...";
            if (_filesDropped) return "still awaiting data submission file(s)...";
            return "awaiting data submission file(s)...";
        }
    }

    private bool _filesDropped = false;
    private DataCaptureSpreadsheet? CurrentSheet { get; set; }
    private DataCaptureSpreadsheet? PreviousSheet { get; set; }

    public bool HasCurrent => CurrentSheet is { IsValid: true };
    public bool HasPrevious => PreviousSheet is { IsValid: true };

    public bool CanDAVE => HasCurrent;

    public void AssureAndVerify()
    {
        if (CurrentSheet is null)
            return;

        try
        {
            resultsWindowService.ShowResults(new ResultsWindowViewModel(CurrentSheet, PreviousSheet));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Handles a dropped file; is it a data capture submission?
    /// </summary>
    /// <param name="file"></param>
    public void HandleFile(IStorageItem file)
    {
        try
        {
            Console.WriteLine(@"MVVM: HandleFile - checking file");
            _filesDropped = true;

            var sheet = new DataCaptureSpreadsheet(file);
            if (sheet.IsValid)
                if (sheet.IsCurrent)
                    CurrentSheet = sheet;
                else if (sheet.IsPrevious)
                    PreviousSheet = sheet;

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(CanDAVE));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}

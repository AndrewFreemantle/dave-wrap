using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using DAVE.Models;
using DAVE.Services;

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
    public DataCaptureSpreadsheet? CurrentSheet { get; private set; }
    public DataCaptureSpreadsheet? PreviousSheet { get; private set; }

    public bool HasCurrent => CurrentSheet is { IsValid: true };
    public bool HasPrevious => PreviousSheet is { IsValid: true };

    public bool CanDAVE => HasCurrent;

    public void AnalyseAndVerify()
    {
        resultsWindowService.ShowResults(new ResultsWindowViewModel(CurrentSheet, PreviousSheet));
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

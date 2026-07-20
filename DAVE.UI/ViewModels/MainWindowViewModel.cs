using System;
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
            if (_filesDropped) return "still awaiting data submission file(s)...";
            return "awaiting data submission file(s)...";
        }
    }

    private bool _filesDropped = false;
    private DataCaptureSpreadsheet? CurrentSheet { get; set; }
    private DataCaptureSpreadsheet? PreviousSheet { get; set; }

    public bool HasCurrent => CurrentSheet is { IsValid: true };
    public bool HasPrevious => PreviousSheet is { IsValid: true };
    public bool HasAnySheet => HasCurrent || HasPrevious;

    public string CurrentYear => CurrentSheet is { IsValid: true } sheet ? sheet.GetValue<DateTime>(DataFieldName.SubmissionDate).Year.ToString() : string.Empty;
    public string PreviousYear => PreviousSheet is { IsValid: true } sheet ? sheet.GetValue<DateTime>(DataFieldName.SubmissionDate).Year.ToString() : string.Empty;

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
    /// Clears any loaded submissions, returning the view to its initial state.
    /// </summary>
    public void Reset()
    {
        CurrentSheet = null;
        PreviousSheet = null;
        _filesDropped = false;

        NotifySheetPropertiesChanged();
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

            NotifySheetPropertiesChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    private void NotifySheetPropertiesChanged()
    {
        OnPropertyChanged(nameof(Status));
        OnPropertyChanged(nameof(CanDAVE));
        OnPropertyChanged(nameof(HasCurrent));
        OnPropertyChanged(nameof(HasPrevious));
        OnPropertyChanged(nameof(HasAnySheet));
        OnPropertyChanged(nameof(CurrentYear));
        OnPropertyChanged(nameof(PreviousYear));
    }
}

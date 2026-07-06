using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using DAVE.Models;

namespace DAVE.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

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
    private readonly List<DataCaptureSpreadsheet> _spreadsheets = [];

    public bool HasCurrent => _spreadsheets.Any(s => s is { IsValid: true, IsCurrent: true });
    public bool HasPrevious => _spreadsheets.Any(s => s is { IsValid: true, IsPrevious: true });

    public bool CanDAVE => HasCurrent;

    public void AnalyseAndVerify()
    {
        Console.WriteLine(@"Button Clicked...");
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

            var spreadsheet = new DataCaptureSpreadsheet(file);
            if (spreadsheet.IsValid)
                _spreadsheets.Add(spreadsheet);

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

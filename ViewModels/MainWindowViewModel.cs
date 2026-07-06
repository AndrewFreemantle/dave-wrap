using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using DAVE.Models;

namespace DAVE.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; } = "patiently awaiting spreadsheet(s)...";

    public List<DataCaptureSpreadsheet> _spreadsheets = [];

    public bool CanDAVE => _spreadsheets.Count(s => s.IsValid) >= 1;

    public void AnalyseAndVerify()
    {
        Console.WriteLine($"Button Clicked...");
    }

    /// <summary>
    /// Handles a dropped file; is it a data capture submission?
    /// </summary>
    /// <param name="file"></param>
    public void HandleFile(IStorageItem file)
    {
        try
        {
            _spreadsheets.Add(new DataCaptureSpreadsheet(file));
            Console.WriteLine("MVVM: HandleFile - adding spreadsheet");
            base.OnPropertyChanged(nameof(CanDAVE));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}

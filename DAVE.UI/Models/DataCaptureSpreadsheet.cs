using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Avalonia.Platform.Storage;
using ExcelDataReader;

namespace DAVE.Models;

/// <summary>
/// Represents a WRAP Data Capture Spreadsheet Submission
/// </summary>
public class DataCaptureSpreadsheet
{
    private const int CurrentYear = 2026;

    private DateTime _submissionDate;
    private readonly DataTable? _dataCaptureSheet;

    public bool IsCurrent => _submissionDate.Year == CurrentYear;
    public bool IsPrevious => _submissionDate.Year < CurrentYear;

    public bool IsValid => _dataCaptureSheet != null;

    private CultureInfo _gbCulture = CultureInfo.GetCultureInfo("en-GB");
    public T GetValue<T>(DataFieldName dataFieldName) where T : IParsable<T>
    {
        var rawValue = GetRawValue(dataFieldName).ToString();
        return T.Parse(rawValue, _gbCulture);
    }

    private object GetRawValue(DataFieldName dataFieldName)
    {
        if (!IsValid)
            throw new InvalidOperationException("Invalid data capture sheet");
        if (_dataCaptureSheet == null)
            throw new InvalidOperationException("Data capture sheet not initialized");

        var (row, column) = _dataFieldMappings[dataFieldName];
        return _dataCaptureSheet.Rows[row][column];
    }

    public DataCaptureSpreadsheet(IStorageItem file)
    {
        try
        {
            var path = file.TryGetLocalPath();

            if (path == null) return;

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    Console.WriteLine($@"> Sheets: {reader.ResultsCount}");

                    var result = reader.AsDataSet();
                    var dataCaptureSheet = result.Tables["Data capture sheet"];
                    if (dataCaptureSheet?.Rows[0][0].ToString() == "Food Loss and Waste Data Capture Sheet")
                    {
                        _dataCaptureSheet = dataCaptureSheet;
                        _submissionDate = GetValue<DateTime>(DataFieldName.SubmissionDate);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private readonly Dictionary<DataFieldName, Tuple<int, int>> _dataFieldMappings = new()
    {
        { DataFieldName.CompanyName, new Tuple<int, int>(8 -1, 2) },
        { DataFieldName.AnnualTurnover, new Tuple<int, int>(13 -1, 2) },
        { DataFieldName.SubmissionDate, new Tuple<int, int>(12 -1, 2) },
        { DataFieldName.InventoryPeriod, new Tuple<int, int>(17 -1, 2) },
        { DataFieldName.InventoryPeriodStart, new Tuple<int, int>(17 -1, 2) },
        { DataFieldName.InventoryPeriodEnd, new Tuple<int, int>(18 -1, 2) },
        { DataFieldName.Country, new Tuple<int, int>(22 -1, 2) },
        { DataFieldName.Sector, new Tuple<int, int>(23 -1, 2) },
        { DataFieldName.Lifecycle, new Tuple<int, int>(24 -1, 2) },
        { DataFieldName.SitesTotal, new Tuple<int, int>(26 -1, 2) },
        { DataFieldName.SitesCovered, new Tuple<int, int>(27 -1, 2) },
        { DataFieldName.SitesContributing, new Tuple<int, int>(28 -1, 2) },
    };

}

public enum DataFieldName
{
    CompanyName,
    AnnualTurnover,
    SubmissionDate,
    InventoryPeriod,
    InventoryPeriodStart,
    InventoryPeriodEnd,
    Country,
    Sector,
    Lifecycle,
    SitesTotal,
    SitesCovered,
    SitesContributing,
}

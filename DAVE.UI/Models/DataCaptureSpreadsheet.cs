using System;
using System.Collections.Generic;
using System.Data;
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

    public object GetValue(int row, int column)
    {
        if (!IsValid)
            throw new InvalidOperationException("Invalid data capture sheet");
        if (_dataCaptureSheet == null)
            throw new InvalidOperationException("Data capture sheet not initialized");

        return _dataCaptureSheet.Rows[row][column];
    }

    public object GetValue(DataFieldName dataFieldName)
    {
        return GetValue(_dataFieldMappings[dataFieldName].Item1, _dataFieldMappings[dataFieldName].Item2);
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
                        _submissionDate = DateTime.Parse(GetValue(DataFieldName.SubmissionDate).ToString());
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
        { DataFieldName.CompanyName, new Tuple<int, int>(7, 2) },
        { DataFieldName.SubmissionDate, new Tuple<int, int>(11, 2) }
    };

}

public enum DataFieldName
{
    CompanyName,
    SubmissionDate
}

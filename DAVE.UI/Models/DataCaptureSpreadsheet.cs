using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Numerics;
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

    private readonly CultureInfo _gbCulture = CultureInfo.GetCultureInfo("en-GB");
    public T GetValue<T>(DataFieldName dataFieldName) where T : IParsable<T>
    {
        var (row, column) = _dataFieldMappings[dataFieldName];
        return GetValue<T>(row, column);
    }

    public IEnumerable<T> GetValues<T>(List<int> rows, List<int> columns) where T : IParsable<T>
    {
        var values = new List<T>();
        foreach (var column in columns)
        {
            foreach (var row in rows)
            {
                // rows and columns are zero-based
                values.Add(GetValue<T>(row - 1, column - 1));
            }
        }
        return values;
    }

    private T GetValue<T>(int row, int column) where T : IParsable<T>
    {
        var rawValue = GetRawValue(row, column).ToString();
        if (rawValue != null && T.TryParse(rawValue, _gbCulture, out T? result))
            return result;

        if (typeof(T) == typeof(string))
            return (T)(object)string.Empty;

        // handle null numeric types - Numeric IParsable types (int, double, decimal, etc.) can all parse "0",
        //  so return a zero instead of throwing an exception
        if (T.TryParse("0", _gbCulture, out T? fallback))
            return fallback;

        throw new FormatException(
            $"Unable to parse '{rawValue}' as {typeof(T).Name} for row {row + 1}, column {column + 1}, and no fallback is defined for this type.");
    }

    private object GetRawValue(int row, int column)
    {
        if (!IsValid)
            throw new InvalidOperationException("Invalid data capture sheet");
        if (_dataCaptureSheet == null)
            throw new InvalidOperationException("Data capture sheet not initialized");

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
        // The -1 is because rows are zero-based. Using the actual row number is easier to cross-check against the source spreadsheet
        // Note: if we need to, we can return a different Tuple<row, column> depending on IsCurrent or IsPrevious
        //       useful if the format/layout changes year-on-year
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
        { DataFieldName.TonnesOfFoodProduced, new Tuple<int, int>(29 -1, 2) },
        { DataFieldName.UnitsProduced, new Tuple<int, int>(30 -1, 2) },
        { DataFieldName.HaFSTotalAnnualCovers, new Tuple<int, int>(31 -1, 2) },
        { DataFieldName.PackagingWeight, new Tuple<int, int>(32 -1, 2) },
        { DataFieldName.SewerWastewaterTreatment, new Tuple<int, int>(44 -1, 2) },
        { DataFieldName.TotalFLW, new Tuple<int, int>(49 -1, 2) },
        { DataFieldName.FoodVsInediblePartsNotice, new Tuple<int, int>(55 -1, 4) },
        { DataFieldName.RedistributionNotes, new Tuple<int, int>(52 -1, 4) },
        { DataFieldName.BioRedistributionNotes, new Tuple<int, int>(61 -1, 4) },
        { DataFieldName.FLWReductionTarget, new  Tuple<int, int>(75 -1, 2) },
        { DataFieldName.FLWReductionTargetForm, new  Tuple<int, int>(76 -1, 2) },
        { DataFieldName.FLWReductionBaselineYear, new  Tuple<int, int>(77 -1, 2) },
        { DataFieldName.FLWReductionTargetYear, new  Tuple<int, int>(78 -1, 2) },
        { DataFieldName.FLWReductionPercentage, new  Tuple<int, int>(79 -1, 2) },
        { DataFieldName.FLWReductionProgress, new  Tuple<int, int>(80 -1, 2) },
        { DataFieldName.FLWReductionHotspots, new  Tuple<int, int>(81 -1, 2) },
        { DataFieldName.FLWReductionHotspotsNotes, new  Tuple<int, int>(81 -1, 3) },
        { DataFieldName.FLWReductionOperationalAction, new  Tuple<int, int>(82 -1, 2) },
        { DataFieldName.FLWReductionOperationalActionNotes, new  Tuple<int, int>(82 -1, 3) },
        { DataFieldName.FLWReductionSupplyChain, new  Tuple<int, int>(83 -1, 2) },
        { DataFieldName.FLWReductionSupplyChainNotes, new  Tuple<int, int>(83 -1, 3) },
        { DataFieldName.FLWReductionCitizens, new  Tuple<int, int>(84 -1, 2) },
        { DataFieldName.FLWReductionCitizensNotes, new Tuple<int, int>(84 -1, 3) },
        { DataFieldName.SitesExclusionNotes, new Tuple<int, int>(97 -1, 2) },
        { DataFieldName.SitesExclusionReasons, new Tuple<int, int>(98 -1, 2) },
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
    TonnesOfFoodProduced,
    UnitsProduced,
    HaFSTotalAnnualCovers,
    PackagingWeight,
    SewerWastewaterTreatment,
    TotalFLW,
    FoodVsInediblePartsNotice,
    RedistributionNotes,
    BioRedistributionNotes,
    FLWReductionTarget,
    FLWReductionTargetForm,
    FLWReductionBaselineYear,
    FLWReductionTargetYear,
    FLWReductionPercentage,
    FLWReductionProgress,
    FLWReductionHotspots,
    FLWReductionHotspotsNotes,
    FLWReductionOperationalAction,
    FLWReductionOperationalActionNotes,
    FLWReductionSupplyChain,
    FLWReductionSupplyChainNotes,
    FLWReductionCitizens,
    FLWReductionCitizensNotes,
    SitesExclusionNotes,
    SitesExclusionReasons
}

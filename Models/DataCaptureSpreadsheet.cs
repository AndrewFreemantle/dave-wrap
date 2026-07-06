using System;
using System.IO;
using Avalonia.Platform.Storage;
using ExcelDataReader;

namespace DAVE.Models;

/// <summary>
/// Represents a WRAP Data Capture Spreadsheet Submission
/// </summary>
public class DataCaptureSpreadsheet
{
    private IStorageItem _file;

    public bool IsValid
    {
        get
        {
            try
            {
                using (var stream = File.Open(_file.TryGetLocalPath(), FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        Console.WriteLine($"> Sheets: {reader.ResultsCount}");

                        // Test 1: Company Name
                        //  Find the sheet names 'Data capture sheet'
                        var result = reader.AsDataSet();

                        var dataCaptureSheet = result.Tables["Data capture sheet"];
                        if (dataCaptureSheet == null) return false;


                        return dataCaptureSheet.Rows[0][0].ToString() == "Food Loss and Waste Data Capture Sheet";

                        // {
                        //     var companyName = dataCaptureSheet.Rows[7][2].ToString();
                        //     Console.WriteLine($"> Company name?: {companyName}");
                        // }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }

    public DataCaptureSpreadsheet(IStorageItem file)
    {
        _file = file;
    }
}

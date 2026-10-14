using System;
using Avalonia.Platform.Storage;

namespace DAVE.Models;

public static class StorageItemExtensions
{
    /// <summary>
    /// Reads a dropped file as a Data Capture Spreadsheet submission
    /// </summary>
    public static DataCaptureSpreadsheet ToDataCaptureSpreadsheet(this IStorageItem file)
    {
        string? path = null;
        try
        {
            path = file.TryGetLocalPath();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return new DataCaptureSpreadsheet(path);
    }
}

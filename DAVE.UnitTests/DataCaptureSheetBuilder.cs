using System.Data;

namespace DAVE.UnitTests;

/// <summary>
/// Builds a Data Capture Sheet workbook in memory, laid out the way ExcelDataReader reads a real submission
/// </summary>
public class DataCaptureSheetBuilder
{
    private const int Rows = 150;
    private const int Columns = 8;

    private readonly DataTable _sheet = new("Data capture sheet");

    public DataCaptureSheetBuilder()
    {
        for (var column = 0; column < Columns; column++)
            _sheet.Columns.Add($"Column{column}", typeof(object));

        for (var row = 0; row < Rows; row++)
            _sheet.Rows.Add(_sheet.NewRow());

        Set("A1", "Food Loss and Waste Data Capture Sheet");
    }

    /// <summary>
    /// Sets a cell by its spreadsheet reference, e.g. "C17"
    /// </summary>
    public DataCaptureSheetBuilder Set(string cell, object value)
    {
        var column = cell[0] - 'A';
        var row = int.Parse(cell[1..]) - 1;
        _sheet.Rows[row][column] = value;
        return this;
    }

    public DataSet Build()
    {
        var workbook = new DataSet();
        workbook.Tables.Add(_sheet.Copy());
        return workbook;
    }

    /// <summary>
    /// A submission for the 2025 inventory period, with only the company name answered
    /// </summary>
    public static DataCaptureSheetBuilder Current() => new DataCaptureSheetBuilder()
        .Set("C8", "Acme Foods")
        .Set("C12", "31/03/2026")
        .Set("C17", "01/01/2025")
        .Set("C18", "31/12/2025");

    /// <summary>
    /// The same signatory's submission for the 2024 inventory period
    /// </summary>
    public static DataCaptureSheetBuilder Previous() => new DataCaptureSheetBuilder()
        .Set("C8", "Acme Foods")
        .Set("C12", "31/03/2025")
        .Set("C17", "01/01/2024")
        .Set("C18", "31/12/2024");
}

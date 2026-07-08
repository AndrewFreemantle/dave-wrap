using System;

namespace DAVE.Models;

public class Check
{
    private DataFieldName _fieldName;
    private DataCaptureSpreadsheet _current;
    private DataCaptureSpreadsheet? _previous;
    private Func<string, bool> _checkFn;

    public int Number { get; set; }
    public string Name { get; set; }
    public string Current => _current.GetValue<string>(_fieldName);
    public string Previous => _previous.GetValue<string>(_fieldName);
    public string QueryMessage { get; set; }

    public bool Pass => _checkFn(Current);

    public Check(int number,
        string name,
        DataFieldName fieldName,
        DataCaptureSpreadsheet currentSheet,
        DataCaptureSpreadsheet? previousSheet,
        Func<string, bool> checkFn,
        string queryMessage)
    {
        Number = number;
        Name = name;

        _fieldName = fieldName;
        _current = currentSheet;
        _previous = previousSheet;
        _checkFn = checkFn;
        QueryMessage = queryMessage;
    }
}

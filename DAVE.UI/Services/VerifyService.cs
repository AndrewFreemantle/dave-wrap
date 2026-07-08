using System.Threading.Tasks;
using DAVE.Models;

namespace DAVE.Services;

public interface IVerifyService
{
    Task VerifyAsync(DataCaptureSpreadsheet currentSheet, DataCaptureSpreadsheet? previousSheet);
}

public class VerifyService : IVerifyService
{



    public Task VerifyAsync(DataCaptureSpreadsheet currentSheet, DataCaptureSpreadsheet? previousSheet)
    {
        // First QA Check - business name must exist



        // 8	Company Name - is it included?	IF blank, please raise query.		Row 8: Incomplete response. Please confirm the Company Name to which the data capture sheet refers

        var check1 = currentSheet.GetValue(DataFieldName.CompanyName).ToString();
        if (string.IsNullOrEmpty(check1) || string.IsNullOrWhiteSpace(check1))
        {

        }

        return Task.CompletedTask;
    }
}


public class CheckBase(DataCaptureSpreadsheet currentSheet, DataCaptureSpreadsheet? previousSheet)
{



}

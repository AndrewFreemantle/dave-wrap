using System;

namespace DAVE.Models;

public class CheckHaFSBusiness : CheckBase
{
    private readonly string _hafs = "Hospitality and Food Services";
    private readonly string _sector;
    private readonly string _covers;

    public override bool Pass
    {
        get
        {
            if (string.Equals(_sector, _hafs, StringComparison.InvariantCultureIgnoreCase))
                return !string.IsNullOrWhiteSpace(_covers);

            return true;
        }
    }

    public CheckHaFSBusiness(int number,
        string name,
        string sector,
        string totalAnnualCovers,
        string? previousSector,
        string? previousTotalAnnualCovers,
        string queryMessage)
        : base(
            number,
            name,
            sector,
            previousSector,
            queryMessage)
    {
        _sector = sector;
        _covers = totalAnnualCovers;

        Current = string.Equals(sector, _hafs, StringComparison.InvariantCultureIgnoreCase)
            ? $"HaFS, {totalAnnualCovers}"
            : "Not HaFS";

        if (previousSector != null)
            Previous = string.Equals(previousSector, _hafs, StringComparison.InvariantCultureIgnoreCase)
                ? $"HaFS, {previousTotalAnnualCovers}"
                : "Not HaFS";
    }
}

#nullable disable
using NineTapTour.Core.Entities;
using System.Collections.Generic;

namespace NineTapTour.Core.Repositories;

/// <summary>
/// Reads and saves a member's other earnings: cash not tied to a tournament entry.
/// </summary>
public interface IOtherEarningRepository
{
    /// <summary>
    /// Returns the member's other earnings, oldest first, undated rows last.
    /// </summary>
    List<OtherEarning> GetForMember(int memberNumber);

    /// <summary>
    /// Makes the member's other earnings match <paramref name="rows"/>: rows with
    /// Id 0 are added as hand-entered rows, existing rows are updated, and rows no
    /// longer in the list are deleted.
    /// </summary>
    void SaveForMember(int memberNumber, List<OtherEarning> rows);
}

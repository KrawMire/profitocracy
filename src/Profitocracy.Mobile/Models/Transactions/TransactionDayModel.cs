using Profitocracy.Mobile.Utils;

namespace Profitocracy.Mobile.Models.Transactions;

/// <summary>
/// Transactions of one day, shown as a single card in the activity list.
/// </summary>
public class TransactionDayModel
{
    public required DateTime Date { get; init; }
    public required List<TransactionModel> Transactions { get; init; }
    public required string Total { get; init; }

    /// <summary>
    /// Header of the card, e.g. "Today, 11 March".
    /// </summary>
    public string Title => DateUtils.DayTitle(Date);
}

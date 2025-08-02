using Profitocracy.Core.Domain.Model.Transactions.ValueObjects;
using Profitocracy.Mobile.Resources.Strings;
using System.Globalization;

namespace Profitocracy.Mobile.Models.Transactions
{
    public class RecurringTransactionIntervalModel
    {
        public required string Name { get; init; }
        public required short Value { get; init; }

        public static RecurringTransactionIntervalModel FromDomain(RecurringTransactionInterval recurringTransactionInterval)
        {
            var interval_i18n_name = "RecurringTransactionInterval_" + recurringTransactionInterval.ToString();
            return new RecurringTransactionIntervalModel
            {
                Name = AppResources.ResourceManager.GetString(interval_i18n_name, CultureInfo.CurrentCulture) ?? throw new ArgumentNullException("No resource string found for recurring transaction interval name " + interval_i18n_name),
                Value = (short)recurringTransactionInterval
            };
        }
    }
}

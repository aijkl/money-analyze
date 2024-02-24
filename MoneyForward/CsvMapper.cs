namespace MoneyForward;

internal sealed class ExpensesMapper : CsvHelper.Configuration.ClassMap<Expenses>
{
    internal ExpensesMapper()
    {
        Map(x => x.Date).Index(1).Convert(args =>
        {
            DateOnly.TryParse(args.Row.GetField<string>(1), out var result);
            return result;
        });
        Map(x => x.Summary).Index(2);
        Map(x => x.Amount).Index(3).Convert(args => args.Row.GetField<int>(3));
        Map(x => x.ServiceName).Index(4);
        Map(x => x.Category).Index(5);
        Map(x => x.SubCategory).Index(6);
        Map(x => x.Transfer).Index(8).Convert(args => args.Row.GetField<int>(8) == 1);
    }
}
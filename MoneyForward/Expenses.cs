namespace MoneyForward;

public class Expenses
{
    public DateOnly Date { set; get; }
    public string Summary { set; get; } = string.Empty;
    public int Amount { set; get; }
    public bool Transfer { set; get; }
    public string ServiceName { set; get; } = string.Empty;
    public string Category { set; get; } = string.Empty;
    public string SubCategory { set; get; } = string.Empty;
}
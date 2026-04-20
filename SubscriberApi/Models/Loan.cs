namespace SubscriberApi.Models;

public enum LoanStatus
{
    Pending,
    Approved,
    Rejected
}

public class Loan
{
    public Guid Id { get; set; }
    public string BorrowerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int DurationMonths { get; set; }
    public decimal InterestRate { get; set; }
    public LoanStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
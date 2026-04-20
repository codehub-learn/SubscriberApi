using SubscriberApi.Models;

namespace SubscriberApi.Requests;

public class CreateLoanRequest
{
    public string BorrowerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class UpdateLoanRequest
{
    public string? BorrowerName { get; set; }
    public decimal? Amount { get; set; }
}

public class CreateLoanCommand
{
    public string BorrowerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int DurationMonths { get; set; }
    public int CreditScore { get; set; }   // 300–850
}

public class LoanDecisionResult
{
    public bool Approved { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal MonthlyInstallment { get; set; }
    public Loan Loan { get; set; } = null!;
}
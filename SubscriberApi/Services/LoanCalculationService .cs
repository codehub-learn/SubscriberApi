namespace SubscriberApi.Services;

 
using SubscriberApi.Models;
using SubscriberApi.Persistence;
using SubscriberApi.Requests;

 

public class LoanService : ILoanService
{
    private readonly IPersistenceService _persistence;

    public LoanService(IPersistenceService persistence)
    {
        _persistence = persistence;
    }

    public async Task<LoanDecisionResult> EvaluateAndCreateLoanAsync(
        CreateLoanCommand command,
        CancellationToken cancellationToken)
    {
        // =========================
        // VALIDATION
        // =========================
        if (string.IsNullOrWhiteSpace(command.BorrowerName))
            throw new ArgumentException("Borrower name is required");

        if (command.Amount <= 0)
            throw new ArgumentException("Amount must be > 0");

        if (command.DurationMonths <= 0)
            throw new ArgumentException("Duration must be > 0");

        if (command.CreditScore < 300 || command.CreditScore > 850)
            throw new ArgumentException("Invalid credit score");

        // =========================
        // RISK / APPROVAL RULES
        // =========================
        var approved = true;
        var reason = "Approved";

        if (command.CreditScore < 500)
        {
            approved = false;
            reason = "Low credit score";
        }
        else if (command.Amount > 50000 && command.CreditScore < 650)
        {
            approved = false;
            reason = "High amount with insufficient credit score";
        }

        // =========================
        // INTEREST RATE CALCULATION
        // =========================
        var interestRate = CalculateInterestRate(command.CreditScore);

        // =========================
        // MONTHLY INSTALLMENT
        // =========================
        var monthlyInstallment = CalculateMonthlyInstallment(
            command.Amount,
            interestRate,
            command.DurationMonths);

        // =========================
        // CREATE ENTITY
        // =========================
        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerName = command.BorrowerName,
            Amount = command.Amount,
            DurationMonths = command.DurationMonths,
            InterestRate = interestRate,
            Status = approved ? LoanStatus.Approved : LoanStatus.Rejected,
            CreatedAtUtc = DateTime.UtcNow
        };

        // =========================
        // PERSIST ONLY IF APPROVED
        // =========================
        if (approved)
        {
            await _persistence.AddAsync(loan, cancellationToken);
            await _persistence.SaveChangesAsync(cancellationToken);
        }

        return new LoanDecisionResult
        {
            Approved = approved,
            Reason = reason,
            MonthlyInstallment = monthlyInstallment,
            Loan = loan
        };
    }

    // =========================
    // PRIVATE BUSINESS LOGIC
    // =========================
    private decimal CalculateInterestRate(int creditScore)
    {
        return creditScore switch
        {
            >= 750 => 0.03m,
            >= 650 => 0.05m,
            >= 550 => 0.08m,
            _ => 0.12m
        };
    }

    private decimal CalculateMonthlyInstallment(decimal amount, decimal annualRate, int months)
    {
        var monthlyRate = annualRate / 12;

        if (monthlyRate == 0)
            return amount / months;

        var denominator = (decimal)(1 - Math.Pow((double)(1 + monthlyRate), -months));
        return amount * monthlyRate / denominator;
    }
}
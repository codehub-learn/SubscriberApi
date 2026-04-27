namespace SubscriberApi.BusinessServices;

using SubscriberApi.Models;
using SubscriberApi.Persistence;
using SubscriberApi.Requests;



using Microsoft.Extensions.Logging;

public class LoanService : ILoanService
{
    private readonly ILogger<LoanService> _logger;

    public LoanService(ILogger<LoanService> logger)
    {
        _logger = logger;
    }

    public LoanDecisionResult EvaluateLoan(CreateLoanCommand command)
    {
        _logger.LogInformation("Starting loan evaluation for borrower: {Borrower}", command.BorrowerName);

        try
        {
            // =========================
            // VALIDATION
            // =========================
            Validate(command);

            // =========================
            // DECISION
            // =========================
            var (approved, reason) = EvaluateApproval(command);

            _logger.LogInformation(
                "Loan decision computed. Approved: {Approved}, Reason: {Reason}",
                approved, reason);

            // =========================
            // CALCULATIONS
            // =========================
            var interestRate = CalculateInterestRate(command.CreditScore);

            var monthlyInstallment = CalculateMonthlyInstallment(
                command.Amount,
                interestRate,
                command.DurationMonths);

            _logger.LogDebug(
                "Calculated interest rate: {InterestRate}, Monthly installment: {MonthlyInstallment}",
                interestRate, monthlyInstallment);

            // =========================
            // DOMAIN OBJECT CREATION
            // =========================
            var loan = CreateLoan(command, interestRate, approved);

            _logger.LogInformation(
                "Loan entity created with Id: {LoanId} and Status: {Status}",
                loan.Id, loan.Status);

            return new LoanDecisionResult
            {
                Approved = approved,
                Reason = reason,
                MonthlyInstallment = monthlyInstallment,
                Loan = loan
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error during loan evaluation for borrower: {Borrower}",
                command.BorrowerName);

            throw;
        }
    }

    // =========================
    // VALIDATION
    // =========================
    private void Validate(CreateLoanCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.BorrowerName))
        {
            _logger.LogWarning("Validation failed: Borrower name is empty");
            throw new ArgumentException("Borrower name is required");
        }

        if (command.Amount <= 0)
        {
            _logger.LogWarning("Validation failed: Invalid amount {Amount}", command.Amount);
            throw new ArgumentException("Amount must be > 0");
        }

        if (command.DurationMonths <= 0)
        {
            _logger.LogWarning("Validation failed: Invalid duration {Duration}", command.DurationMonths);
            throw new ArgumentException("Duration must be > 0");
        }

        if (command.CreditScore < 300 || command.CreditScore > 850)
        {
            _logger.LogWarning("Validation failed: Invalid credit score {CreditScore}", command.CreditScore);
            throw new ArgumentException("Invalid credit score");
        }
    }

    // =========================
    // BUSINESS RULES
    // =========================
    private (bool approved, string reason) EvaluateApproval(CreateLoanCommand command)
    {
        if (command.CreditScore < 500)
        {
            _logger.LogInformation("Loan rejected due to low credit score: {CreditScore}", command.CreditScore);
            return (false, "Low credit score");
        }

        if (command.Amount > 50000 && command.CreditScore < 650)
        {
            _logger.LogInformation(
                "Loan rejected due to high amount {Amount} with insufficient credit score {CreditScore}",
                command.Amount, command.CreditScore);

            return (false, "High amount with insufficient credit score");
        }

        return (true, "Approved");
    }

    // =========================
    // DOMAIN FACTORY
    // =========================
    private Loan CreateLoan(CreateLoanCommand command, decimal interestRate, bool approved)
    {
        return new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerName = command.BorrowerName,
            Amount = command.Amount,
            DurationMonths = command.DurationMonths,
            InterestRate = interestRate,
            Status = approved ? LoanStatus.Approved : LoanStatus.Rejected,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    // =========================
    // CALCULATIONS
    // =========================
    private decimal CalculateInterestRate(int creditScore)
    {
        var rate = creditScore switch
        {
            >= 750 => 0.03m,
            >= 650 => 0.05m,
            >= 550 => 0.08m,
            _ => 0.12m
        };

        _logger.LogDebug("Interest rate calculated for score {Score}: {Rate}", creditScore, rate);

        return rate;
    }

    private decimal CalculateMonthlyInstallment(decimal amount, decimal annualRate, int months)
    {
        var monthlyRate = annualRate / 12;

        if (monthlyRate == 0)
        {
            var installment = amount / months;
            _logger.LogDebug("Zero interest loan. Installment: {Installment}", installment);
            return installment;
        }

        var denominator = (decimal)(1 - Math.Pow((double)(1 + monthlyRate), -months));
        var result = amount * monthlyRate / denominator;

        _logger.LogDebug("Monthly installment calculated: {Installment}", result);

        return result;
    }
}
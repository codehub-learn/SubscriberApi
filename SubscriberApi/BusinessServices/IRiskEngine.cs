using SubscriberApi.Requests;

namespace SubscriberApi.BusinessServices;

public interface IRiskEngine
{
    RiskAssessment Assess(CreateLoanCommand command);
}

public class RiskAssessment
{
    public bool IsApproved { get; set; }
    public string? Reason { get; set; }

    public int RiskScore { get; set; }          // 0–100
    public RiskLevel RiskLevel { get; set; }    // Low, Medium, High
}

public enum RiskLevel
{
    Low,
    Medium,
    High
}


public class RiskEngine : IRiskEngine
{
    private readonly ILogger<RiskEngine> _logger;

    public RiskEngine(ILogger<RiskEngine> logger)
    {
        _logger = logger;
    }

    public RiskAssessment Assess(CreateLoanCommand command)
    {
        int score = 0;
        var reasons = new List<string>();

        // =========================
        // RULE 1: Credit Score
        // =========================
        if (command.CreditScore < 500)
        {
            score += 50;
            reasons.Add("Low credit score");
        }
        else if (command.CreditScore < 650)
        {
            score += 20;
        }

        // =========================
        // RULE 2: Loan Amount Risk
        // =========================
        if (command.Amount > 50000)
        {
            score += 30;

            if (command.CreditScore < 650)
            {
                reasons.Add("High amount with insufficient credit score");
            }
        }

        // =========================
        // RISK LEVEL
        // =========================
        var level = score switch
        {
            >= 70 => RiskLevel.High,
            >= 40 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };

        var approved = level != RiskLevel.High;

        var reason = reasons.Any()
            ? string.Join("; ", reasons)
            : "Approved";

        _logger.LogInformation(
            "Risk assessed. Score: {Score}, Level: {Level}, Approved: {Approved}",
            score, level, approved);

        return new RiskAssessment
        {
            RiskScore = score,
            RiskLevel = level,
            IsApproved = approved,
            Reason = reason
        };
    }
}
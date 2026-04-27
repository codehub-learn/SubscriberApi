using SubscriberApi.Requests;

namespace SubscriberApi.BusinessServices;

public interface ILoanService
{
    /// <summary>
    /// Evaluates a loan application and returns the decision result.
    /// </summary>
    /// <param name="command">The loan application details.</param>
    /// <returns>A <see cref="LoanDecisionResult"/> containing the decision and related information.</returns>
    LoanDecisionResult EvaluateLoan(CreateLoanCommand command);
}
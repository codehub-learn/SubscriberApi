using SubscriberApi.Requests;

namespace SubscriberApi.Services;

public interface ILoanService
{
    Task<LoanDecisionResult> EvaluateAndCreateLoanAsync(CreateLoanCommand command, CancellationToken cancellationToken);
}
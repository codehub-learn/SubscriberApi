namespace SubscriberApi.Services;

 
using SubscriberApi.Models;
using SubscriberApi.Persistence;

 

public class LoanPersistenceService
{
    private readonly IPersistenceService _persistenceService;

    public LoanPersistenceService(IPersistenceService persistenceService)
    {
        _persistenceService = persistenceService;
    }

    public async Task<Guid> CreateLoanAsync(string borrowerName, decimal amount, CancellationToken cancellationToken = default)
    {
        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerName = borrowerName,
            Amount = amount,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _persistenceService.AddAsync(loan, cancellationToken);
        await _persistenceService.SaveChangesAsync(cancellationToken);

        return loan.Id;
    }
}
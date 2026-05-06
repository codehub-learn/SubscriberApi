namespace SubscriberApi.BusinessServices;

using Microsoft.EntityFrameworkCore;
using SubscriberApi.Data;
using SubscriberApi.Models;

public class PersistentLoanService
{

    private readonly ApplicationDbContext _db;

    public PersistentLoanService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task AddLoan(decimal amount)
    {
        _db.Loans.Add(new Loan
        {
            Amount = amount
        });

        await _db.SaveChangesAsync();
    }

    public async Task<int> CountLoans()
    {
        return await _db.Loans.CountAsync();
    }
}
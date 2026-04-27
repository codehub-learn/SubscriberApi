using Microsoft.Extensions.Logging;
using SubscriberApi.BusinessServices;
using SubscriberApi.Models;
using SubscriberApi.Requests;
namespace TestProjectLoans;

public class LoanUnitTest
{

         

    [Fact]
    public void ShouldCalculateInterestCorrectly()
    {
        var loan = new Loan { Amount = 1000m };
        var calculatedValue = loan.Amount * 0.05m;
        var expectedValue = 50m;
        Assert.Equal(expectedValue, calculatedValue);
    }

    [Fact]
    public void EvaluateLoan_ShouldApprove_WhenAllCriteriaMet()
    {
        var   _service = new LoanService(new LoggerFactory().CreateLogger<LoanService>());
        var command = new CreateLoanCommand { 
            Amount = 1000, 
            BorrowerName="User", 
            CreditScore=600, 
            DurationMonths=36
        };
        Assert.True(_service.EvaluateLoan(command).Approved);
    }

}

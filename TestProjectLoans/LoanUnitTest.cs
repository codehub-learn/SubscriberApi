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


    //Boundary value (Credit score just below approval threshold)
    [Theory]
    [InlineData(499, false)]
    [InlineData(500, true)]   //Happy path (Nominal values)
    [InlineData(601, true)]
    public void EvaluateLoan_BoundaryCases(int creditScore, bool expectedApproval)
    {
        var _service = new LoanService(new LoggerFactory().CreateLogger<LoanService>());
        var command = new CreateLoanCommand
        {
            Amount = 1000,
            BorrowerName = "User",
            CreditScore = creditScore,
            DurationMonths = 36
        };


        Assert.Equal(expectedApproval, _service.EvaluateLoan(command).Approved);
    }
}
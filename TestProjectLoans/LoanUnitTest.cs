using SubscriberApi.Models;
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



}

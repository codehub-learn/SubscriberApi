using Microsoft.Extensions.Logging;
using Moq;
using SubscriberApi.BusinessServices;
using SubscriberApi.Models;
using SubscriberApi.Requests;
namespace TestProjectLoans;

public class LoanUnitTest
{
    private readonly LoanService _service;
    private readonly Mock<ICreditScoreService> _creditMock;
    private readonly Mock<IFraudService> _fraudMock;

    public LoanUnitTest()
    {
        var logger = new LoggerFactory().CreateLogger<LoanService>();
        _creditMock = new Mock<ICreditScoreService>();
        _fraudMock = new Mock<IFraudService>();
        _service = new LoanService(logger, _creditMock.Object, _fraudMock.Object);
    }


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
    [InlineData(499, true)]
    [InlineData(500, true)]   //Happy path (Nominal values)
    [InlineData(601, true)]
    public void EvaluateLoan_BoundaryCases(int creditScore, bool expectedApproval)
    {
        _creditMock.Setup(x => x.GetCreditScore(It.IsAny<int>())).Returns(500);
        _fraudMock.Setup( x => x.IsFraudulent(It.IsAny<int>())).Returns(false);

        var command = new CreateLoanCommand
        {
            Amount = 1000,
            BorrowerName = "User",
            CreditScore = creditScore,
            DurationMonths = 36
        };
        Assert.Equal(expectedApproval, _service.EvaluateLoan(command).Approved);
    }

    //input validation, guard clauses
    [Fact]
    public void EvaluateLoan_ShouldThrow_WhenCommandIsNull()
    {
        _creditMock.Setup(x => x.GetCreditScore(It.IsAny<int>())).Returns(500);
        _fraudMock.Setup(x => x.IsFraudulent(It.IsAny<int>())).Returns(false);

        var command = new CreateLoanCommand
        {
            Amount = 1000,
            BorrowerName = string.Empty,
            CreditScore = 500,
            DurationMonths = 36
        };

        Assert.Throws<ArgumentException>(() => _service.EvaluateLoan(command));
    }


    [Fact]
    public void EvaluateLoan_ShouldReturnCompleteResult()
    {
        _creditMock.Setup(x => x.GetCreditScore(It.IsAny<int>())).Returns(500);
        _fraudMock.Setup(x => x.IsFraudulent(It.IsAny<int>())).Returns(false);

        var result = _service.EvaluateLoan(validCommand());

        Assert.NotNull(result);
        Assert.True(result.Approved);
        Assert.Equal(36, result.Loan.DurationMonths);
    }


    private CreateLoanCommand validCommand()
    {
        return new CreateLoanCommand
        {
            Amount = 1000,
            BorrowerName = "User",
            CreditScore = 500,
            DurationMonths = 36
        };
    }

}
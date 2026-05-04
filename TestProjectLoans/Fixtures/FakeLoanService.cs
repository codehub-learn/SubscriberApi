using SubscriberApi.BusinessServices;


namespace TestProjectLoans.Fixtures;

using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class LoanServiceTests
{
    private readonly LoanServiceFixture _fixture;
    private readonly ILoanService _service;

    public LoanServiceTests()
    {
        _fixture = new LoanServiceFixture();
        _service = _fixture.CreateService();
    }

    //Happy Path (Approved)
    [Fact]
    public void EvaluateLoan_ShouldApprove_WhenValid()
    {
        _fixture.CreditMock
            .Setup(x => x.GetCreditScore(1))
            .Returns(720);

        _fixture.FraudMock
            .Setup(x => x.IsFraudulent(1))
            .Returns(false);

        var command = _fixture.ValidCommand();

        var result = _service.EvaluateLoan(command);

        Assert.True(result.Approved);
        Assert.Equal("Approved", result.Reason);
        Assert.NotNull(result.Loan);
    }

    //  Fraud Case(Early Exit)  Important: this bypasses all calculations

    [Fact]
    public void EvaluateLoan_ShouldReject_WhenFraudDetected()
    {
        _fixture.FraudMock
            .Setup(x => x.IsFraudulent(1))
            .Returns(true);

        var command = _fixture.ValidCommand();

        var result = _service.EvaluateLoan(command);

        Assert.False(result.Approved);
        Assert.Equal("Fraud suspicion", result.Reason);
        Assert.Null(result.Loan);
    }

    //Low Credit Score
    [Fact]
    public void EvaluateLoan_ShouldReject_LowCreditScore()
    {
        _fixture.CreditMock
            .Setup(x => x.GetCreditScore(1))
            .Returns(450);

        _fixture.FraudMock
            .Setup(x => x.IsFraudulent(1))
            .Returns(false);

        var command = _fixture.ValidCommand();

        var result = _service.EvaluateLoan(command);

        Assert.False(result.Approved);
        Assert.Equal("Low credit score", result.Reason);
    }

    //High Amount + Medium Credit
    [Fact]
    public void EvaluateLoan_ShouldReject_HighAmountWithLowScore()
    {
        _fixture.CreditMock
            .Setup(x => x.GetCreditScore(1))
            .Returns(600);

        _fixture.FraudMock
            .Setup(x => x.IsFraudulent(1))
            .Returns(false);

        var command = _fixture.ValidCommand();
        command.Amount = 60000;

        var result = _service.EvaluateLoan(command);

        Assert.False(result.Approved);
        Assert.Equal("High amount with insufficient credit score", result.Reason);
    }

    //Validation Failure
    [Fact]
    public void EvaluateLoan_ShouldThrow_WhenInvalidAmount()
    {
        var command = _fixture.ValidCommand();
        command.Amount = 0;

        Assert.Throws<ArgumentException>(() =>
            _service.EvaluateLoan(command));
    }


    //Verify Logging (Advanced / Enterprise)
    //Logging is side effect, not business logic.
    //We want to ensure that important events are logged without making our tests brittle. 

    [Fact]
    public void EvaluateLoan_ShouldLogInformation()
    {
        _fixture.CreditMock
            .Setup(x => x.GetCreditScore(It.IsAny<int>()))
            .Returns(720);

        _fixture.FraudMock
            .Setup(x => x.IsFraudulent(It.IsAny<int>()))
            .Returns(false);

        var command = _fixture.ValidCommand();

        _service.EvaluateLoan(command);

        _fixture.LoggerMock.Verify(
              x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<object, Exception?, string>>()),
             Times.AtLeastOnce);
    }

}

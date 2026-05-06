using System;
using System.Collections.Generic;
using System.Text;

namespace TestProjectLoans.AutoFixtures;

using AutoFixture;
using FluentAssertions;
using Moq;
using SubscriberApi.BusinessServices;
using SubscriberApi.Requests;
using Xunit;

public class LoanServiceTests : AutoFixtureTestBase
{
    private readonly ILoanService _service;
    private Mock<ICreditScoreService> creditMock;
    private Mock<IFraudService> fraudMock;
    private Mock<IRiskEngine> riskMock;

    public LoanServiceTests()
    {
        //Very important freeze should be in front of creating the service,
        //otherwise it will create new mocks instead of using the frozen ones

        // Arrange or Prepare
        creditMock = Fixture.Freeze<Mock<ICreditScoreService>>();
        fraudMock = Fixture.Freeze<Mock<IFraudService>>();
        riskMock = Fixture.Freeze<Mock<IRiskEngine>>();


        // Auto-creates LoanService with injected mocks
        _service = Fixture.Create<LoanService>();
    }

    //Freeze dependencies to control their behavior in tests
    [Fact]
    public void EvaluateLoan_ShouldApprove_WhenLowRisk()
    {

        creditMock
            .Setup(x => x.GetCreditScore(It.IsAny<int>()))
            .Returns(750);

        fraudMock
            .Setup(x => x.IsFraudulent(It.IsAny<int>()))
            .Returns(false);

        riskMock
            .Setup(x => x.Assess(It.IsAny<CreateLoanCommand>()))
            .Returns(new RiskAssessment
            {
                IsApproved = true,
                Reason = "Approved"
            });

        var command = Fixture.Build<CreateLoanCommand>()
            .With(x => x.BorrowerId, 1)  // ensure high credit score
            .With(x => x.CreditScore, 600)  // avoid validation failure
            .With(x => x.Amount, 10000)
            .With(x => x.DurationMonths, 12)
            .With(x => x.BorrowerName, "John")
            .Create();

        // Act or Calculation
        var result = _service.EvaluateLoan(command);

        // Assert or Verify
        result.Approved.Should().BeTrue();
        result.Reason.Should().Be("Approved");
    }


    [Fact]
    public void EvaluateLoan_ShouldReject_WhenFraudDetected()
    {


        fraudMock
            .Setup(x => x.IsFraudulent(It.IsAny<int>()))
            .Returns(true);

        var command = Fixture.Build<CreateLoanCommand>()
            .With(x => x.CreditScore, 700)
            .With(x => x.Amount, 10000)
            .With(x => x.DurationMonths, 12)
            .With(x => x.BorrowerName, "John")
            .Create();

        var result = _service.EvaluateLoan(command);

        result.Approved.Should().BeFalse();
        result.Reason.Should().Be("Fraud suspicion");
    }
    [Fact]
    public void EvaluateLoan_ShouldThrow_WhenInvalidAmount()
    {

        // Arrange

        var command = Fixture.Build<CreateLoanCommand>()
            .With(x => x.Amount, 0)
            .With(x => x.CreditScore, 700)
            .With(x => x.DurationMonths, 12)
            .With(x => x.BorrowerName, "John")
            .Create();

        // Act
        Action act = () => _service.EvaluateLoan(command);

        // Assert
        act.Should().Throw<ArgumentException>();
    }
}
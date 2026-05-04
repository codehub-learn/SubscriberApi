using System;
using System.Collections.Generic;
using System.Text;

namespace TestProjectLoans.Fixtures;

using Moq;
using Microsoft.Extensions.Logging;
using SubscriberApi.BusinessServices;
using SubscriberApi.Requests;
using SubscriberApi.Models;

public class LoanServiceFixture
{
    public Mock<ICreditScoreService> CreditMock { get; } = new();
    public Mock<IFraudService> FraudMock { get; } = new();

    // Logger is dependency → mock or NullLogger
    public Mock<ILogger<LoanService>> LoggerMock { get; } = new();

    public Mock<IRiskEngine> RiskMock { get; } = new();

    public ILoanService CreateService()
    {
        return new LoanService(
            LoggerMock.Object,
            CreditMock.Object,
            FraudMock.Object,
            RiskMock.Object);
    }


    public CreateLoanCommand HappyPathSetup()
    {
        CreditMock
           .Setup(x => x.GetCreditScore(1))
           .Returns(720);

        FraudMock
            .Setup(x => x.IsFraudulent(1))
            .Returns(false);

        var command = ValidCommand();
        return command;
    }


    public CreateLoanCommand ValidCommand()
    {
        return new CreateLoanCommand
        {
            BorrowerId = 1,
            BorrowerName = "John Doe",
            Amount = 10000,
            DurationMonths = 12,
            CreditScore = 700 // must pass validation
        };
    }
}
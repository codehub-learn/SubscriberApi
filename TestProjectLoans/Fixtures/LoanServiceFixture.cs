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

    public ILoanService CreateService()
    {
        return new LoanService(
            LoggerMock.Object,
            CreditMock.Object,
            FraudMock.Object);
    }

    public CreateLoanCommand ValidCommand()
    {
        return new CreateLoanCommand
        {
            BorrowerId = 12,
            BorrowerName = "John Doe",
            Amount = 10000,
            DurationMonths = 12,
            CreditScore = 700 // must pass validation
        };
    }
}
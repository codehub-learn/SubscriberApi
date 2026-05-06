namespace TestProjectLoans.TestContainers;

using Microsoft.EntityFrameworkCore;
using SubscriberApi.BusinessServices;
using SubscriberApi.Data;
using Testcontainers.MsSql;
using Xunit;

public class PersistentLoanServiceTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer;
    private ApplicationDbContext _dbContext = null!;

    public PersistentLoanServiceTests()
    {
        _sqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrongPassword123!")
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        // Start SQL Server container
        await _sqlContainer.StartAsync();

        // Configure EF Core
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_sqlContainer.GetConnectionString())
            .Options;

        _dbContext = new ApplicationDbContext(options);

        // Create database schema
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }

        await _sqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task AddLoan_ShouldInsertLoanIntoDatabase()
    {
        // Arrange
        var service = new PersistentLoanService(_dbContext);

        // Act
        await service.AddLoan(5000);

        // Assert
        var count = await service.CountLoans();

        Assert.Equal(1, count);
    }
}
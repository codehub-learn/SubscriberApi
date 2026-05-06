 

namespace TestProjectLoans.WebTests;
 
using System.Net;

public class LoanApiTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoanApiTests(
        CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetLoans_ShouldReturnSuccess()
    {
        //Arrange


        // Act
        var response = await _client.GetAsync("/api/loans", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }
    [Fact]
    public async Task CreateLoan_ShouldInsertLoan()
    {
        //Arrange

        // Act
        var response = await _client.PostAsync("/api/loans?amount=5000", 
            null, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

}
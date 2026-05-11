 

namespace TestProjectLoans.WebTests;

using FluentAssertions;
using SubscriberApi.Dtos;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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


        //List<Loan> loans = response




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


    //Authorization test
    [Fact]
    public async Task ShouldNotReadMessageWithoutAuthorization()
    {

        var  response = await 
            _client.GetAsync("/api/auth/secureMessage", TestContext.Current.CancellationToken);

        //Assert
        response.StatusCode
           .Should()
           .Be(HttpStatusCode.Unauthorized);

    }

    [Fact]
    public async Task Login_ReturnsToken()
    {
        var loginRequest = new
        {
            username = "admin",
            password = "1234"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest, TestContext.Current.CancellationToken);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>(cancellationToken: TestContext.Current.CancellationToken)
                ?? throw new InvalidOperationException("Failed to deserialize login response.");

        result.Token.Should().NotBeNull();



    }

    [Fact]
    public async Task GetLoans_WithValidToken_Returns200()
    {
        // Login first
        var loginRequest = new
        {
            username = "admin",
            password = "1234"
        };

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/auth/login",
                loginRequest, TestContext.Current.CancellationToken);

        var tokenResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken) ?? throw new InvalidOperationException("Failed to deserialize login response.");

        // Attach JWT
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                tokenResult.Token);

        // Call secured endpoint
        var response =
            await _client.GetAsync("/api/auth/secureMessage", TestContext.Current.CancellationToken);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }




}
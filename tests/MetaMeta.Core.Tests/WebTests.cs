using Microsoft.Extensions.Logging;
using Projects;

namespace MetaMeta.Tests;

/// <summary>
/// Integration tests for the web frontend application.
/// </summary>
public class WebTests
{
    // Maximum time to wait for operations before timing out
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Tests that the web frontend root endpoint returns a successful status code.
    /// </summary>
    /// <returns>A task representing the asynchronous test operation.</returns>
    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Step 1: Arrange the test environment with a distributed application
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<MetaMeta_AppHost>();
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            // To output logs to the xUnit.net ITestOutputHelper, consider adding a package from https://www.nuget.org/packages?q=xunit+logging
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        // Step 2: Build and start the application
        await using var app = await appHost.BuildAsync().WaitAsync(DefaultTimeout);
        await app.StartAsync().WaitAsync(DefaultTimeout);

        // Step 3: Act - create an HTTP client and make a request to the root endpoint
        var httpClient = app.CreateHttpClient("webfrontend");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("webfrontend").WaitAsync(DefaultTimeout);
        var response = await httpClient.GetAsync("/");

        // Step 4: Assert that the response has a successful status code
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

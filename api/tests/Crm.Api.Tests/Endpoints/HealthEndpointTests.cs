using Crm.Api.Tests.Infrastructure;

namespace Crm.Api.Tests.Endpoints;

public class HealthEndpointTests(CrmApiFactory factory) : IClassFixture<CrmApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetHealth_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Healthy", content);
    }
}
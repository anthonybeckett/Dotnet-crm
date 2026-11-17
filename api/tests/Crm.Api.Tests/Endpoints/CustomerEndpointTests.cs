using System.Net;
using System.Net.Http.Json;
using Crm.Api.Tests.Infrastructure;
using Crm.Api.Contracts.Customers;
using Crm.Shared.Pagination;

namespace Crm.Api.Tests.Endpoints;

public class CustomerEndpointTests(CrmApiFactory factory) : IClassFixture<CrmApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateCustomer_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            CustomerType = 1,
            Name = "Integration Test Customer",
            Email = "integration@example.com",
            PhoneNumber = "07700900123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers/", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerCreatedResponse>();

        Assert.NotNull(createdCustomer);
        Assert.NotEqual(Guid.Empty, createdCustomer.Id);

        var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        
        var customer = await getResponse.Content
            .ReadFromJsonAsync<CustomerResponse>();

        Assert.NotNull(customer);
        Assert.Equal("Integration Test Customer", customer.Name);
        Assert.Equal("integration@example.com", customer.Email);
        Assert.Equal("07700900123", customer.PhoneNumber);
        Assert.Equal(1, (int)customer.CustomerType);
    }
    
    [Fact]
    public async Task GetCustomers_ReturnsPagedResult()
    {
        // Act
        var response = await _client.GetAsync("/api/customers/?page=1&pageSize=20");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<PagedResult<CustomerResponse>>();

        Assert.NotNull(result);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);
        Assert.True(result.TotalCount > 0);
        Assert.NotEmpty(result.Items);
    }
    
    [Fact]
    public async Task CreateCustomer_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            CustomerType = 1,
            Name = "",
            Email = "invalid-email",
            PhoneNumber = "07700900123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers/", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetCustomerById_WithUnknownId_ReturnsNotFound()
    {
        // Arrange
        var unknownId = Guid.CreateVersion7();

        // Act
        var response = await _client.GetAsync($"/api/customers/{unknownId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private record CustomerCreatedResponse(Guid Id);
}
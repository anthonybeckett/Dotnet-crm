using Crm.Api.Contracts.Customers;
using Crm.Modules.Customers.Application.Commands;
using Wolverine;

namespace Crm.Api.Endpoints.Customers;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/api/customers")
            .WithTags("Customers");

        group.MapPost("/",
                async (CreateCustomerRequest request, IMessageBus messageBus, CancellationToken cancellationToken) =>
                {
                    var command = new CreateCustomerCommand(
                        request.CustomerType,
                        request.Name,
                        request.Email,
                        request.PhoneNumber);

                    var customerId = await messageBus.InvokeAsync<Guid>(command, cancellationToken);

                    return Results.Created($"/api/customers/{customerId}",
                        new
                        {
                            Id = customerId
                        });
                })
            .WithName("CreateCustomer");

        return endpoints;
    }
}
using Crm.Api.Contracts.Customers;
using Crm.Modules.Customers.Application.Commands;
using Crm.Modules.Customers.Application.Queries;
using Crm.Modules.Customers.Domain.Entities;
using Crm.Shared.Pagination;
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
            }).WithName("CreateCustomer");

        group.MapGet("/{id:guid}", async (Guid id, IMessageBus messageBus, CancellationToken cancellationToken) =>
        {
            var customer = await messageBus.InvokeAsync<Customer?>(new GetCustomerByIdQuery(id), cancellationToken);

            if (customer is null)
            {
                return Results.NotFound();
            }

            var response = new CustomerResponse(
                customer.Id,
                customer.CustomerType,
                customer.Name,
                customer.Email,
                customer.PhoneNumber,
                customer.CreatedAtUtc,
                customer.IsArchived);

            return Results.Ok(response);
        }).WithName("GetCustomerById");

        group.MapGet("/",
            async (int? page, int? pageSize, bool? includeArchived, IMessageBus messageBus, CancellationToken cancellationToken) =>
            {
                var query = new GetCustomersQuery(page ?? 1, pageSize ?? 20, includeArchived ?? false);

                var result = await messageBus.InvokeAsync<PagedResult<Customer>>(query, cancellationToken);

                var response = new PagedResult<CustomerResponse>(
                    result.Items.Select(customer => new CustomerResponse(
                        customer.Id,
                        customer.CustomerType,
                        customer.Name,
                        customer.Email,
                        customer.PhoneNumber,
                        customer.CreatedAtUtc,
                        customer.IsArchived)).ToList(),
                    result.Page,
                    result.PageSize,
                    result.TotalCount);

                return Results.Ok(response);
            }).WithName("GetCustomers");

        group.MapPut(
            "/{id:guid}",
            async (
                Guid id,
                UpdateCustomerRequest request,
                IMessageBus messageBus,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateCustomerCommand(
                    id,
                    request.CustomerType,
                    request.Name,
                    request.Email,
                    request.PhoneNumber);

                await messageBus.InvokeAsync(command, cancellationToken);

                return Results.NoContent();
            });

        group.MapDelete("/{id:guid}", async (Guid id, IMessageBus messageBus, CancellationToken cancellationToken) =>
        {
            await messageBus.InvokeAsync(new ArchiveCustomerCommand(id), cancellationToken);

            return Results.NoContent();
        });
        
        group.MapPost("/{id:guid}/restore", async (Guid id, IMessageBus messageBus, CancellationToken cancellationToken) =>
        {
            await messageBus.InvokeAsync(new RestoreCustomerCommand(id), cancellationToken);

            return Results.NoContent();
        });

        return endpoints;
    }
}
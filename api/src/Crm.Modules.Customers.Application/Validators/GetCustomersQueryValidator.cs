using Crm.Modules.Customers.Application.Queries;
using FluentValidation;

namespace Crm.Modules.Customers.Application.Validators;

public class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
{
    public GetCustomersQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than zero.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");
    }
}
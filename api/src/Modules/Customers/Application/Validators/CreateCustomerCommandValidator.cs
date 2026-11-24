using Crm.Modules.Customers.Application.Commands;
using FluentValidation;

namespace Crm.Modules.Customers.Application.Validators;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(customer => customer.CustomerType)
            .IsInEnum()
            .WithMessage("A valid customer type is required.");

        RuleFor(customer => customer.Name)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(200)
            .WithMessage("Customer name cannot exceed 200 characters.");

        RuleFor(customer => customer.Email)
            .MaximumLength(320)
            .WithMessage("Email cannot exceed 320 characters.")
            .EmailAddress()
            .WithMessage("A valid email address is required.")
            .When(customer =>
                !string.IsNullOrWhiteSpace(customer.Email));

        RuleFor(customer => customer.PhoneNumber)
            .MaximumLength(50)
            .WithMessage("Phone number cannot exceed 50 characters.");
    }
}
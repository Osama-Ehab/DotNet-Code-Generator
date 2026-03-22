using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation; // Nuget: FluentValidation
namespace CodeGeneratorSolution.EmbeddedResources.Application.Base
{
    public abstract class BaseValidator<T> : AbstractValidator<T> 
    {
    public BaseValidator()
    {
        // Global Rule: The object itself cannot be null
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Input data cannot be null.");

    }

    // You can add custom reusable validators here
    // Example: A helper for standard "Name" fields
    protected void RuleForName(System.Linq.Expressions.Expression<Func<T, string>> expression)
    {
        RuleFor(expression)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    } }
}

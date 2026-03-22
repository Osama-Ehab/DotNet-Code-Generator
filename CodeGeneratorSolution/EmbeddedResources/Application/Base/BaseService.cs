using CodeGeneratorSolution.Templetes.Infrastructure;
using CodeGeneratorSolution.Templetes.Infrastructure.Utilities; // For Result<T>
using FluentValidation; // For IValidator<T>
using Microsoft.Extensions.DependencyInjection; // For IServiceProvider
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Application.Base
{
    public abstract class BaseService
    {
        // This is the Global "Waiter" that fetches things for us dynamically
        private readonly IServiceProvider _serviceProvider;

        protected BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// A generic helper that automatically finds the right validator for ANY DTO
        /// and runs the rules, returning a clean Result object instead of throwing exceptions.
        /// </summary>
        protected async Task<Result<bool>> ValidateDtoAsync<TDto>(TDto dto)
        {
            if (dto == null)
                return Result<bool>.Failure("Provided data cannot be null.");

            using (var scope = _serviceProvider.CreateScope())
            {

                // 1. Ask the Waiter: "Do you have a registered IValidator for this specific DTO type?"
                var validator = scope.ServiceProvider.GetService<IValidator<TDto>>();

                // 2. If the Waiter found a validator, run the validation rules!
                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(dto);

                    if (!validationResult.IsValid)
                    {
                        // 3. Combine all FluentValidation error messages into one single string
                        string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));

                        // Return a clean Failure Result (No try/catch needed in the UI!)
                        return Result<bool>.Failure(errors);
                    }
                }

            }        

            // 4. If it passed all rules (or if no validator was registered for this DTO), return Success!
            return Result<bool>.Success(true);
        }
    }
}
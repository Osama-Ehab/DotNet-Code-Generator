using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using CodeGeneratorSolution.Infrastructure.Utilities;
using CodeGeneratorSolution.Core.Enums;
using CodeGeneratorSolution.Core.Interfaces; // IUserSession

namespace CodeGeneratorSolution.Application.Base
{
    public abstract class BaseService
    {
        private readonly IServiceProvider _serviceProvider;
        protected readonly IUserSession _session; // متاح للخدمات المُولدة

        protected BaseService(IServiceProvider serviceProvider, IUserSession session)
        {
            _serviceProvider = serviceProvider;
            _session = session;
        }

        // ==========================================
        // محرك التحقق الديناميكي (Validation Engine)
        // ==========================================
        protected async Task<Result<bool>> ValidateDtoAsync<TDto>(TDto dto)
        {
            if (dto == null)
                return Result<bool>.Failure("Provided data cannot be null.", ErrorType.Validation);

            using (var scope = _serviceProvider.CreateScope())
            {
                var validator = scope.ServiceProvider.GetService<IValidator<TDto>>();

                if (validator != null)
                {
                    var validationResult = await validator.ValidateAsync(dto);

                    if (!validationResult.IsValid)
                    {
                        string errors = string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
                        return Result<bool>.Failure(errors, ErrorType.Validation);
                    }
                }
            }
            return Result<bool>.Success(true);
        }
    }
}
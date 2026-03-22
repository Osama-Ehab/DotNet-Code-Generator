// DVLD.Common/ServiceExecutor.cs
using CodeGeneratorSolution.Templetes.Infrastructure.Enums;
using CodeGeneratorSolution.Templetes.Infrastructure.Utilities;
using FluentValidation;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.EmbeddedResources.Application
{

    public static class ServiceExecutor
    {
        // 1. Generic Wrapper for methods that return data (Result<T>)
        public static async Task<Result<T>> RunAsync<T>(Func<Task<Result<T>>> logic)
        {
            try
            {
                return await logic();
            }
            catch (SqlException ex)
            {
                return Result<T>.Failure(SqlErrorMapper.GetMessage(ex), SqlErrorMapper.GetErrorType(ex.Number));
            }
            catch (Exception ex)
            {
                return Result<T>.Failure("An unexpected error occurred.", ErrorType.Unexpected);
            }
        }
        public static async Task<Result<bool>> RunActionAsync(Func<Task> logic)
        {
            try
            {
                await logic();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message); // Uses the same centralized logic
            }
        }

       
    }
}

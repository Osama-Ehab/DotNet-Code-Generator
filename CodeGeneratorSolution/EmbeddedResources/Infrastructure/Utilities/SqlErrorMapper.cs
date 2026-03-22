using CodeGeneratorSolution.Templetes.Infrastructure.Enums;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace CodeGeneratorSolution.Templetes.Infrastructure.Utilities
{

    public static class SqlErrorMapper
    {
        // Map ID -> Config Object (Not just a string anymore)
        private static Dictionary<int, ErrorConfig> _errorConfigs = new Dictionary<int, ErrorConfig>();

        static SqlErrorMapper()
        {
            LoadErrorsFromJson("errors.json");
        }

        private static void LoadErrorsFromJson(string filePath)
        {
            if (!File.Exists(filePath)) return;

            try
            {
                string json = File.ReadAllText(filePath);
                _errorConfigs = JsonSerializer.Deserialize<Dictionary<int, ErrorConfig>>(json);
            }
            catch { /* Log error */ }
        }

        // 1. Get Type (Simple Lookup)
        public static ErrorType GetErrorType(int sqlNumber)
        {
            if (_errorConfigs.TryGetValue(sqlNumber, out var config))
            {
                return Enum.TryParse(config.Type, true, out ErrorType type) ? type : ErrorType.Database;
            }
            return ErrorType.Database;
        }

        // 2. Get Message (Smart Contextual Lookup)
        public static string GetMessage(SqlException ex)
        {
            if (_errorConfigs.TryGetValue(ex.Number, out var config))
            {
                // A. Check specific contexts defined in JSON
                if (config.Contexts != null && config.Contexts.Any())
                {
                    // Find the first context where the SQL message contains the keyword
                    var match = config.Contexts.FirstOrDefault(ctx => ex.Message.Contains(ctx.Contains));

                    if (match != null)
                    {
                        return match.Message;
                    }
                }

                // B. If no context matches, return default
                return config.DefaultMessage;
            }

            return "An undefined database error occurred.";
        }
    }
}

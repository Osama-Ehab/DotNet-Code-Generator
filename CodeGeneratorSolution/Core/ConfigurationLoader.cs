using CodeGeneratorSolution.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Core
{
    public static class ConfigurationLoader
    {
        public static GeneratorConfig LoadConfig()
        {
            // 1. Tell the builder to look in the exact folder where the .exe is running
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // 2. Compile the configuration
            IConfigurationRoot configuration = builder.Build();

            // 3. Bind the JSON section to your C# class
            var configObject = new GeneratorConfig();
            configuration.GetSection("GeneratorConfig").Bind(configObject);

            // 4. Safety Checks
            if (string.IsNullOrWhiteSpace(configObject.ConnectionString))
                throw new InvalidOperationException("Connection string is missing in appsettings.json!");

            if (string.IsNullOrWhiteSpace(configObject.OutputRootDirectory))
                throw new InvalidOperationException("Output directory is missing in appsettings.json!");

            return configObject;
        }
    }
}

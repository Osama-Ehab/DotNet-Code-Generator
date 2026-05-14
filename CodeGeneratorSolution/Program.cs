
using CodeGeneratorSolution.Core;
using Humanizer;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace CodeGeneratorSolution 
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        public static IConfiguration Configuration { get; private set; }


        [STAThread]
        static async Task Main()
        {
            //Application.SetHighDpiMode(HighDpiMode.SystemAware);
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            // 1. Build Configuration
            var configObject = ConfigurationLoader.LoadConfig();

            GeneratorEngine generatorEngine = new GeneratorEngine(configObject.ConnectionString, configObject.OutputRootDirectory, configObject.SolutionName);
            await generatorEngine.GenerateSolutionAsync();
            stopWatch.Stop();

            Console.WriteLine(stopWatch.ToString());

        

        }
    }
}
  
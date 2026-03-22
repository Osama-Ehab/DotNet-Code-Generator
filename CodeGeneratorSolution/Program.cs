
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
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            GeneratorEngine generatorEngine = new GeneratorEngine(Configuration.GetConnectionString("DefaultConnection"), "C:\\DVLDSolution\\DVLD", "DVLD");
            generatorEngine.GenerateEntryPoint("FrmMain");
            await generatorEngine.GenerateSolutionAsync();
            stopWatch.Stop();

            Console.WriteLine(stopWatch.ToString());

        

        }
    }
}
  
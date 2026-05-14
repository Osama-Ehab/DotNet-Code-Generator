using CodeGeneratorSolution.Core;
using CodeGeneratorSolution.Utlis;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CodeGeneratorSolution.Utils
{
    public class EmbeddedResourceManager
    {
        private readonly PathResolver _pathResolver;
        private readonly ContentProcessor _contentProcessor;

        public EmbeddedResourceManager(string targetSolutionName)
        {
            _pathResolver = new PathResolver(targetSolutionName);
            _contentProcessor = new ContentProcessor(targetSolutionName);
        }

        public void Extract(string targetOutputRoot)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string prefix = "CodeGeneratorSolution.EmbeddedResources.";

            var staticResources = assembly.GetManifestResourceNames()
                                          .Where(r => r.StartsWith(prefix));

            Console.WriteLine("\n--- Extracting Static Architecture ---");

            foreach (string resourceName in staticResources)
            {
                string relativeDottedPath = resourceName.Substring(prefix.Length);

                // 1. Ask the resolver where this goes
                var (folderPath, fileName) = _pathResolver.ResolvePath(relativeDottedPath);

                
                string finalDirectory = folderPath == null ? targetOutputRoot : Path.Combine(targetOutputRoot, folderPath);
                string finalFilePath = Path.Combine(finalDirectory, fileName);


                // Skip project files if they already exist
                if (folderPath == ProjectManifest.Root && File.Exists(finalFilePath))
                {
                    // UX: Yellow color indicates a safe "skip" to protect the developer's custom code
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"[SKIPPED] {finalFilePath,-50} (Preserved existing file)");
                    Console.ResetColor();
                    continue;
                }

                // 2. Create the directories and write the file
                EnsureDirectoryExists(finalFilePath);

                // 3. Read the file
                using (Stream stream = assembly.GetManifestResourceStream(resourceName)!)
                using (StreamReader reader = new StreamReader(stream))
                {
                    string rawContent = reader.ReadToEnd();

                    // 4. Ask the processor to modify the text
                    string finalContent = _contentProcessor.Process(rawContent);

                    // 5. Write to disk
                    File.WriteAllText(finalFilePath, finalContent);
                }

                // UX: Green color indicates a successful code generation
                Console.ForegroundColor = ConsoleColor.Green;
                // The ,-50 formatting aligns the output text nicely into columns!
                Console.WriteLine($"[CREATED] {finalFilePath,-50} (Success)");
                Console.ResetColor();
            }

            Console.WriteLine("--------------------------------------\n");
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            string folder = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }
    }
}
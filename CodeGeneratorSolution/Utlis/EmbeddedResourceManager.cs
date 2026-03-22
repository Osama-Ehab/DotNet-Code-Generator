using CodeGeneratorSolution.Utlis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CodeGeneratorSolution.Utils
{
    public static class EmbeddedResourceManager
    {
        /// <summary>
        /// Scans the assembly for resources starting with a specific prefix,
        /// and dumps them into the target directory, preserving the folder structure.
        /// </summary>
        /// <param name="resourcePrefix">e.g. "MyGen.Templates.Infrastructure"</param>
        /// <param name="outputRoot">e.g. "C:\Output\MyApp.Infrastructure"</param>
        /// <param name="replacements">Dictionary of placeholders to replace</param>
        public static void ExtractAll(string resourcePrefix, string outputRoot, Dictionary<string, string> replacements)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string[] allResources = assembly.GetManifestResourceNames();

            // 1. Filter: Get only files in this "Folder"
            var targetResources = allResources
                .Where(name => name.StartsWith(resourcePrefix))
                .ToList();

            Console.WriteLine($"🔍 Found {targetResources.Count} resources under '{resourcePrefix}'");

            foreach (var resourceName in targetResources)
            {
                // 2. Read Content
                string content = "";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        // Debugging Aid: List all available resources if not found
                        var available = string.Join(", ", assembly.GetManifestResourceNames());
                        throw new FileNotFoundException($"Resource '{resourceName}' not found. \nAvailable: {available}");
                    }

                    using (StreamReader reader = new StreamReader(stream))
                    {
                        content = reader.ReadToEnd();
                    }
                }

                // 3. Apply Replacements (The "Switch Case" Logic)
                // This replaces {{SolutionName}}, {{Namespace}}, etc. automatically
                foreach (var item in replacements)
                {
                    content = content.Replace(item.Key, item.Value);
                }

                // 4. Determine Output Path (The Tricky Part)
                // Convert: "MyGen.Templates.Infrastructure.Data.Helper.cs"
                // To:      "Data\Helper.cs"

                string relativeName = resourceName.Substring(resourcePrefix.Length + 1);

                // 5. FIX: Remove .template extension FIRST
                // We do this BEFORE calculating folders so "Program.cs" stays together.
                if (relativeName.EndsWith(".template"))
                {
                    relativeName = relativeName.Substring(0, relativeName.Length - ".template".Length);
                }

                string subFoldersPath = GetSubDirectoryFormResourcePath(relativeName);
                string filename = GetFileNameFormResourcePath(relativeName);

                // 6. Write File
                FileWriter.WriteFile(outputRoot, subFoldersPath,filename, content);

                Console.WriteLine($"   📄 Extracted: {filename}");
            }
        }

        // Helper to convert "Data.Helper.cs" -> "Data\Helper.cs"
        // Handles the issue where Folders and Extensions both use dots.
        private static string GetFileNameFormResourcePath(string resourceName)
        {
            string[] parts = resourceName.Split('.');
            if (parts.Length < 2) return resourceName;

            // The last 2 parts are the Filename + Extension (e.g. "Helper" + "cs")
            string fileName = $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}";

            return fileName;
        }
        private static string GetSubDirectoryFormResourcePath(string resourceName)
        {
            string[] parts = resourceName.Split('.');
            if (parts.Length <= 2) return "";

            string SubfolderPath = Path.Combine(parts.Take(parts.Length - 2).ToArray());

            return SubfolderPath;
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
using System;
using System.IO;
using System.Threading;

namespace CodeGeneratorSolution.Utlis
{
    public static class FileWriter
    {
        /// <summary>
        /// Safely wipes the output directory to ensure a clean generation.
        /// Includes safety checks to prevent deleting your hard drive.
        /// </summary>
        public static void CleanOutputDirectory(string directoryPath)
        {
            if (string.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("Output directory cannot be empty.");

            // 1. SAFETY CHECK: Prevent deleting root drives (e.g. "C:\")
            if (Path.GetPathRoot(directoryPath) == directoryPath)
            {
                throw new InvalidOperationException($"SAFETY TRIGGER: You cannot delete a root drive: {directoryPath}");
            }

            // 2. Delete existing directory
            if (Directory.Exists(directoryPath))
            {
                Console.WriteLine($"🧹 Cleaning output directory: {directoryPath}...");
                try
                {
                    Directory.Delete(directoryPath, true);

                    // Wait 100ms for Windows to release file locks (Common VS issue)
                    Thread.Sleep(100);
                }
                catch (IOException ex)
                {
                    // If a file is open in VS, we might fail. Warn and continue.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"⚠️ Warning: Could not fully clean directory. Is a file open? \nError: {ex.Message}");
                    Console.ResetColor();
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Error: Access Denied. Check permissions.\nError: {ex.Message}");
                    Console.ResetColor();
                    throw; // Stop here, we can't generate.
                }
            }

            // 3. Re-create the empty root folder
            Directory.CreateDirectory(directoryPath);
            Console.WriteLine("✅ Directory Cleaned.");
        }

        /// <summary>
        /// Scaffolds the standard 4-Layer Onion Architecture folder structure.
        /// </summary>
        public static void InitializeOutputDirectories(string outputDir, string solutionName)
        {
            Console.WriteLine("📂 Initializing Project Structure...");

            // 1. Define Top-Level Layers
            string corePath = Path.Combine(outputDir, $"Core");
            string infraPath = Path.Combine(outputDir, $"Infrastructure");
            string appPath = Path.Combine(outputDir, $"Application");
            string uiPath = Path.Combine(outputDir, $"UI");

            // 2. Create Core (Domain) Folders
            // Note: We separate 'Generated' code from 'Custom' code
            Directory.CreateDirectory(Path.Combine(corePath, "Entities", "Generated"));
            Directory.CreateDirectory(Path.Combine(corePath, "DTOs"));
            Directory.CreateDirectory(Path.Combine(corePath, "Interfaces"));

            // 3. Create Infrastructure (Data Access) Folders
            Directory.CreateDirectory(Path.Combine(infraPath, "Base"));
            Directory.CreateDirectory(Path.Combine(infraPath, "Data")); // For GenericDataHelper
            Directory.CreateDirectory(Path.Combine(infraPath, "Security")); // For SecurityHelper
            Directory.CreateDirectory(Path.Combine(infraPath, "Repositories", "Generated"));

            // 4. Create Application (Business Logic) Folders
            Directory.CreateDirectory(Path.Combine(appPath, "Base"));
            Directory.CreateDirectory(Path.Combine(appPath, "Services", "Generated"));
            Directory.CreateDirectory(Path.Combine(appPath, "Validators"));
            Directory.CreateDirectory(Path.Combine(appPath, "Mappers"));
            Directory.CreateDirectory(Path.Combine(appPath, "Exceptions"));

            // 5. Create UI (Presentation) Folders
            Directory.CreateDirectory(Path.Combine(uiPath, "Forms"));
            Directory.CreateDirectory(Path.Combine(uiPath, "Controls"));
            Directory.CreateDirectory(Path.Combine(uiPath, "Resources"));


            Console.WriteLine("✅ Project Infrastructure Created.");
        }

        /// <summary>
        /// Helper to write content to a specific file, ensuring the directory exists.
        /// </summary>
        public static void WriteFile(string folderPath, string SubFoldersPath, string fileName, string content)
        {
            string DirectoryPath = folderPath;
            if (SubFoldersPath != null)
            {
                DirectoryPath = Path.Combine(folderPath, SubFoldersPath);

            }
            string fullPath = Path.Combine(DirectoryPath, fileName);

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            File.WriteAllText(fullPath, content);
        }

        
    }
}
using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace CodeGeneratorSolution
{
    public static class ScriptExecutor
    {
        public static async Task RunAllScriptsInFolder(string folderPath, string connectionString)
        {
            if (!Directory.Exists(folderPath)) return;

            string[] files = Directory.GetFiles(folderPath, "*.sql");
            Console.WriteLine($"\n--- Deploying {files.Length} SQL Scripts to Database ---");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();


                foreach (var file in files)
                {
                    string fileName =  Path.GetFileName(file);
                    Console.Write($"Executing {fileName}... ");

                    try
                    {
                        string scriptContent = await File.ReadAllTextAsync(file);
                        ExecuteScript(conn, scriptContent);
                        Console.WriteLine("Success.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nFAILED: {ex.Message}");
                    }
                }
            }
        }

        private static void ExecuteScript(SqlConnection conn, string script)
        {
            // 1. Split script by "GO" (Case insensitive, whole word on its own line)
            string[] commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

           foreach(var command in commands)
            {
                if (string.IsNullOrWhiteSpace(command)) return;

                using (SqlCommand cmd = new SqlCommand(command, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
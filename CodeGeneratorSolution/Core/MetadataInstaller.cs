using System;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Data.SqlClient;

namespace CodeGeneratorSolution.Core
{
    public static class MetadataInstaller
    {
        public static bool EnsureStoredProceduresExist(string connectionString)
        {
            Console.WriteLine("Checking System Procedures...");

            // 1. Define the path to the SQL file we wrote above
            // Assuming you saved it in a folder named 'SystemSQL' in your project
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sql","usp_MetaDataFetcherScript.sql");

            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("Warning: Metadata SQL file not found. Skipping install.");
                    return false;
                }
                // 2. Read the script
                string script = File.ReadAllText(filePath);

                // 3. Reuse our ScriptExecutor logic to install them
                // We temporarily write it to a temp folder to reuse the existing executor logic, 
                // OR we can just execute it directly here. Let's run directly for simplicity.

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Use the Regex logic from ScriptExecutor to split by GO
                    string[] commands = System.Text.RegularExpressions.Regex.Split(script, @"^\s*GO\s*$", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    foreach (var cmdText in commands)
                    {
                        if (string.IsNullOrWhiteSpace(cmdText)) continue;
                        using (SqlCommand cmd = new SqlCommand(cmdText, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                Console.WriteLine("System Procedures Installed Successfully.");
                return true;
            }
            catch { Console.WriteLine("System Procedures Installed failed."); return false; }
            return false;
           

            
        
        }


        private static bool ExecuteSql(SqlConnection conn, string sql)
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn)) {  return  cmd.ExecuteNonQuery() > 0; }
        }
    }
}
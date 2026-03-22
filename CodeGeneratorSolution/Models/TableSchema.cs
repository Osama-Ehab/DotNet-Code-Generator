using CodeGeneratorSolution.Utlis;
using CodeGeneratorSolution.Core;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGeneratorSolution.Models
{
    public class TableSchema
    {

    // 1. ALLOCATE REGEX EXACTLY ONCE
    private static readonly System.Text.RegularExpressions.Regex _camelCaseRegex =
        new System.Text.RegularExpressions.Regex("([a-z])([A-Z])", System.Text.RegularExpressions.RegexOptions.Compiled);

        // ========================================================================
        // 1. RAW DATA (Injected by SchemaReader)
        // ========================================================================
        public required string Name { get; set; } // e.g., "Users" or "tbl_People"
        public List<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();
        public List<ColumnDefinition> GridColumns { get; set; } = new List<ColumnDefinition>();
        public List<ColumnDefinition> DetailedColumns { get; set; } = new List<ColumnDefinition>();

        // ========================================================================
        // 2. NAMING CONVENTIONS (Powered by Humanizer)
        // ========================================================================
        // Class Name: "User"
        public string ClassName => NameFormatter.ToPascalCase(NameFormatter.Singularize(Name));

        // Variable Name (Camel Case): "user" (Perfect for: var user = new User();)
        public string CamelCaseName => ClassName.Camelize();

        // Plural Name: "Users" (Perfect for: IEnumerable<User> Users)
        public string PluralName => ClassName.Pluralize();

        // ========================================================================
        // 3. PRIMARY KEY HELPERS (No more null checks in T4!)
        // ========================================================================
        public ColumnDefinition? PrimaryKey => Columns.FirstOrDefault(c => c.IsPrimaryKey);

        public string PkName => PrimaryKey?.Name ??
            throw new InvalidOperationException($"Table '{Name}' does not have a single Primary Key defined. Cannot generate standard CRUD operations.");

        public string PkType => PrimaryKey?.CSharpType ?? "int";

        // ========================================================================
        // 4. DTO & MAPPING HELPERS (Optimized)
        // ========================================================================
        public IEnumerable<ColumnDefinition> CreateDtoColumns => Columns.Where(c => c.IncludeInInsert);
        public IEnumerable<ColumnDefinition> UpdateDtoColumns => Columns.Where(c => c.IncludeInUpdate);
        public IEnumerable<ColumnDefinition> StandrdDtoColumns => Columns.Where(c => !c.IsSensitive);

        // ========================================================================
        // 6. UI GRID & SEARCH HELPERS (Optimized & Custom-Value Aware)
        // ========================================================================

        public IEnumerable<ColumnDefinition> UniqueSelectorColumns => GridColumns.Where(c => c.IsUniqueSelector);
        public IEnumerable<ColumnDefinition> SearchableColumns => GridColumns.Where(c => c.IsSearchable);

        // Any column that has values for a combobox
        public IEnumerable<ColumnDefinition> DropdownColumns => SearchableColumns.Where(c => c.HasAllowedValues);

        // ONLY standard booleans that still use exactly "True" and "False"
        public IEnumerable<ColumnDefinition> DropdownStandardBooleanColumns =>
            DropdownColumns.Where(c => c.CSharpType == "bool" && c.AllowedFilterValues.Contains("True"));

        // ALL other dropdowns: String Enums (e.g., "Pending, Complete") AND Overridden Booleans (e.g., "Active, Inactive")
        public IEnumerable<ColumnDefinition> DropdownCustomColumns =>
            DropdownColumns.Except(DropdownStandardBooleanColumns);

        // ========================================================================
        // 7. FEATURE FLAGS
        // ========================================================================
        public bool IsAuditable => Columns.Any(c => c.IsAuditable);
        public bool HasPassword => Columns.Any(c => c.IsHoldPassword); // Ensure IsSensitive catches passwords
        public bool HasFileOrImage => Columns.Any(c => c.CSharpType == "byte[]");

        // ========================================================================
        // 8. TABLE-LEVEL METADATA (Extended Properties)
        // ========================================================================
        public Dictionary<string, string> ExtendedProperties { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // The Magic Flag!
        public bool GenerateSelectorControl
        {
            get
            {
                if (ExtendedProperties.TryGetValue("GenerateSelectorControl", out string val))
                {
                    return val.ToLower() == "true" || val == "1";
                }
                return false; // Default to false if the DB Architect didn't add the property
            }
        }

        public string FriendlyName
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return string.Empty;

                if (ExtendedProperties.TryGetValue("DisplayName", out string extDisplayName))
                {
                    return extDisplayName;
                }

                // 2. USE THE COMPILED REGEX (Blazing Fast)
                string result = _camelCaseRegex.Replace(Name, "$1 $2");

                // Split the spaced string into individual words
                string[] words = result.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // 4. The Initializer: If it's 3 or more words, shrink the first word to an initial!
                if (words.Length >= 3)
                {
                    // "Customer Billing Address" -> "C. Billing Address"
                    words[0] = words[0].Substring(0, 1) + ".";

                    if (words.Length >= 5)
                    {
                        words[1] = words[1].Substring(0, 1) + ".";
                    }
                }

                // Join them back together cleanly
                return string.Join(" ", words).Trim();
            }
        }

    }
}
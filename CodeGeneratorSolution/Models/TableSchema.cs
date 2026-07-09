using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeGeneratorSolution.Models
{
    public class TableSchema
    {
        // استخدام Regex مجمع وسريع لتقسيم الكلمات
        private static readonly Regex _camelCaseRegex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);

        // ========================================================================
        // 1. RAW DATA (Injected by SchemaReader)
        // ========================================================================
        public string Name { get; set; }
        public List<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();

        // ========================================================================
        // 2. NAMING CONVENTIONS (Powered by Humanizer)
        // ========================================================================
        // استغنينا عن NameFormatter واستخدمنا Humanizer مباشرة لتقليل التبعيات
        public string EntityName => Name.Singularize().Pascalize();
        public string CamelCaseName => EntityName.Camelize();
        public string PluralName => EntityName.Pluralize();

        // ========================================================================
        // 3. PRIMARY KEY HELPERS
        // ========================================================================
        public ColumnDefinition PrimaryKey => Columns.FirstOrDefault(c => c.IsPrimaryKey);

        public string PkName => PrimaryKey?.Name ??
            throw new InvalidOperationException($"Table '{Name}' does not have a single Primary Key defined.");

        public string PkType => PrimaryKey?.CSharpType ?? "int";

        // ========================================================================
        // 4. DTO & MAPPING HELPERS
        // ========================================================================
        public IEnumerable<ColumnDefinition> CreateDtoColumns => Columns.Where(c => c.IncludeInInsert);
        public IEnumerable<ColumnDefinition> UpdateDtoColumns => Columns.Where(c => c.IncludeInUpdate);

        // تم الإصلاح: السماح للـ Primary Key بالتواجد في الـ DTO الأساسي!
        public IEnumerable<ColumnDefinition> StandrdDtoColumns => Columns.Where(c => !(c.MetaIsSensitive ?? false));

        // ========================================================================
        // 5. THE MAGIC LINK FOR LOCALIZATION (للإجراءات المخزنة)
        // ========================================================================
        public List<string> LocalizedColumnPrefixes
        {
            get
            {
                var arColumns = Columns.Where(c => c.Name.EndsWith("Ar")).Select(c => c.Name.Substring(0, c.Name.Length - 2));
                var enColumns = Columns.Where(c => c.Name.EndsWith("En")).Select(c => c.Name.Substring(0, c.Name.Length - 2));
                return arColumns.Intersect(enColumns).ToList();
            }
        }

        // ========================================================================
        // 6. UI GRID & SEARCH HELPERS (Smart Properties)
        // ========================================================================
        // الفلترة الذكية: لا تظهر الباسورد، ولا الصور، ولا النصوص التي تتجاوز 250 حرف في الـ Grid
        public IEnumerable<ColumnDefinition> GridColumns => Columns.Where(c =>
            !(c.MetaIsSensitive ?? false) &&
            c.CSharpType != "byte[]" &&
            (c.MaxLength <= 250 || c.MaxLength == -1));

        public IEnumerable<ColumnDefinition> DetailedColumns => Columns.Where(c => !(c.MetaIsSensitive ?? false));

        public IEnumerable<ColumnDefinition> UniqueSelectorColumns => Columns.Where(c => c.IsUniqueSelector);
        public IEnumerable<ColumnDefinition> SearchableColumns => Columns.Where(c => (c.MetaIsSearchable ?? false));
        public IEnumerable<ColumnDefinition> DropdownColumns => SearchableColumns.Where(c => c.HasMetaAllowedValues);

        // ========================================================================
        // 7. FEATURE FLAGS
        // ========================================================================
        public bool IsAuditable => Columns.Any(c => c.IsAuditable);
        public bool HasPassword => Columns.Any(c => c.MetaIsSensitive ?? false);
        public bool HasFileOrImage => Columns.Any(c => c.CSharpType == "byte[]");
        public bool GenerateSelectorControl => Columns.Any(c => c.MetaIsUniqueSelector??false);

        public Dictionary<string, string> ExtendedProperties { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // ========================================================================
        // 8. FRIENDLY NAMES
        // ========================================================================
        public string FriendlyNameEn
        {
            get
            {
                if (ExtendedProperties.TryGetValue("DisplayNameEn", out string extDisplayName))
                    return extDisplayName;

                string result = _camelCaseRegex.Replace(Name, "$1 $2");
                return result.Singularize().Humanize(LetterCasing.Title);
            }
        }

        public string FriendlyNameAr
        {
            get
            {
                if (ExtendedProperties.TryGetValue("DisplayNameAr", out string extDisplayName))
                    return extDisplayName;

                return TranslationHelper.TranslateEntityName(FriendlyNameEn); 
            }
        }
    }
}
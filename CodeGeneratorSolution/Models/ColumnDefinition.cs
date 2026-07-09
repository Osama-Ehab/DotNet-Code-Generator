using CodeGeneratorSolution.CSharp_Compiler.UI.Helpers;
using Humanizer;
using System;
using System.Text.RegularExpressions;

namespace CodeGeneratorSolution.Models
{
    public class ColumnDefinition
    {
        // استخدمنا Regex مجمع للأداء الصاروخي
        private static readonly Regex _camelCaseRegex = new Regex("([a-z])([A-Z])", RegexOptions.Compiled);

        // ========================================================================
        // 1. DATABASE SCHEMA
        // ========================================================================
        public string Name { get; set; }
        public string SqlType { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsForeignKey { get; set; }

        // ========================================================================
        // 2. METADATA OVERRIDES
        // ========================================================================
        public string MetaDisplayNameAr { get; set; }
        public string MetaDisplayNameEn { get; set; }
        public string MetaControlType { get; set; }
        public string MetaIconCode { get; set; }
        public string MetaAllowedValues { get; set; }
        public bool HasMetaAllowedValues => !string.IsNullOrWhiteSpace(MetaAllowedValues);

        public bool? MetaIsSensitive { get; set; }
        public bool? MetaIsAuditable { get; set; }
        public bool? MetaIncludeInInsert { get; set; }
        public bool? MetaIncludeInUpdate { get; set; }
        public bool? MetaIsPassword { get; set; }
        public bool? MetaIsSearchable { get; set; }
        public bool? MetaIsUniqueSelector { get; set; }
        public bool? MetaIsLookupColumn { get; set; }

        public int? UIRow { get; set; }
        public int? UIColumn { get; set; }
        public int? UIColSpan { get; set; }

        // ========================================================================
        // 3. C# TYPE MAPPING
        // ========================================================================
        public string PrimitiveCSharpType
        {
            get
            {
                return SqlType.ToLower() switch
                {
                    "int" => "int",
                    "bigint" => "long",
                    "smallint" => "short",
                    "tinyint" => "byte",
                    "bit" => "bool",
                    "date" => "DateTime",
                    "datetime" => "DateTime",
                    "datetime2" => "DateTime",
                    "decimal" => "decimal",
                    "money" => "decimal",
                    "float" => "double",
                    "uniqueidentifier" => "Guid",
                    "image" => "byte[]",
                    "varbinary" => "byte[]",
                    _ => "string"
                };
            }
        }

        public string CSharpType
        {
            get
            {
                string primitive = PrimitiveCSharpType;
                if (IsNullable && primitive != "string" && primitive != "byte[]" && primitive != "object")
                    return primitive + "?";
                return primitive;
            }
        }

        // ========================================================================
        // 4. SQL & ADO.NET HELPERS
        // ========================================================================
        public string SqlParameterName => "@" + Name;
        public string SqlLocalVariableName => "@" + Name + "_Local";

        public string SqlFullType
        {
            get
            {
                string t = SqlType.ToUpper();
                if (t.Contains("CHAR"))
                {
                    string len = MaxLength == -1 ? "MAX" : MaxLength.ToString();
                    return $"{t}({len})";
                }
                if (t == "DECIMAL" || t == "NUMERIC") return $"{t}({Precision},{Scale})";
                return t;
            }
        }

        // ========================================================================
        // 5. BUSINESS LOGIC & FLAGS
        // ========================================================================
        public bool IncludeInInsert => MetaIncludeInInsert ?? (!IsIdentity && Name != "ModifiedDate" && Name != "ModifiedBy");
        public bool IncludeInUpdate => MetaIncludeInUpdate ?? (!(IsPrimaryKey || IsIdentity || Name == "CreatedDate" || Name == "CreatedBy"));
        public bool IsAuditable => MetaIsAuditable ?? (Name == "CreatedDate" || Name == "CreatedBy" || Name == "ModifiedDate" || Name == "ModifiedBy");
        public bool IsUniqueSelector => MetaIsUniqueSelector ?? IsPrimaryKey;

        // ربط ذكي بالحقول اللغوية والبحث
        public bool IsLookupColumn => MetaIsLookupColumn ?? (!IsPrimaryKey && (Name.ToLower().Contains("name") || Name.ToLower().Contains("title")));

        // ========================================================================
        // 6. UI PRESENTATION & ICONS
        // ========================================================================
        public string CamelCaseName => Name.Camelize();
        public string LabelName => "lbl" + Name;

        public string WinControl
        {
            get
            {
                // the foregin key will be ModernComboBoxControl
                if (!string.IsNullOrWhiteSpace(MetaControlType)) return MetaControlType;

                string t = SqlType.ToLower();
                if (t == "int" || t == "decimal" || t == "money") return "NumericUpDown";
                if (t == "bit") return "ModernToggle";
                if (t.Contains("date")) return "ModernDateTimePicker";
                if (t.Contains("varchar") && MaxLength > 255) return "ModernInputGroup";

                return "ModernInputGroup";
            }
        }

        public string ControlNameWithPrefix
        {
            get
            {
                string prefix = WinControl switch
                {
                    "ModernToggle" => "tgl",      // تحديث يتوافق مع CustomUIControls
                    "ModernDateTimePicker" => "dtp",
                    "ModernInputGroup" => "mInput",
                    "ComboBox" => "cmb",
                    "NumericUpDown" => "nud",
                    _ => "txt"
                };
                return prefix + Name;
            }
        }

        public string FriendlyNameEn
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MetaDisplayNameEn)) return MetaDisplayNameEn;
                if (string.IsNullOrEmpty(Name)) return string.Empty;

                string result = _camelCaseRegex.Replace(Name, "$1 $2");
                if (!IsPrimaryKey && result.EndsWith(" I D", StringComparison.OrdinalIgnoreCase))
                    result = result.Substring(0, result.Length - 4).Trim();

                return result;
            }
        }

        public string FriendlyNameAr => !string.IsNullOrWhiteSpace(MetaDisplayNameAr) ? MetaDisplayNameAr : FriendlyNameEn;
    

// --- ICONS (Outline & Solid) ---
           public string IconOutline
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(MetaIconCode)) return MetaIconCode; // يمكن تجاوزها من الداتابيز

                string n = Name.ToLower();
                if (n.Contains("email") || n.Contains("mail")) return SegoeIcons.Email.Outline;
                if (n.Contains("phone") || n.Contains("mobile")) return SegoeIcons.Phone.Outline;
                if (n.Contains("pass")) return SegoeIcons.Password.Outline;
                if (n.Contains("user") || n.Contains("name")) return SegoeIcons.Person.Outline;
                if (n.Contains("address") || n.Contains("city")) return SegoeIcons.Location.Outline;
                if (n.Contains("date") || n.Contains("time")) return SegoeIcons.Calendar.Outline;
                if (n.Contains("money") || n.Contains("price") || n.Contains("salary") || n.Contains("fee")) return SegoeIcons.Money.Outline;
                if (n.Contains("document") || n.Contains("file")) return SegoeIcons.Document.Outline;
                if (IsPrimaryKey || n.Contains("id")) return SegoeIcons.Key.Outline;

                return SegoeIcons.Default.Outline;
            }
        }

        public string IconSolid
        {
            get
            {
                // نفس المنطق، لكن يُرجع نسخة الـ Solid للـ Hover
                string n = Name.ToLower();
                if (n.Contains("email") || n.Contains("mail")) return SegoeIcons.Email.Solid;
                if (n.Contains("phone") || n.Contains("mobile")) return SegoeIcons.Phone.Solid;
                if (n.Contains("pass")) return SegoeIcons.Password.Solid;
                if (n.Contains("user") || n.Contains("name")) return SegoeIcons.Person.Solid;
                if (n.Contains("address") || n.Contains("city")) return SegoeIcons.Location.Solid;
                if (n.Contains("date") || n.Contains("time")) return SegoeIcons.Calendar.Solid;
                if (n.Contains("money") || n.Contains("price") || n.Contains("salary") || n.Contains("fee")) return SegoeIcons.Money.Solid;
                if (n.Contains("document") || n.Contains("file")) return SegoeIcons.Document.Solid;
                if (IsPrimaryKey || n.Contains("id")) return SegoeIcons.Key.Solid;

                return SegoeIcons.Default.Solid;
            }
        }


    }
}
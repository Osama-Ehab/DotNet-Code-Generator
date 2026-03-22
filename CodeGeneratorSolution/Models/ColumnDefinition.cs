using Humanizer;

namespace CodeGeneratorSolution.Models
{
    public class ColumnDefinition
    {
        // ... (Existing Properties: Name, SqlType, IsNullable, etc.) ...
        public required string Name { get; set; }
        public required string SqlType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsForeignKey { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; } // Recommended: Add this for Decimal/Money
        public int Scale { get; set; }     // Recommended: Add this for Decimal/Money

        // ... (Existing C# Mappings) ...

        // ========================================================================
        // 1. SQL QUERY HELPERS
        // ========================================================================

        // Returns the parameter name used in Stored Procs (e.g., "@FirstName")
        public string SqlParameterName => "@" + Name;

        // Returns the parameter name for local variables in SPs (e.g., "@NewID")
        // Useful for OUTPUT parameters logic
        public string SqlLocalVariableName => "@" + Name + "_Local";

        // Returns the full SQL declaration (e.g., "NVARCHAR(50)", "DECIMAL(18, 2)")
        // Essential for creating CREATE TABLE scripts or DECLARE statements inside SPs
        public string SqlFullType
        {
            get
            {
                string t = SqlType.ToUpper();
                if (t.Contains("CHAR")) // CHAR, NCHAR, VARCHAR, NVARCHAR
                {
                    string len = MaxLength == -1 ? "MAX" : MaxLength.ToString();
                    return $"{t}({len})";
                }
                if (t == "DECIMAL" || t == "NUMERIC")
                {
                    // Default to (18,0) if generic, or use actual precision/scale if you have them
                    return $"{t}({Precision},{Scale})";
                }
                return t;
            }
        }

        // ========================================================================
        // 2. ADO.NET HELPERS (For Repository Generation)
        // ========================================================================

        // Maps the string SQL type to the System.Data.SqlDbType Enum name
        // Usage in T4: cmd.Parameters.Add("@Name", SqlDbType.<#= col.SqlDbTypeEnum #>)
        public string SqlDbTypeEnum
        {
            get
            {
                return SqlType.ToLower() switch
                {
                    "int" => "Int",
                    "bigint" => "BigInt",
                    "smallint" => "SmallInt",
                    "tinyint" => "TinyInt",
                    "bit" => "Bit",
                    "date" => "Date",
                    "datetime" => "DateTime",
                    "datetime2" => "DateTime2",
                    "decimal" => "Decimal",
                    "money" => "Money",
                    "float" => "Float",
                    "uniqueidentifier" => "UniqueIdentifier",
                    "varchar" => "VarChar",
                    "nvarchar" => "NVarChar",
                    "char" => "Char",
                    "nchar" => "NChar",
                    "varbinary" => "VarBinary",
                    "image" => "Image",
                    "xml" => "Xml",
                    _ => "VarChar" // Default fallback
                };
            }
        }


        // ========================================================================
        // 3. SECURITY & AUDIT FLAGS (Metadata-Driven)
        // ========================================================================
        private static readonly HashSet<string> _sensitiveKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "password", "passwd", "pwd", "salt", "hash", "token",
            "secret", "apikey", "otp", "securitystamp", "sessionid",
            "creditcard", "cvv"
        };

        public bool IsSensitive
        {
            get
            {
                // 1. METADATA OVERRIDE
                if (ExtendedProperties.TryGetValue("IsSensitive", out string extProp))
                    return extProp.ToLower() == "true" || extProp == "1";

                // 2. FALLBACK
                if (string.IsNullOrEmpty(Name)) return false;
                foreach (string keyword in _sensitiveKeywords)
                {
                    if (Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0) return true;
                }
                return false;
            }
        }

        public bool IsAuditable
        {
            get
            {
                // 1. METADATA OVERRIDE
                if (ExtendedProperties.TryGetValue("IsAuditable", out string extProp))
                    return extProp.ToLower() == "true" || extProp == "1";

                // 2. FALLBACK
                return Name == "CreatedDate" || Name == "CreatedBy" ||
                       Name == "ModifiedDate" || Name == "ModifiedBy";
            }
        }

        // ========================================================================
        // 4. CRUD & DTO LOGIC FLAGS (Metadata-Driven)
        // ========================================================================

        // Determines if the column is included in the stored procedure INSERT parameters
        public bool IncludeInInsert
        {
            get
            {
                if (ExtendedProperties.TryGetValue("IncludeInInsert", out string extProp))
                    return extProp.ToLower() == "true" || extProp == "1";

                // Default: Skip auto-increment IDs and modification audit fields
                return !IsIdentity && Name != "ModifiedDate" && Name != "ModifiedBy";
            }
        }

        // Determines if the column is included in the stored procedure UPDATE parameters
        public bool IncludeInUpdate
        {
            get
            {
                if (ExtendedProperties.TryGetValue("IncludeInUpdate", out string extProp))
                    return extProp.ToLower() == "true" || extProp == "1";

                // Default: Never update PKs, Identities, or Creation audit fields
                if (IsPrimaryKey || IsIdentity || Name == "CreatedDate" || Name == "CreatedBy")
                    return false;

                return true;
            }
        }

        // ========================================================================
        // 4. UI DISPLAY HELPERS
        // ========================================================================

        // 1. STATIC DICTIONARY (Decoupled & Memory Optimized)
        // Allocated only once per application lifecycle, not per-property access.
        private static readonly Dictionary<string, string> _friendlyAbbreviations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Description", "Desc" },
            { "Transaction", "Trans" },
            { "Department", "Dept" },
            { "Maximum", "Max" },
            { "Minimum", "Min" },
            { "Information", "Info" },
            { "Identification", "ID" },
            { "Address", "Addr" },
            { "Number", "No" },
            { "Amount", "Amt" },
            { "Configuration", "Config" },
            { "Management", "Mgmt" },
            { "Organization", "Org" },
            { "Quantity", "Qty" }
        };


        // Variable Name (Camel Case):
        public string CamelCaseName => Name.Camelize();

        // 1. ALLOCATE REGEX EXACTLY ONCE
        private static readonly System.Text.RegularExpressions.Regex _camelCaseRegex =
            new System.Text.RegularExpressions.Regex("([a-z])([A-Z])", System.Text.RegularExpressions.RegexOptions.Compiled);

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

                // Remove generic suffixes like "ID" for foreign keys
                if (!IsPrimaryKey && result.EndsWith(" I D", StringComparison.OrdinalIgnoreCase))
                {
                    result = result.Substring(0, result.Length - 4).Trim();
                }

                // Split the spaced string into individual words
                string[] words = result.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Apply abbreviations to any word that matches
                for (int i = 0; i < words.Length; i++)
                {
                    if (_friendlyAbbreviations.TryGetValue(words[i], out string abbr))
                    {
                        words[i] = abbr;
                    }
                }

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


        // ========================================================================
        // 5. SECURITY HELPERS (Optimized)
        // ========================================================================

        private static readonly HashSet<string> _PasswordKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "password", "passwd", "pwd", "salt", "hash"
        };

        public bool IsHoldPassword
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;

                // 1. METADATA OVERRIDE: The ultimate source of truth
                if (ExtendedProperties.TryGetValue("IsPassword", out string extProp))
                    return extProp.ToLower() == "true" || extProp == "1";

                // 2. FALLBACK: Contains match handles both exact ("password") and partial ("UserPassword")
                foreach (string keyword in _PasswordKeywords)
                {
                    if (Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }

                return false;
            }
        }

        // ========================================================================
        //  SEARCH & FILTER HELPERS
        // ========================================================================
        public Dictionary<string, string> ExtendedProperties { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // ========================================================================
        // 7. ENUM & DROPDOWN HELPERS
        // ========================================================================

        // Upgraded to IReadOnlyList to prevent accidental modification by T4 templates
        public IReadOnlyList<string> AllowedFilterValues
        {
            get
            {
                var list = new List<string>();

                // 1. Metadata Override
                if (ExtendedProperties.TryGetValue("AllowedValues", out string vals))
                {
                    list.AddRange(vals.Split(',').Select(s => s.Trim()));
                    return list;
                }

                // 2. Boolean inference
                if (CSharpType == "bool")
                {
                    list.Add("True");
                    list.Add("False");
                }

                return list;
            }
        }

        public bool HasAllowedValues => AllowedFilterValues.Count > 0;

        // STATIC ALLOCATION: Array built once, saving huge amounts of RAM in T4 loops
        private static readonly string[] _searchableKeywords =
        {
            "name", "email", "phone", "number", "no",
            "title", "code", "barcode", "username", "license"
        };

        public bool IsSearchable
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return false;

                // 1. METADATA OVERRIDE
                if (ExtendedProperties.TryGetValue("IsSearchable", out string extProp))
                {
                    if (extProp.ToLower() == "true" || extProp == "1") return true;
                    if (extProp.ToLower() == "false" || extProp == "0") return false;
                }

                // 2. HARD CONSTRAINTS
                if (CSharpType == "bool" || CSharpType == "DateTime" || CSharpType == "byte[]") return false;
                if (SqlType.Equals("varchar", StringComparison.OrdinalIgnoreCase) && MaxLength == -1) return false;
                if (Name.IndexOf("created", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    Name.IndexOf("modified", StringComparison.OrdinalIgnoreCase) >= 0) return false;

                // 3. AUTOMATIC INCLUSIONS
                if (IsPrimaryKey) return true;

                // 4. SMART HEURISTICS (Using the static array)
                string lowerName = Name.ToLower();
                foreach (var keyword in _searchableKeywords)
                {
                    if (lowerName.Contains(keyword)) return true;
                }

                return false;
            }
        }

        // ========================================================================
        // 8. UNIQUE SELECTOR LOGIC (For ctrlSelectorCard Generation)
        // ========================================================================

        /// <summary>
        /// Defines if this column can be used to uniquely find a record (e.g., PK, NationalNo, Email).
        /// </summary>
        public bool IsUniqueSelector
        {
            get
            {
                // 1. Base logic: The Primary Key is ALWAYS a unique selector.
                if (IsPrimaryKey) return true;

                // 2. Metadata Override: The DBA tags unique constraints (like NationalNo)
                if (ExtendedProperties.TryGetValue("IsUniqueSelector", out string extProp))
                {
                    return extProp.ToLower() == "true" || extProp == "1";
                }

                // Default to false. We don't want people doing exact-match lookups on "FirstName"
                return false;
            }
        }

        // 1. Map SQL -> C# Type
        public string CSharpType
        {
            get
            {
                string t = SqlType.ToLower();
                string cSharp = t switch
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

                // Add nullable modifier if required and valid for the type
                if (IsNullable && cSharp != "string" && cSharp != "byte[]" && cSharp != "object")
                    return cSharp + "?";

                return cSharp;
            }
        }

        // ========================================================================
        // UI CONTROL & ICON GENERATION (Metadata-First)
        // ========================================================================

        // 1. Map SQL -> Windows Forms Control
        public string WinControl
        {
            get
            {
                // OVERRIDE: Did the DB Architect force a specific control?
                if (ExtendedProperties.TryGetValue("ControlType", out string ctrlType))
                    return ctrlType;

                // FALLBACK: Guess based on SQL Type
                string t = SqlType.ToLower();
                if (t == "bit") return "CheckBox";
                if (t.Contains("date")) return "DateTimePicker";
                if (t.Contains("varchar") && MaxLength > 255) return "RichTextBox"; // Smart Multi-line

                return "TextBox";
            }
        }

        // 2. Map Control -> UI Prefix (Fixed syntax and logic)
        public string ControlNameWithPrefix
        {
            get
            {
                string prefix = WinControl switch
                {
                    "CheckBox" => "chk",
                    "DateTimePicker" => "dtp",
                    "RichTextBox" => "rtb",
                    "ComboBox" => "cb",
                    "NumericUpDown" => "nud",
                    _ => "txt"
                };

                return prefix + Name;
            }
        }

        // 3. Map Name -> UI Icon (Segoe MDL2 Assets)
        public string IconCode
        {
            get
            {
                // OVERRIDE: Did the DB Architect define a specific Unicode icon?
                if (ExtendedProperties.TryGetValue("IconCode", out string extIcon))
                    return extIcon;

                // FALLBACK: Guess by column name
                string n = Name.ToLower();
                if (n.Contains("email") || n.Contains("mail")) return "\\uE715"; // Envelope
                if (n.Contains("phone") || n.Contains("mobile")) return "\\uE717"; // Phone
                if (n.Contains("pass")) return "\\uE72E"; // Lock
                if (n.Contains("user") || n.Contains("name")) return "\\uE77B"; // User Person
                if (n.Contains("address") || n.Contains("city")) return "\\uE816"; // Map Pin
                if (n.Contains("date") || n.Contains("time")) return "\\uE787"; // Calendar
                if (n.Contains("money") || n.Contains("price")) return "\\uE8C7"; // Dollar
                if (IsPrimaryKey || n.Contains("id")) return "\\uE13D"; // Key

                return "\\uE968"; // Default: List Item icon
            }
        }

        public bool IsLookupColumn
        {
            get
            {
                // OVERRIDE: Explicitly tag this as a lookup column in the DB
                if (ExtendedProperties.TryGetValue("IsLookupColumn", out string extProp))
                    return extProp.ToLower() == "true" || extProp == "1";

                // FALLBACK: If column is NOT a primary key but sounds like a unique ID
                string n = Name.ToLower();
                return !IsPrimaryKey && (n.Contains("name") || n.Contains("email") || n.Contains("number"));
            }
        }

        public string LabelName => "lbl" + Name;    // e.g., lblUsername

    }
}

using CodeGeneratorSolution;
using CodeGeneratorSolution.Core;
using CodeGeneratorSolution.Infrastructure.Interfaces;
using CodeGeneratorSolution.Models;
using CodeGeneratorSolution.Templates;
using CodeGeneratorSolution.Templates.Application.Interfaces;
using CodeGeneratorSolution.Templates.Application.Services;
using CodeGeneratorSolution.Templates.Application.Validators;
using CodeGeneratorSolution.Templates.Core.DTOs;
using CodeGeneratorSolution.Templates.Core.Entities;
using CodeGeneratorSolution.Templates.Core.Mapping;
using CodeGeneratorSolution.Templates.Infrastructure.Repositories;
using CodeGeneratorSolution.Templates.UI;
using CodeGeneratorSolution.Templates.UI.Controls;
using CodeGeneratorSolution.Templates.UI.DependencyInjection;
using CodeGeneratorSolution.Templates.UI.Helpers;
using CodeGeneratorSolution.Utils;
using CodeGeneratorSolution.Utlis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

public partial class GeneratorEngine
{
    private readonly string _outputRootDir;
    private readonly string _solutionName;
    private readonly string _connStr;
    private readonly MetadataFetcher _fetcher;
    private readonly EmbeddedResourceManager _embeddedResourceManager;

    public GeneratorEngine(string connStr, string outputRootDirectory, string solutionName)
    {
        _connStr = connStr;
        _outputRootDir = outputRootDirectory;
        _solutionName = solutionName;
        _fetcher = new MetadataFetcher(_connStr);
        _embeddedResourceManager = new EmbeddedResourceManager(solutionName);
    }

    public async Task GenerateSolutionAsync()
    {
        Console.WriteLine("🚀 Starting Enterprise Code Generator...");

        GenerateStaticFiles();
        await GenerateAllDynamicFilesAsync();

        Console.WriteLine("✅ Solution Generated Successfully!");
    }

    public async Task GenerateAllDynamicFilesAsync()
    {
        Console.WriteLine("📡 Fetching Metadata from SQL Server...");
        List<TableSchema> tables = await _fetcher.GetTablesAsync();

        // استبعاد جداول النظام الداخلية
        tables = tables.Where(t => t.Name != "UI_ColumnMetadata" && t.Name != "sysdiagrams").ToList();

        Console.WriteLine($"⚙️ Generating Code for {tables.Count} Tables (Parallel Execution)...");

        // =====================================================================
        // 1. التوليد المتوازي الحقيقي (True Parallel Generation)
        // =====================================================================
        var generationTasks = tables.Select(table => Task.Run(() =>
        {
            string entityName = table.EntityName;

            // --- Database ---
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Database.StoredProcedures.Generated, $"usp_Base_{entityName}.sql",
                new SqlStoreProcedureTemplate { Table = table }.TransformText());

            // --- Core Layer (Entities, DTOs, Mappings) ---
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Core.Entities.Generated, $"{entityName}.g.cs",
                new EntityTemplate { Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Core.DTOs.Generated, $"{entityName}DTOs.g.cs",
                new DTOsTemplate { Table = table }.TransformText());

            // تم تصحيح المسار ليكون في طبقة Core بدلاً من Application
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Core.Mapping, $"{entityName}Mapper.g.cs",
                new MappingExtensionTemplate { Table = table }.TransformText());

            // --- Infrastructure Layer (Repositories) ---
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Infrastructure.Interfaces.Generated, $"I{entityName}Repository.g.cs",
                new IEntityRepositoryTemplate { Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Infrastructure.Repositories.Generated, $"{entityName}Repository.g.cs",
                new RepositoryTemplate { Table = table }.TransformText());

            // --- Application Layer (Services & Validators) ---
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Validators.Generated, $"{entityName}Validator.g.cs",
                new DtoValidatorTemplate { Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Interfaces.Generated, $"I{entityName}Service.g.cs",
                new IEntityServiceTemplate { Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Services.Generated, $"{entityName}Service.g.cs",
                new EntityServiceTemplate { Table = table }.TransformText());

            // --- UI Layer (Controls) ---
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls.Generated, $"ctrl{entityName}AddEdit.g.cs",
                new ctrlAddEdit { Table = table }.TransformText());

            // بناءً على ملاحظتك المعمارية: تم استعادة توليد ملف الـ Designer 
            // لكي يحتوي على InitializeComponent وإدارة الذاكرة IContainer وتنسيق الـ Auto-Layout
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls.Generated, $"ctrl{entityName}AddEdit.g.Designer.cs",
               new ctrlAddEditDesigner { Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls.Generated, $"ctrl{entityName}ShowDetails.g.cs",
                new ctrlShowDetails { Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls.Generated, $"ctrl{entityName}ShowDetails.g.Designer.cs",
                new ctrlShowDetailsControlDesigner { Table = table }.TransformText());
        }));

        // انتظار انتهاء جميع المهام (أداء صاروخي)
        await Task.WhenAll(generationTasks);

        Console.WriteLine("🧩 Generating Aggregate System Files...");

        // =====================================================================
        // 2. ملفات النظام التجميعية (Aggregate Files)
        // =====================================================================
        // UI Helpers (Icons)
        FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Helpers, $"AppIcons.g.cs",
            new AppIcons { Tables = tables }.TransformText());

        // Routing & View Registry
        FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Routing, $"ViewRegistry.g.cs",
            new ViewRegistryTemplate { Tables = tables }.TransformText());

        // Auto Dependency Injection
        FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.DependencyInjection, $"DependencyInjectionSetup.g.cs",
            new DependencyInjectionTemplate { Tables = tables }.TransformText());


        // =====================================================================
        // 3. التنفيذ الآلي في قاعدة البيانات (Executing SQL Scripts)
        // =====================================================================
        Console.WriteLine("💾 Deploying Generated Stored Procedures to Database...");

        // تم تصحيح المسار ليتوجه إلى مجلد الـ Generated الفعلي
        string sqlGeneratedFolder = Path.Combine(_outputRootDir, ProjectManifest.Database.StoredProcedures.Generated);
        if (Directory.Exists(sqlGeneratedFolder))
        {
            await ScriptExecutor.RunAllScriptsInFolder(sqlGeneratedFolder, _connStr);
        }
    }

    public void GenerateStaticFiles()
    {
        Console.WriteLine("📦 Extracting Static Core Framework...");
        _embeddedResourceManager.Extract(targetOutputRoot: _outputRootDir);
    }

    // (دالة GetContent تم تركها كما هي لاستخدامها داخلياً في بعض الـ Helpers إذا لزم الأمر)
    private string GetContent(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"CodeGeneratorSolution.Templates.{fileName}";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                var available = string.Join(", ", assembly.GetManifestResourceNames());
                throw new FileNotFoundException($"Resource '{resourceName}' not found. \nAvailable: {available}");
            }
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
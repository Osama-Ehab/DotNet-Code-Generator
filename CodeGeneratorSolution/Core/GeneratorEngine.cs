using CodeGeneratorSolution;
using CodeGeneratorSolution.Core;
using CodeGeneratorSolution.Models;
using CodeGeneratorSolution.Templetes.T4;
using CodeGeneratorSolution.Utils;
using CodeGeneratorSolution.Utlis;
using System.Reflection;
using System.Windows.Forms.Design;


public partial class GeneratorEngine
{
    private string _rootPath;
    private string _outputDir;
    private string _solutionName;
    private string _connStr;
    private MetadataFetcher _fetcher;
    public GeneratorEngine(string ConnStr, string outputDir, string solutionName)
    {
        _connStr = ConnStr;
        _fetcher = new MetadataFetcher(_connStr);
        _outputDir = outputDir;
        _solutionName = solutionName;
    }

 

    public async Task GenerateSolutionAsync()
    {
        GenerateEmbeddedCoreLayer();
        GenerateEmbeddedInfrastructureLayer();
        GenerateEmbeddedApplicationLayer();
        GenerateEmbeddedUiLayer();
        await GenerateAllDynamicFilesAsync();
    }

 
    
    public async Task GenerateAllDynamicFilesAsync()
    {
        FileWriter.InitializeOutputDirectories(_outputDir, _solutionName);
        List<TableSchema> tables = await _fetcher.GetTablesAsync();
        // High Performance: Generate all files in parallel
        foreach (var table in tables)
        {
            var Columns = table.Columns; // Fetch Metadata
            string ClassName = table.ClassName;
            // 0. Generate SQL Stored Procedures Script
            FileWriter.WriteFile(_outputDir, "SqlScripts", $"{ClassName}StoredProsedures.sql",
            new SqlStoreProcedureTemplate { SolutionName = _solutionName, Table = table }.TransformText());

            // 1. Model & DTOs
            FileWriter.WriteFile(_outputDir, "Core/Entities", $"{ClassName}Entity.Generated.cs",
                new EntityTemplate { SolutionName = _solutionName, Table = table }.TransformText());
     
            FileWriter.WriteFile(_outputDir, "Core/DTOs", $"{ClassName}DTOs.Generated.cs",
              new IEntityServiceTemplate { SolutionName = _solutionName, Table = table }.TransformText());
  
            FileWriter.WriteFile(_outputDir, "Core/Mapping", $"{ClassName}MappingExtensions.Generated.cs",
                new MappingExtensionTemplate { SolutionName = _solutionName, Table = table }.TransformText());

            // 2. Repository (Async + SP)
            FileWriter.WriteFile(_outputDir, "Infrastructure/Generated", $"{ClassName}Repository.Generated.cs",
                new RepositoryTemplate { SolutionName = _solutionName, Table = table }.TransformText());
            FileWriter.WriteFile(_outputDir, "Infrastructure/Generated", $"I{ClassName}Repository.Generated.cs",
                new RepositoryInterfaceTemplate { SolutionName = _solutionName, Table = table }.TransformText());

            // 3. Service (Logic)
            FileWriter.WriteFile(_outputDir, "Application/Validators", $"{ClassName}DTOValidators.Generated.cs",
                new DtoValidatorTemplate { SolutionName = _solutionName, Table = table }.TransformText());
      
            FileWriter.WriteFile(_outputDir, "Application", $"{table}Service.Generated.cs",
                new ServiceTemplate { SolutionName = _solutionName, Table = table }.TransformText());

            FileWriter.WriteFile(_outputDir, "Application", $"{table}DataMapper.Generated.cs",
                new DataMapperTemplate { SolutionName = _solutionName, Table = table }.TransformText());


            // UI (control)          
            FileWriter.WriteFile(_outputDir, "UI/Controls/Generated", $"ctrl{ClassName}.Generated.cs",
                new ctrlAddEdit { SolutionName = _solutionName, Table = table }.TransformText());
            FileWriter.WriteFile(_outputDir, "UI/Controls/Generated", $"ctrl{ClassName}.Designer.Generated.cs",
                new ctrlAddEditDesigner { SolutionName = _solutionName, Table = table }.TransformText());

            // 4. UI (Forms)      
            FileWriter.WriteFile(_outputDir, "UI/Controls/Generated", $"ctrl{ClassName}.Generated.cs",
                new ctrlList { SolutionName = _solutionName, Table = table }.TransformText());
            FileWriter.WriteFile(_outputDir, "UI/Controls/Generated", $"ctrl{ClassName}.Designer.Generated.cs",
                new ctrlListDesigner { SolutionName = _solutionName, Table = table }.TransformText());

            // 4. UI (control)      
            FileWriter.WriteFile(_outputDir, "UI/Controls/Generated", $"ctrl{ClassName}.Generated.cs",
                new ctrlAddEdit { SolutionName = _solutionName, Table = table }.TransformText());
            FileWriter.WriteFile(_outputDir, "UI/Controls/Generated", $"ctrl{ClassName}.Designer.Generated.cs",
                new ctrlAddEditDesigner { SolutionName = _solutionName, Table = table }.TransformText());


        }


        FileWriter.WriteFile(_outputDir, "UI", $"WindowsFormsNavigationService.Generated.cs",
            new WindowsFormsNavigationServiceTemplate { SolutionName = _solutionName, Tables = tables }.TransformText());
        
        FileWriter.WriteFile(_outputDir, "UI", $"DependencyInjection.Generated.cs",
            new DependencyInjectionTemplate { SolutionName = _solutionName, Tables = tables }.TransformText());


        await ScriptExecutor.RunAllScriptsInFolder(Path.Combine(_outputDir, "SqlScripts"), _connStr);

    }



    public void GenerateEntryPoint(string mainFormName)
    {
        string folder = "ProjectFiles";

        string safeConnString = _connStr.Replace("\\", "\\\\");
        // 1. Define the Context (Your "Switch Case" Parameters)
        var context = new Dictionary<string, string>
    {
        { "{{SolutionName}}", _solutionName },
        { "{{Namespace}}", $"{_solutionName}.ProjectFiles" },
        { "{{MainForm}}",mainFormName },
        { "{{ConnectionString}}",safeConnString }
    };

        // 2. Extract Entire Infrastructure Folder
        // Source: MyEnterpriseGenerator/Templates/Infrastructure
        // Target: C:/Output/MyDVLD.Infrastructure

        EmbeddedResourceManager.ExtractAll(
            resourcePrefix: "CodeGeneratorSolution.Templetes.ProjectFiles",
            outputRoot: Path.Combine(_outputDir, folder),
            replacements: context
        );
        Console.WriteLine("✅ Generated Program.cs (from Template)");
    }
    private string GetEmbeddedContent(string fileName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // IMPORTANT: The resource name is "DefaultNamespace.Folder.Filename"
        // Adjust "CodeGeneratorSolution" to match your Project's Default Namespace.
        string resourceName = $"CodeGeneratorSolution.Templates.{fileName}";

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
                return reader.ReadToEnd();
            }
        }
    }

    public void GenerateEmbeddedInfrastructureLayer()
    {
        // 1. Define the Context (Your "Switch Case" Parameters)
        var context = new Dictionary<string, string>
    {
        { "{{SolutionName}}", _solutionName },
        { "{{Namespace}}", $"{_solutionName}.Infrastructure" },
        { "{{ConnectionString}}",_connStr }
    };

        // 2. Extract Entire Infrastructure Folder
        // Source: MyEnterpriseGenerator/Templates/Infrastructure
        // Target: C:/Output/MyDVLD.Infrastructure

        EmbeddedResourceManager.ExtractAll(
            resourcePrefix: "CodeGeneratorSolution.EmbeddedResources.Infrastructure",
            outputRoot: Path.Combine(_outputDir, $"Infrastructure"),
            replacements: context
        );

        Console.WriteLine("✅ Infrastructure Layer Generated.");
    }
    public void GenerateEmbeddedCoreLayer()
    {
        // 1. Define the Context (Your "Switch Case" Parameters)
        var context = new Dictionary<string, string>
    {
        { "{{SolutionName}}", _solutionName },
        { "{{Namespace}}", $"{_solutionName}.Core" },
        { "{{ConnectionString}}",_connStr }
    };

        // 2. Extract Entire Infrastructure Folder
        // Source: MyEnterpriseGenerator/Templates/Infrastructure
        // Target: C:/Output/MyDVLD.Infrastructure

        EmbeddedResourceManager.ExtractAll(
            resourcePrefix: "CodeGeneratorSolution.EmbeddedResources.Core",
            outputRoot: Path.Combine(_outputDir, $"Core"),
            replacements: context
        );

        Console.WriteLine("✅ Core Layer Generated.");
    }
    public void GenerateEmbeddedApplicationLayer()
    {
        // 1. Define the Context (Your "Switch Case" Parameters)
        var context = new Dictionary<string, string>
    {
        { "{{SolutionName}}", _solutionName },
        { "{{Namespace}}", $"{_solutionName}.Application" },
        { "{{ConnectionString}}",_connStr }
    };

        // 2. Extract Entire Infrastructure Folder
        // Source: MyEnterpriseGenerator/Templates/Infrastructure
        // Target: C:/Output/MyDVLD.Infrastructure

        EmbeddedResourceManager.ExtractAll(
            resourcePrefix: "CodeGeneratorSolution.EmbeddedResources.Application",
            outputRoot: Path.Combine(_outputDir, $"Application"),
            replacements: context
        );

        Console.WriteLine("✅ Application Layer Generated.");
    }
    public void GenerateEmbeddedUiLayer()
    {
        // 1. Define the Context (Your "Switch Case" Parameters)
        var context = new Dictionary<string, string>
    {
        { "{{SolutionName}}", _solutionName },
        { "{{Namespace}}", $"{_solutionName}.UI" },
        { "{{ConnectionString}}",_connStr }
    };

        // 2. Extract Entire Infrastructure Folder
        // Source: MyEnterpriseGenerator/Templates/Infrastructure
        // Target: C:/Output/MyDVLD.Infrastructure

        EmbeddedResourceManager.ExtractAll(
            resourcePrefix: "CodeGeneratorSolution.EmbeddedResources.UI",
            outputRoot: Path.Combine(_outputDir, $"UI"),
            replacements: context
        );

        Console.WriteLine("✅ UI Layer Generated.");
    }
    // ... Implement GetTables() and GetTable.Columns() using ADO.NET here ...
}
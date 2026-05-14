
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
using CodeGeneratorSolution.Templates.Infrastructure.Mapping;
using CodeGeneratorSolution.Templates.Infrastructure.Repositories;
using CodeGeneratorSolution.Templates.UI;
using CodeGeneratorSolution.Templates.UI.Controls;
using CodeGeneratorSolution.Templates.UI.DependencyInjection;
using CodeGeneratorSolution.Utils;
using CodeGeneratorSolution.Utlis;
using System.Reflection;


public partial class GeneratorEngine
{
    private string _outputRootDir;
    private string _solutionName;
    private string _connStr;
    private MetadataFetcher _fetcher;
    private EmbeddedResourceManager _embeddedResourceManager;
    private Dictionary<string, string> projectFileMap;
    public GeneratorEngine(string ConnStr, string outputRootDirectory, string solutionName)
    {
        _connStr = ConnStr;
        _fetcher = new MetadataFetcher(_connStr);
        _outputRootDir = outputRootDirectory;
        _solutionName = solutionName;
        _embeddedResourceManager = new EmbeddedResourceManager(solutionName);
    }

 

    public async Task GenerateSolutionAsync()
    {
        GenerateStaticFiles();
        await GenerateAllDynamicFilesAsync();
    }

 
    
    public async Task GenerateAllDynamicFilesAsync()
    {
        FileWriter.InitializeOutputDirectories(_outputRootDir, _solutionName);
        List<TableSchema> tables = await _fetcher.GetTablesAsync();
        // High Performance: Generate all files in parallel
        foreach (var table in tables)
        {
            var Columns = table.Columns; // Fetch Metadata
            string ClassName = table.ClassName;
            // 0. Generate SQL Stored Procedures Script
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.SqlScripts, $"{ClassName}StoredProsedures.sql",
             new SqlStoreProcedureTemplate {Table = table }.TransformText());

            // 1. Model & DTOs
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Core.Entities.Root, $"{ClassName}Entity.g.cs",
                new EntityTemplate {  Table = table }.TransformText());
     
  
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Core.DTOs, $"{ClassName}DTOs.g.cs",
              new DTOsTemplate {  Table = table }.TransformText());
  
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Core.Mapping, $"{ClassName}MappingExtensions.g.cs",
                new MappingExtensionTemplate {  Table = table }.TransformText());

            // 2. Repository (Async + SP)
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Infrastructure.Root, $"{ClassName}Repository.g.cs",
                new RepositoryTemplate {  Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Infrastructure.Interfaces, $"I{ClassName}Repository.g.cs",
                new IEntityRepositoryTemplate {  Table = table }.TransformText());

            // 3. Service (Logic)
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Validators, $"{ClassName}DTOValidators.g.cs",
                new DtoValidatorTemplate {  Table = table }.TransformText());
      
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Services, $"{ClassName}Service.g.cs",
                new EntityServiceTemplate {  Table = table }.TransformText());
            
            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Interfaces, $"I{ClassName}Service.g.cs",
                new IEntityServiceTemplate {  Table = table }.TransformText());

            FileWriter.WriteFile(_outputRootDir, ProjectManifest.Application.Root, $"{ClassName}DataMapper.g.cs",
                new DataMapperTemplate {  Table = table }.TransformText());


            //// UI (control)          
            //FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}AddEdit.g.cs",
            //    new ctrlAddEdit {  Table = table }.TransformText());                          
            //FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}AddEdit.g.Designer.cs",
            //    new ctrlAddEditDesigner {  Table = table }.TransformText());

            //// UI (control)          
            //FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}AddEdit.g.cs",
            //    new ctrlAddEdit {  Table = table }.TransformText());                          
            //FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}AddEdit.g.Designer.cs",
            //    new ctrlAddEditDesigner {  Table = table }.TransformText());

            //// UI (control)          
            //FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}ShowDetails.g.cs",
            //    new ctrlShowDetails {  Table = table }.TransformText());                              
            //FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}ShowDetails.g.Designer.cs",
            //    new ctrlShowDetailsDesigner {  Table = table }.TransformText());

            //if(table.GenerateSelectorControl) 
            //{
                
            //    FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}SelectorCard.g.cs",
            //        new ctrlSelectorCard { Table = table }.TransformText());
            //    FileWriter.WriteFile(_outputRootDir, ProjectManifest.UI.Controls, $"ctrl{ClassName}SelectorCard.g.Designer.cs",
            //        new ctrlSelectorCardDesigner { Table = table }.TransformText());

            //}
           
           


        }



        //FileWriter.WriteFile(_outputRootDir, "UI", $"ViewRegistry.g.cs",
        //    new ViewRegistryTemplate { Tables = tables }.TransformText());
        
        //FileWriter.WriteFile(_outputRootDir, "UI", $"DependencyInjection.g.cs",
        //    new DependencyInjectionTemplate {  Tables = tables }.TransformText());


        await ScriptExecutor.RunAllScriptsInFolder(Path.Combine(_outputRootDir, "SqlScripts"), _connStr);

    }



    public void GenerateStaticFiles()
    {
        _embeddedResourceManager.Extract(targetOutputRoot: _outputRootDir);
        Console.WriteLine("✅ Generated All Static Files");
    }
    private string GetContent(string fileName)
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
}
using System.IO;

namespace CodeGeneratorSolution.Core
{
    public static class ProjectManifest
    {
        // الكلمة الأساسية التي تسبق كل المشاريع
        public const string SolutionName = "SchoolERP";

        // ==========================================
        // 0. DATABASE SCRIPTING
        // ==========================================
        public static class Database
        {
            public const string Root = "SqlScripts";

            public static class StoredProcedures
            {
                public static readonly string Root = Path.Combine(Database.Root, "StoredProcedures");
                public static readonly string Generated = Path.Combine(Root, "Generated"); // usp_Base_...
                public static readonly string Custom = Path.Combine(Root, "Custom");       // usp_Custom_...
            }

            public static class Tables
            {
                public static readonly string Root = Path.Combine(Database.Root, "Tables");
            }
        }

        // ==========================================
        // 1. CORE LAYER (Entities & DTOs)
        // ==========================================
        public static class Core
        {
            public const string ProjectName = $"{SolutionName}.Core";
            public static readonly string Root = ProjectName;

            public static class Entities
            {
                public static readonly string Root = Path.Combine(ProjectName, "Entities");
                public static readonly string Base = Path.Combine(Root, "Base");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            public static class DTOs
            {
                public static readonly string Root = Path.Combine(ProjectName, "DTOs");
                public static readonly string Base = Path.Combine(Root, "Base");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            public static class Interfaces
            {
                public static readonly string Root = Path.Combine(ProjectName, "Interfaces");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            public static readonly string Mapping = Path.Combine(ProjectName, "Mapping");
        }

        // ==========================================
        // 2. APPLICATION LAYER (Business Logic)
        // ==========================================
        public static class Application
        {
            public const string ProjectName = $"{SolutionName}.Application";
            public static readonly string Root = ProjectName;
            public static readonly string Base = Path.Combine(ProjectName, "Base");
            public static readonly string Security = Path.Combine(ProjectName, "Security");
            public static readonly string Mapping = Path.Combine(ProjectName, "Mapping");

            public static class Interfaces
            {
                public static readonly string Root = Path.Combine(ProjectName, "Interfaces");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            public static class Validators
            {
                public static readonly string Root = Path.Combine(ProjectName, "Validators");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            public static class Services
            {
                public static readonly string Root = Path.Combine(ProjectName, "Services");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }
        }

        // ==========================================
        // 3. INFRASTRUCTURE LAYER (Data Access)
        // ==========================================
        public static class Infrastructure
        {
            public const string ProjectName = $"{SolutionName}.Infrastructure";
            public static readonly string Root = ProjectName;
            public static readonly string Base = Path.Combine(ProjectName, "Base");
            public static readonly string Enums = Path.Combine(ProjectName, "Enums");
            public static readonly string Extensions = Path.Combine(ProjectName, "Extensions");
            public static readonly string Mapping = Path.Combine(ProjectName, "Mapping");
            public static readonly string Utilities = Path.Combine(ProjectName, "Utilities");

            public static class Interfaces
            {
                public static readonly string Root = Path.Combine(ProjectName, "Interfaces");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            // تمت إضافة Repositories لأنها قلب طبقة البنية التحتية
            public static class Repositories
            {
                public static readonly string Root = Path.Combine(ProjectName, "Repositories");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }
        }

        // ==========================================
        // 4. PRESENTATION LAYER (WinForms UI)
        // ==========================================
        public static class UI
        {
            public const string ProjectName = $"{SolutionName}.UI";
            public static readonly string Root = ProjectName;

            public static readonly string CustomControls = Path.Combine(ProjectName, "CustomControls");
            public static readonly string Events = Path.Combine(ProjectName, "Events");
            public static readonly string GenericForms = Path.Combine(ProjectName, "GenericForms");
            public static readonly string Helpers = Path.Combine(ProjectName, "Helpers");
            public static readonly string DependencyInjection = Path.Combine(ProjectName, "DependencyInjection");
            public static readonly string Routing = Path.Combine(ProjectName, "Routing");
            public static readonly string ProjectFiles = ProjectName;

            public static class Controls
            {
                public static readonly string Root = Path.Combine(ProjectName, "Controls");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            public static class Interfaces
            {
                public static readonly string Root = Path.Combine(ProjectName, "Interfaces");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }

            // يفضل إضافة مسار للفورمز المستقلة
            public static class Forms
            {
                public static readonly string Root = Path.Combine(ProjectName, "Forms");
                public static readonly string Generated = Path.Combine(Root, "Generated");
                public static readonly string Custom = Path.Combine(Root, "Custom");
            }
        }
    }
}
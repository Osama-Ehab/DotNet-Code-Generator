using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Core
{
    public static class ProjectManifest
    {
        // ==========================================
        // 0. Database
        // ==========================================
        // Database
        public const string SqlScripts = "SqlScripts";

        public const string Root = "";

        // ==========================================
        // 1. CORE LAYER (Entities & DTOs)
        // ==========================================
        public static class Core
        {
            public const string Root = "Core";
            public static class Entities
            {
                public const string Root = "Core/Entities";
                public const string Base = "Core/Entities/Base";
              
            }
            public const string DTOs = "Core/DTOs";
            public const string Mapping = "Core/Mapping";
            public const string DtoInterfaces = "Core/DTOs/Interfaces";
        }


        // ==========================================
        // 2. APPLICATION LAYER (Business Logic)
        // ==========================================
        public static class Application
        {
            public const string Root = "Application";
            public const string Base = "Application/Base";
            public const string Interfaces = "Application/Interfaces";
            public const string Security = "Application/Security";
            public const string Validators = "Application/Validators";
            public const string Services = "Application/Services";
        }

        // ==========================================
        // 3. INFRASTRUCTURE LAYER (Data Access)
        // ==========================================
        public static class Infrastructure
        {
            public const string Root = "Infrastructure";
            public const string Base = "Infrastructure/Base";
            public const string Enums = "Infrastructure/Enums";
            public const string Extensions = "Infrastructure/Extensions";
            public const string Interfaces = "Infrastructure/Interfaces";
            public const string Utilities = "Infrastructure/Utilities";
        }

        // ==========================================
        // 4. PRESENTATION LAYER (WinForms UI)
        // ==========================================
        public static class UI
        {
            public const string Root = "UI";
            public const string CustomControls = "UI/CustomControls";
            public const string Controls = "UI/Controls";
            public const string Events = "UI/Events";
            public const string GenericForms = "UI/GenericForms";
            public const string Helpers = "UI/Helpers";
            public const string Interfaces = "UI/Interfaces";

            // Project configuration files usually sit at the root of the UI project
            public const string ProjectFiles = "UI";
        }
    }
}

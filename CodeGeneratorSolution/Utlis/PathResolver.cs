using CodeGeneratorSolution.Core;

namespace CodeGeneratorSolution.Utlis
{
    public class PathResolver
    {
        private readonly string _targetSolutionName;
        private readonly Dictionary<string, string> _projectFileMap;

        public PathResolver(string targetSolutionName)
        {
            _targetSolutionName = targetSolutionName;

            // The dictionary lives here, isolated from the rest of the app
            _projectFileMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Program.txt", "Program.cs" },
            { ".csproj", $"{_targetSolutionName}.csproj" }
        };
        }


        public (string FolderPath, string FileName) ResolvePath(string relativeDottedPath)
        {
            string folderPath = "";
            string fileName = "";


            // 2. We need to handle the ".Designer.cs" and ".cs" double-dot problem safely
            // Find the index of the ACTUAL file extension.
            int extensionDotIndex = relativeDottedPath.EndsWith(".Designer.cs")
                ? relativeDottedPath.LastIndexOf(".Designer.cs")
                : relativeDottedPath.LastIndexOf('.');


            // Find the second-to-last dot (which separates the folder from the filename)
            int fileNameDotIndex = extensionDotIndex == 0 ? -1 : relativeDottedPath.LastIndexOf('.', extensionDotIndex - 1);


            if (fileNameDotIndex == -1)
                return (null,relativeDottedPath); 

            // Extract the folders part: "DVLD.Application.Validators"
            folderPath = relativeDottedPath.Substring(0, fileNameDotIndex);

            // Extract the file part: "BaseValidator.cs"
            fileName = relativeDottedPath.Substring(fileNameDotIndex + 1);



            // Apply special rules for UI Root
            if (folderPath == @"UI.ProjectFiles")
            {
                folderPath = ProjectManifest.Root;

                if (_projectFileMap.TryGetValue(fileName, out string mappedName))
                {
                    fileName = mappedName;
                }
            }

            // Convert the folder dots to directory separators
            string physicalFolders = folderPath.Replace('.', Path.DirectorySeparatorChar);

            return (physicalFolders, fileName);
        }
    }
}

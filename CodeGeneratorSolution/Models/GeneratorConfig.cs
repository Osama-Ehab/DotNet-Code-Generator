using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Models
{
    public class GeneratorConfig
    {
        public string SolutionName { get; set; } = string.Empty;
        public string OutputRootDirectory { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string ResourcesPath { get; set; } = string.Empty;
    }
}

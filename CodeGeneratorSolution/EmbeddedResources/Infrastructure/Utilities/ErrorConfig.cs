using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGeneratorSolution.Templetes.Infrastructure.Utilities
{
    public class ErrorConfig
    {
        public string? Type { get; set; }
        public string? DefaultMessage { get; set; }
        public List<ErrorContext> Contexts { get; set; } = new List<ErrorContext>();
    }

    public class ErrorContext
    {
        public string? Contains { get; set; } // The text to look for (e.g., "DELETE statement")
        public string? Message { get; set; }  // The specific message to show
    }
}

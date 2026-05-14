
namespace CodeGeneratorSolution.Utlis
{
    public class ContentProcessor
    {
        private readonly string _targetSolutionName;

        public ContentProcessor(string targetSolutionName)
        {
            _targetSolutionName = targetSolutionName;
        }

        public string Process(string rawContent)
        {
            if (rawContent != null && rawContent.Contains("{{TARGET_NAMESPACE}}"))
            {
                return rawContent.Replace("{{TARGET_NAMESPACE}}", _targetSolutionName);
            }

            return rawContent;
        }
    }
}

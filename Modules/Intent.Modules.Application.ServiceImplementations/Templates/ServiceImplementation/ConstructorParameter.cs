using Intent.Modules.Common;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public class ConstructorParameter
    {
        public ConstructorParameter(string type, string name, ITemplateDependency templateDependency)
        {
            ParameterType = type;
            ParameterName = name;
            TemplateDependency = templateDependency;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }
        public ITemplateDependency TemplateDependency { get; }
    }
}
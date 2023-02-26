using System.Text;
using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Dapr.AspNetCore
{
    public static class ExtensionMethods
    {
        public static string GetDaprApplicationName(this ISoftwareFactoryExecutionContext executionContext, string applicationId)
        {
            return GetDaprName(executionContext.GetApplicationConfig(applicationId).Name);
        }

        public static string GetDaprSolutionName(this ISoftwareFactoryExecutionContext executionContext)
        {
            return GetDaprName(executionContext.GetSolutionConfig().SolutionName);
        }

        private static string GetDaprName(string name)
        {
            var sb = new StringBuilder(name.ToKebabCase());
            for (var i = 0; i < sb.Length; i++)
            {
                if (sb[i] != '-' && !char.IsLetter(sb[i]))
                {
                    sb[i] = '-';
                }
            }

            return sb.ToString();
        }
    }
}

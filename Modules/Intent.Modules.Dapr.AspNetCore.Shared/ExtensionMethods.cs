using System.Text;
using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Dapr.AspNetCore
{
    internal static class ExtensionMethods
    {
        public static string GetDaprApplicationName(this ISoftwareFactoryExecutionContext executionContext, string applicationId)
        {
            var sourceAppName = executionContext.GetApplicationConfig(applicationId).Name.ToKebabCase();

            var sb = new StringBuilder(sourceAppName);
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

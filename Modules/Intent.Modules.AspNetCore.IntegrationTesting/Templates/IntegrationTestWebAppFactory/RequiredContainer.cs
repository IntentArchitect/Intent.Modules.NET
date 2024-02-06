using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.IntegrationTestWebAppFactory
{
    internal class RequiredContainer
    {
        public RequiredContainer(string propertyName, string typeName)
        {
            PropertyName = propertyName;
            TypeName = typeName;
        }

        public string PropertyName { get; }
        public string TypeName { get; }
    }
}

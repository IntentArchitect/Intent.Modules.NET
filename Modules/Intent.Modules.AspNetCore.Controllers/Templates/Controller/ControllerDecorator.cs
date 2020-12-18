using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modelers.Services.Api;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    public class ControllerDecorator : ITemplateDecorator
    {
        public virtual int Priority { get; protected set; } = 0;

        public virtual IEnumerable<string> DependencyNamespaces()
        {
            return new string[0];
        }

        public virtual string BaseClass()
        {
            return null;
        }

        public virtual string EnterClass()
        {
            return null;
        }

        public virtual string ExitClass()
        {
            return null;
        }

        public virtual string ConstructorImplementation()
        {
            return null;
        }

        public virtual IEnumerable<string> ConstructorParameters()
        {
            return new string[0];
        }

        public virtual string EnterOperationBody(OperationModel operationModel)
        {
            return null;
        }

        public virtual string MidOperationBody(OperationModel operationModel)
        {
            return null;
        }

        public virtual string ExitOperationBody(OperationModel operationModel)
        {
            return null;
        }
    }
}

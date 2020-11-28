using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modelers.Services.Api;
using Intent.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Ignore)]

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller
{
    public class ControllerDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual string OnEnterOperationBody(OperationModel operationModel)
        {
            return null;
        }

        public virtual string OnExitOperationBody(OperationModel operationModel)
        {
            return null;
        }
    }
}

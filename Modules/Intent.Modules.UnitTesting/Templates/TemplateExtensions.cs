using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.UnitTesting.Templates.CommandHandlerTest;
using Intent.Modules.UnitTesting.Templates.DomainEventHandlerTest;
using Intent.Modules.UnitTesting.Templates.IntegrationEventHandlerTest;
using Intent.Modules.UnitTesting.Templates.QueryHandlerTest;
using Intent.Modules.UnitTesting.Templates.ServiceOperationTest;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Templates
{
    public static class TemplateExtensions
    {


    }
}
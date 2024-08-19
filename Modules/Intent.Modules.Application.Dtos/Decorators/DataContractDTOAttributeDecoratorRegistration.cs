using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using DtoModelTemplate = Intent.Modules.Application.Dtos.Templates.DtoModel.DtoModelTemplate;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Decorators
{
    [Description(DataContractDTOAttributeDecorator.DecoratorId)]
    public class DataContractDTOAttributeDecoratorRegistration : DecoratorRegistration<DtoModelTemplate, DtoModelDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override DtoModelDecorator CreateDecoratorInstance(DtoModelTemplate template, IApplication application)
        {
            return new DataContractDTOAttributeDecorator(template, application);
        }

        public override string DecoratorId => DataContractDTOAttributeDecorator.DecoratorId;
    }
}

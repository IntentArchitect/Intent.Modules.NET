using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Templates.DtoModel
{
    public abstract class DtoModelDecorator : ITemplateDecorator
    {
        public virtual int Priority => 0;
        public virtual string BaseClass() => null;
        public virtual IEnumerable<string> BaseInterfaces() => new string[0];
        public virtual string ClassAttributes(DTOModel dto) => null;
        public virtual string PropertyAttributes(DTOModel dto, DTOFieldModel field) => null;
        public virtual string EnterClass() => null;
        public virtual string ExitClass() => null;
    }
}

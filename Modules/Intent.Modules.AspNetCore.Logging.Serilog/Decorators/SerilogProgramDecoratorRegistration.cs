using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Templates.Program;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Decorators
{
    [Description(SerilogProgramDecorator.DecoratorId)]
    public class SerilogProgramDecoratorRegistration : DecoratorRegistration<ProgramTemplate, ProgramDecoratorBase>
    {
        public override ProgramDecoratorBase CreateDecoratorInstance(ProgramTemplate template, IApplication application)
        {
            return new SerilogProgramDecorator(template, application);
        }

        public override string DecoratorId => SerilogProgramDecorator.DecoratorId;
    }
}
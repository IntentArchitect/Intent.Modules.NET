using System.Collections.Generic;
using System.Linq;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorContract", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.Templates.Consumer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public abstract class ConsumerDecorator : ITemplateDecorator
    {
        public int Priority { get; protected set; } = 0;

        public virtual IEnumerable<string> GetConsumeEnterCode() => Enumerable.Empty<string>();

        public virtual IEnumerable<string> GetConsumeExitCode() => Enumerable.Empty<string>();

        public virtual IEnumerable<RequiredService> RequiredServices() => Enumerable.Empty<RequiredService>();

        public class RequiredService
        {
            public RequiredService(string type, string name)
            {
                Type = type;
                Name = name;
            }
            public string Type { get; }
            public string Name { get; }
            public string FieldName => $"_{Name}";
        }
    }
}
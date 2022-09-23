using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class DomainEventsDbContextDecorator : DecoratorBase
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsDbContextDecorator";

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public DomainEventsDbContextDecorator(ICSharpFileBuilderTemplate template, IApplication application)
        {
            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System.Linq");
                file.AddUsing("System.Threading");
                file.AddUsing("System.Threading.Tasks");
                var @class = file.Classes.First();
                @class.Constructors.First().AddParameter(template.GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId), "domainEventService", param =>
                {
                    param.IntroduceReadonlyField();
                });
                var saveMethod = @class.Methods.SingleOrDefault(x => x.Name == "SaveChangesAsync");
                if (saveMethod != null)
                {
                    saveMethod.InsertStatement(0, $"await DispatchEvents();");
                }
                else
                {
                    @class.InsertMethod(0, "Task<int>", "SaveChangesAsync", method =>
                    {
                        method.Override().Async()
                            .AddParameter($"CancellationToken", "cancellationToken",
                                param =>
                                {
                                    param.WithDefaultValue("default(CancellationToken)");
                                })
                            .AddStatement("await DispatchEvents();")
                            .AddStatement("return await base.SaveChangesAsync(cancellationToken);");
                    });
                }

                @class.AddMethod("Task", "DispatchEvents", method =>
                {
                    method.Private().Async()
                        .AddStatements($@"
while (true)
{{
    var domainEventEntity = ChangeTracker
        .Entries<{template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId)}>()
        .Select(x => x.Entity.DomainEvents)
        .SelectMany(x => x)
        .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

    if (domainEventEntity == null) break;

    domainEventEntity.IsPublished = true;
    await _domainEventService.Publish(domainEventEntity);
}}");
                });
            });
        }
    }
}
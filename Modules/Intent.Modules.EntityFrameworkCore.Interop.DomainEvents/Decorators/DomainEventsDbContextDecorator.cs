using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Modules.EntityFrameworkCore.Shared;
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

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
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

                var saveMethod = template.GetSaveChangesMethod();
                saveMethod.InsertStatement(0, "DispatchEventsAsync().GetAwaiter().GetResult();");

                var saveAsyncMethod = template.GetSaveChangesAsyncMethod();
                saveAsyncMethod.InsertStatement(0, "await DispatchEventsAsync(cancellationToken);");

                @class.AddMethod("Task", "DispatchEventsAsync", method =>
                {
                    method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method.Private().Async();
                    method.AddWhileStatement("true", @while => @while
                        .AddMethodChainStatement("var domainEventEntity = ChangeTracker", chain => chain
                            .AddChainStatement($"Entries<{template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId)}>()")
                            .AddChainStatement("Select(x => x.Entity.DomainEvents)")
                            .AddChainStatement("SelectMany(x => x)")
                            .AddChainStatement("FirstOrDefault(domainEvent => !domainEvent.IsPublished)"))
                        .AddIfStatement("domainEventEntity is null", @if => @if
                            .AddStatement("break;"))
                        .AddStatement("domainEventEntity.IsPublished = true;", s => s.SeparatedFromPrevious())
                        .AddStatement("await _domainEventService.Publish(domainEventEntity, cancellationToken);"));
                });
            });
        }
    }
}
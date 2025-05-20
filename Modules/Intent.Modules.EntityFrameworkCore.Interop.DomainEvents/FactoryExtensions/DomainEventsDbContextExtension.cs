using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventsDbContextExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsDbContextExtension";

        [IntentManaged(Mode.Ignore)] public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var contexts = DbContextManager.GetDbContexts(application.Id, application.MetadataManager);
            foreach (var context in contexts)
            {                
                var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext, context);
                template?.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System.Linq");
                    file.AddUsing("System.Threading");
                    file.AddUsing("System.Threading.Tasks");
                    var @class = file.Classes.First();
                    @class.Constructors.First().AddParameter(template.GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId), "domainEventService",
                        param => { param.IntroduceReadonlyField(); });

                    var saveMethod = template.GetSaveChangesMethod();
                    saveMethod.InsertStatement(0, "DispatchEventsAsync().GetAwaiter().GetResult();");

                    var saveAsyncMethod = template.GetSaveChangesAsyncMethod();
                    saveAsyncMethod.InsertStatement(0, "await DispatchEventsAsync(cancellationToken);");

                    @class.AddMethod(template.UseType("System.Threading.Tasks.Task"), "DispatchEventsAsync", method =>
                    {
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.Private().Async();
                        method.AddWhileStatement("true", @while => @while
                            .AddMethodChainStatement("var domainEventEntity = ChangeTracker", chain => chain
                                .AddChainStatement($"Entries<{template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId)}>()")
                                .AddChainStatement("SelectMany(x => x.Entity.DomainEvents)")
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
}
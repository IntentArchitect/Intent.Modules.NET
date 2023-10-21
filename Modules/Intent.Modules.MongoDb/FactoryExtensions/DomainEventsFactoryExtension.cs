using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Microsoft.Win32;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventsFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.DomainEventsFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private string HasDomainEventInterfaceTemplateId => "Intent.DomainEvents.HasDomainEventInterface";
        private string DomainEventServiceInterfaceTemplateId => "Intent.DomainEvents.DomainEventServiceInterface";

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
#warning FulFullsRole
            if (application.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(DomainEventServiceInterfaceTemplateId)) == null)
                return;

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(ApplicationMongoDbContextTemplate.TemplateId));
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("System.Linq");
                var @class = file.Classes.First();
                @class.Constructors.First().AddParameter(template.GetTypeName(DomainEventServiceInterfaceTemplateId), "domainEventService", param =>
                {
                    param.IntroduceReadonlyField();
                });
                var saveMethod = @class.Methods.SingleOrDefault(x => x.Name == "SaveChangesAsync");
                if (saveMethod != null)
                {
                    saveMethod.InsertStatement(0, $"await DispatchEvents(cancellationToken);");
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
                            .AddStatement("await DispatchEvents(cancellationToken);")
                            .AddStatement("await base.SaveChangesAsync(cancellationToken);")
                            .AddStatement("return default;");
                    });
                }

                @class.AddMethod("Task", "DispatchEvents", method =>
                {
                    method.Async().AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"));

                    string hasDomainEventsInterface = template.GetTypeName(HasDomainEventInterfaceTemplateId);
                    method.Private().Async();
                    method.AddWhileStatement("true", @while => @while
                        .AddMethodChainStatement("var domainEventEntity = ChangeTracker", chain => chain
                            .AddChainStatement("Entries()")
                            .AddChainStatement("Select(x => x.Entity)")
                            .AddChainStatement($"OfType<{hasDomainEventsInterface}>()")
                            .AddChainStatement("SelectMany(x => x.DomainEvents)")
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
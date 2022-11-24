using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEvent;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.Modules.DomainEvents.Templates.DomainEventServiceInterface;
using Intent.Modules.DomainEvents.Templates.HasDomainEventInterface;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Interop.DomainEvents.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DomainEventsDecoratorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.Interop.DomainEvents.DomainEventsDecoratorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => -10;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var entityStateTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in entityStateTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.FirstOrDefault();
                    if (@class.TryGetMetadata<ClassModel>("model", out var model) && 
                        model.IsAggregateRoot() && model.ParentClass == null)
                    {
                        @class.ImplementsInterface(template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId));
                        @class.AddProperty($"{template.UseType("System.Collections.Generic.List")}<{template.GetTypeName(DomainEventBaseTemplate.TemplateId)}>", "DomainEvents", property =>
                        {
                            property.WithInitialValue($"new {property.Type}()");
                            property.AddMetadata("non-persistent", true);
                        });
                    }
                });
            }

            var entityInterfaceTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Interface));
            foreach (var template in entityInterfaceTemplates.Where(x => x.CSharpFile.Interfaces.Any()))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    if (@interface.TryGetMetadata<ClassModel>("model", out var model) &&
                        model.IsAggregateRoot() && model.ParentClass == null)
                    {
                        @interface.ExtendsInterface(template.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId));
                    }
                });
            }

            var designTimeDbContextFactoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Data.DesignTimeDbContextFactory"));
            foreach (var template in designTimeDbContextFactoryTemplates)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("CreateDbContext");
                    var lastStatement = method.Statements.LastOrDefault(p => p.HasMetadata("return-statement"));
                    lastStatement?.Replace(lastStatement?.GetText("").Replace(");", ", null);"));
                });
            }

            //            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Infrastructure.Data.DbContext");
            //            dbContext?.CSharpFile.OnBuild(file =>
            //            {
            //                file.AddUsing("System.Linq");
            //                file.AddUsing("System.Threading");
            //                file.AddUsing("System.Threading.Tasks");
            //                var @class = file.Classes.First();
            //                @class.Constructors.First().AddParameter(dbContext.GetTypeName(DomainEventServiceInterfaceTemplate.TemplateId), "domainEventService", param =>
            //                {
            //                    param.IntroduceReadonlyField();
            //                });
            //                var saveMethod = @class.Methods.SingleOrDefault(x => x.Name == "SaveChangesAsync");
            //                if (saveMethod != null)
            //                {
            //                    saveMethod.InsertStatement(0, $"await DispatchEvents();");
            //                }
            //                else
            //                {
            //                    @class.InsertMethod(0, "Task<int>", "SaveChangesAsync", method =>
            //                    {
            //                        method.Override().Async()
            //                            .AddParameter($"CancellationToken", "cancellationToken",
            //                                param =>
            //                                {
            //                                    param.WithDefaultValue("default(CancellationToken)");
            //                                })
            //                            .AddStatement("await DispatchEvents();")
            //                            .AddStatement("return await base.SaveChangesAsync(cancellationToken);");
            //                    });
            //                }

            //                @class.AddMethod("Task", "DispatchEvents", method =>
            //                {
            //                    method.Private().Async()
            //                        .AddStatements($@"
            //while (true)
            //{{
            //    var domainEventEntity = ChangeTracker
            //        .Entries<{dbContext.GetTypeName(HasDomainEventInterfaceTemplate.TemplateId)}>()
            //        .Select(x => x.Entity.DomainEvents)
            //        .SelectMany(x => x)
            //        .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

            //    if (domainEventEntity == null) break;

            //    domainEventEntity.IsPublished = true;
            //    await _domainEventService.Publish(domainEventEntity);
            //}}");
            //                });
            //            });
        }
    }
}
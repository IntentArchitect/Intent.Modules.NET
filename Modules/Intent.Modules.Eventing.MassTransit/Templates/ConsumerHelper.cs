using System;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.UnitOfWork.Persistence.Shared;

using OutboxPatternType = Intent.Modules.Eventing.MassTransit.Settings.EventingSettings.OutboxPatternOptionsEnum;

namespace Intent.Modules.Eventing.MassTransit.Templates;

public static class ConsumerHelper
{
    public static void AddConsumerDependencies<TModel>(CSharpTemplateBase<TModel> template)
    {
        template.AddNugetDependency(NugetPackages.MassTransitAbstractions(template.OutputTarget));
        template.AddTypeSource(IntegrationEventMessageTemplate.TemplateId);
        template.AddTypeSource(IntegrationCommandTemplate.TemplateId);
    }

    public delegate void ConfigureClass(CSharpClass consumerClass, CSharpGenericParameter tMessage);
    public delegate void ConfigureConsumeMethod(CSharpClass consumerClass, CSharpClassMethod consumeMethod, CSharpGenericParameter tMessage);
    
    public static void AddConsumerClass(
        ICSharpFileBuilderTemplate template,
        string baseName,
        ConfigureClass configureClass,
        ConfigureConsumeMethod configureConsumeMethod,
        bool applyStandardUnitOfWorkLogic)
    {
        template.CSharpFile.AddUsing("Microsoft.Extensions.DependencyInjection");

        template.CSharpFile.AddClass($"{baseName}Consumer", @class =>
        {
            @class.AddGenericParameter("TMessage", out var tMessage);
            @class.ImplementsInterface($"{template.UseType("MassTransit.IConsumer")}<{tMessage}>");
            @class.AddGenericTypeConstraint(tMessage, c => c.AddType("class"));

            @class.AddConstructor(ctor => { ctor.AddParameter(template.UseType("System.IServiceProvider"), "serviceProvider", param => param.IntroduceReadonlyField()); });

            configureClass(@class, tMessage);

            @class.AddMethod(template.UseType("System.Threading.Tasks.Task"), "Consume", method =>
            {
                method.Async();
                method.AddParameter($"{template.UseType("MassTransit.ConsumeContext")}<{tMessage}>", "context");

                method.AddStatement($"var eventBus = _serviceProvider.GetRequiredService<{template.GetMassTransitEventBusName()}>();");
                method.AddStatement($"eventBus.ConsumeContext = context;");

                configureConsumeMethod(@class, method, tMessage);
                
                // This assumes two modes of implementation:
                // 1. The Consumer will handle everything.
                // 2. The Consumer uses a dispatcher with its own middleware that will do everything.
                if (applyStandardUnitOfWorkLogic)
                {
                    method.AddStatement($"await eventBus.FlushAllAsync(context.CancellationToken);",
                        stmt => stmt.AddMetadata("event-bus-flush", true));

                    if (template.SystemUsesPersistenceUnitOfWork())
                    {
                        ApplyUnitOfWorkSaves(template, @class, method);
                    }
                }
            });
        });
    }

    public static void AddConsumerDefinitionClass(
        ICSharpFileBuilderTemplate template,
        string baseName)
    {
        template.CSharpFile.AddClass($"{baseName}ConsumerDefinition", @class =>
        {
            var consumerClass = template.CSharpFile.Classes.FirstOrDefault(p => p.Name != @class.Name);
            if (consumerClass is null)
            {
                throw new Exception($"This should be invoked after {nameof(AddConsumerClass)}");
            }

            foreach (var genericParameter in consumerClass.GenericParameters)
            {
                @class.AddGenericParameter(genericParameter.TypeName);
            }

            foreach (var constraint in consumerClass.GenericTypeConstraints)
            {
                @class.AddGenericTypeConstraint(constraint.GenericTypeParameter, constraintConfig =>
                {
                    foreach (var constraintType in constraint.Types)
                    {
                        constraintConfig.AddType(constraintType);
                    }
                });
            }

            var consumerClassTypeName = $"{consumerClass.Name}<{string.Join(", ", consumerClass.GenericParameters.Select(s => s.TypeName))}>";

            @class.WithBaseType($"{template.UseType("MassTransit.ConsumerDefinition")}<{consumerClassTypeName}>");

            @class.AddMethod("void", "ConfigureConsumer", method =>
            {
                method.Protected().Override();
                method.AddParameter(template.UseType("MassTransit.IReceiveEndpointConfigurator"), "endpointConfigurator");
                method.AddParameter($"{template.UseType("MassTransit.IConsumerConfigurator")}<{consumerClassTypeName}>", "consumerConfigurator");
                method.AddParameter(template.UseType("MassTransit.IRegistrationContext"), "context");

                switch (template.ExecutionContext.Settings.GetEventingSettings().OutboxPattern().AsEnum())
                {
                    case OutboxPatternType.InMemory:
                        method.AddStatement("endpointConfigurator.UseInMemoryInboxOutbox(context);");
                        break;
                    case OutboxPatternType.EntityFramework when EfIsPresent(template):
                        method.AddStatement($"endpointConfigurator.UseEntityFrameworkOutbox<{template.GetTypeName(TemplateRoles.Infrastructure.Data.DbContext)}>(context);");
                        break;
                    case OutboxPatternType.None:
                    case OutboxPatternType.EntityFramework:
                    default:
                        // Do nothing
                        break;
                }
            });
        });
    }

    private static void ApplyUnitOfWorkSaves(ICSharpFileBuilderTemplate template, CSharpClass @class, CSharpClassMethod method)
    {
        var executeHandler = method.FindStatement(x => x.TryGetMetadata("handler", out string value) && value == "execute");
        executeHandler.Remove();

        var flushAll = method.FindStatement(p => p.HasMetadata("event-bus-flush"));
        flushAll.Remove();

        var outboxPatternType = template.ExecutionContext.Settings.GetEventingSettings().OutboxPattern().AsEnum();

        // When we're using EF's outbox pattern, then MassTransit itself creates a transaction and saves the changes
        var allowTransactionScope = outboxPatternType != OutboxPatternType.EntityFramework;

        method.ApplyUnitOfWorkImplementations(
            template: template,
            constructor: @class.Constructors.First(),
            invocationStatement: executeHandler,
            allowTransactionScope: allowTransactionScope,
            cancellationTokenExpression: "context.CancellationToken");

        switch (outboxPatternType)
        {
            case OutboxPatternType.None:
            case OutboxPatternType.InMemory:
                method.AddStatement(flushAll);
                break;
            case OutboxPatternType.EntityFramework:
                // Transactional outbox pattern will have the EventBus Flush happen in the DbContext's SaveChanges.
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private static bool EfIsPresent(ICSharpFileBuilderTemplate template)
    {
        return template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out _) ||
               template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out _);
    }
}
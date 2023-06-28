using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.Decorators;

[IntentManaged(Mode.Merge)]
public class ConsumerUnitOfWorkDecorator : ConsumerDecorator
{
    [IntentManaged(Mode.Fully)]
    public const string DecoratorId = "Intent.Eventing.MassTransit.EntityFrameworkCore.ConsumerUnitOfWorkDecorator";

    [IntentManaged(Mode.Fully)] private readonly WrapperConsumerTemplate _template;

    [IntentManaged(Mode.Fully)] private readonly IApplication _application;

    [IntentManaged(Mode.Fully, Body = Mode.Fully)]
    public ConsumerUnitOfWorkDecorator(WrapperConsumerTemplate template, IApplication application)
    {
        _template = template;
        _application = application;

        template.CSharpFile.OnBuild(file =>
        {
            var priClass = file.Classes.First(p => p.Name == "WrapperConsumer");
            var secClass = file.Classes.First(p => p.Name == "WrapperConsumerDefinition");
            var unitOfWorkName = GetUnitOfWorkName();

            if (!string.IsNullOrEmpty(unitOfWorkName) && ShouldUseTransactionScope())
            {
                _template.AddUsing("System.Transactions");
                priClass.Constructors.First()
                    .AddParameter(unitOfWorkName, "unitOfWork", parm => parm.IntroduceReadonlyField());

                InstallTransactionScope(priClass, unitOfWorkName);
            }
            else
            {
                secClass.FindMethod("ConfigureConsumer")
                    ?.AddStatement($"endpointConfigurator.UseEntityFrameworkOutbox<{_template.GetTypeName("Infrastructure.Data.DbContext")}>(_serviceProvider);");
            }
        });
    }

    private void InstallTransactionScope(CSharpClass @class, string unitOfWorkName)
    {
        var consume = @class.FindMethod("Consume");
        var handlers = consume.FindStatements(p => p.HasMetadata("handler")).ToList();
        var usingStatement = new CSharpUsingBlock($@"var transaction = new TransactionScope(TransactionScopeOption.Required,
new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled)");
        
        var flushAll = consume.FindStatement(p => p.HasMetadata("event-bus-flush"));
        flushAll.Remove();
        
        handlers.First().InsertAbove(usingStatement);
        handlers.ForEach(x => x.Remove());
        handlers.ForEach(x => usingStatement.AddStatement(x));
        usingStatement.AddStatement($"await _unitOfWork.SaveChangesAsync(context.CancellationToken);",
            stmt => stmt.AddMetadata("transaction", "save-changes"));
        usingStatement.AddStatement($"transaction.Complete();", 
            stmt => stmt.AddMetadata("transaction", "complete"));
        
        switch (_template.ExecutionContext.Settings.GetEventingSettings().OutboxPattern().AsEnum())
        {
            case EventingSettings.OutboxPatternOptionsEnum.None:
                usingStatement.InsertBelow(flushAll);
                break;
            case EventingSettings.OutboxPatternOptionsEnum.EntityFramework:
                // Do nothing
                break;
            case EventingSettings.OutboxPatternOptionsEnum.InMemory:
                usingStatement.FindStatement(p => p.HasMetadata("transaction"))
                    ?.InsertAbove(flushAll);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetUnitOfWorkName()
    {
        if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWorkTypeName) ||
            _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface,
                out unitOfWorkTypeName))
        {
            return unitOfWorkTypeName;
        }

        return null;
    }

    // Use of Transaction scopes is not needed when EF Outbox pattern is used since it will
    // have its own embedded transaction scope instead.
    private bool ShouldUseTransactionScope()
    {
        return !_template.ExecutionContext.Settings.GetEventingSettings().OutboxPattern().IsEntityFramework();
    }
}
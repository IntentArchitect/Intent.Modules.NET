using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConsumerTransactionDecorator : ConsumerDecorator
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.Eventing.MassTransit.EntityFrameworkCore.ConsumerTransactionDecorator";

        [IntentManaged(Mode.Fully)] private readonly WrapperConsumerTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ConsumerTransactionDecorator(WrapperConsumerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override IEnumerable<RequiredService> RequiredServices()
        {
            var unitOfWorkName = GetUnitOfWorkName();
            if (string.IsNullOrEmpty(unitOfWorkName))
            {
                yield break;
            }

            yield return new RequiredService(unitOfWorkName, "unitOfWork");
        }

        public override IEnumerable<string> GetConsumeEnterCode()
        {
            var unitOfWorkName = GetUnitOfWorkName();
            if (string.IsNullOrEmpty(unitOfWorkName))
            {
                yield break;
            }

            _template.AddUsing("System.Transactions");

            yield return $@"using (var transaction = new TransactionScope(TransactionScopeOption.Required,";
            yield return $@"    new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))";
            yield return $@"{{";
        }

        public override IEnumerable<string> GetConsumeExitCode()
        {
            var unitOfWorkName = GetUnitOfWorkName();
            if (string.IsNullOrEmpty(unitOfWorkName))
            {
                yield break;
            }

            yield return $@"    await _unitOfWork.SaveChangesAsync(context.CancellationToken);";
            yield return $@"    ";
            yield return $@"    transaction.Complete();";
            yield return $@"}}";
        }

        private string GetUnitOfWorkName()
        {
            if (_template.TryGetTypeName(TemplateFulfillingRoles.Domain.UnitOfWork, out var unitOfWorkTypeName) ||
                _template.TryGetTypeName(TemplateFulfillingRoles.Application.Common.DbContextInterface, out unitOfWorkTypeName))
            {
                return unitOfWorkTypeName;
            }

            return null;
        }
    }
}
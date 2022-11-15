using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.WrapperConsumer;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConsumerUnitOfWorkDecorator : ConsumerDecorator
    {
        [IntentManaged(Mode.Fully)] public const string DecoratorId = "Intent.Eventing.MassTransit.EntityFrameworkCore.ConsumerUnitOfWorkDecorator";

        [IntentManaged(Mode.Fully)] private readonly WrapperConsumerTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ConsumerUnitOfWorkDecorator(WrapperConsumerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override void BeforeTemplateExecution()
        {
            switch (_template.ExecutionContext.Settings.GetEventingSettings().OutboxPattern().AsEnum())
            {
                case EventingSettings.OutboxPatternOptionsEnum.None:
                    _template.RepositionFlushAllStatement(this, WrapperConsumerTemplate.RepositionDirection.AfterThisDecorator);
                    break;
                case EventingSettings.OutboxPatternOptionsEnum.EntityFramework:
                    _template.RepositionFlushAllStatement(this, WrapperConsumerTemplate.RepositionDirection.BeforeThisDecorator);
                    _template.AddConsumerConfiguration($"endpointConfigurator.UseEntityFrameworkOutbox<{_template.GetTypeName("Infrastructure.Data.DbContext")}>(_serviceProvider);");
                    break;
                case EventingSettings.OutboxPatternOptionsEnum.InMemory:
                    _template.RepositionFlushAllStatement(this, WrapperConsumerTemplate.RepositionDirection.BeforeThisDecorator);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
        }

        public override IEnumerable<string> GetConsumeExitCode()
        {
            var unitOfWorkName = GetUnitOfWorkName();
            if (string.IsNullOrEmpty(unitOfWorkName))
            {
                yield break;
            }

            yield return $@"    await _unitOfWork.SaveChangesAsync(context.CancellationToken);";
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
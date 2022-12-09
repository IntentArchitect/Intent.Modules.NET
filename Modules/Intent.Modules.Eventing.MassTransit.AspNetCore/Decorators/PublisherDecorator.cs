using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.AspNetCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class PublisherDecorator : ControllerDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Eventing.MassTransit.AspNetCore.PublisherDecorator";

        [IntentManaged(Mode.Fully)] private readonly ControllerTemplate _template;
        [IntentManaged(Mode.Fully)] private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PublisherDecorator(ControllerTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }







        //public override int Priority => IsTransactionalOutboxPatternSelected() ? 90 : -15;

        //public override string EnterClass()
        //{
        //    return $@"private readonly {_template.GetEventBusInterfaceName()} _eventBus;";
        //}

        //public override IEnumerable<string> ConstructorParameters()
        //{
        //    yield return $@"{_template.GetEventBusInterfaceName()} eventBus";
        //}

        //public override string ConstructorImplementation()
        //{
        //    return $@"_eventBus = eventBus;";
        //}

        //public override string MidOperationBody(OperationModel operationModel)
        //{
        //    return IsTransactionalOutboxPatternSelected()
        //        ? $@"await _eventBus.FlushAllAsync(cancellationToken);"
        //        : string.Empty;
        //}

        //public override string ExitOperationBody(OperationModel operationModel)
        //{
        //    return !IsTransactionalOutboxPatternSelected()
        //        ? $@"await _eventBus.FlushAllAsync(cancellationToken);"
        //        : string.Empty;
        //}

        private bool IsTransactionalOutboxPatternSelected()
        {
            return _application.Settings.GetEventingSettings()?.OutboxPattern()?.IsEntityFramework() == true;
        }
    }
}
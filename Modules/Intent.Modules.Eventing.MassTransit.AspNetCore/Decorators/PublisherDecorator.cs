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
            _template.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var ctor = @class.Constructors.First();
                ctor.AddParameter(_template.GetEventBusInterfaceName(), "eventBus", p =>
                {
                    p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                });

                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<OperationModel>("model", out var operation) && 
                        operation.HasHttpSettings() && !operation.GetHttpSettings().Verb().IsGET())
                    {
                        if (IsTransactionalOutboxPatternSelected())
                        {
                            method.Statements.LastOrDefault(x => x.ToString().Trim().StartsWith("await "))?
                                .InsertBelow("await _eventBus.FlushAllAsync(cancellationToken);");
                        }
                        else
                        {
                            method.Statements.LastOrDefault(x => x.ToString().Trim().StartsWith("return "))?
                                .InsertAbove("await _eventBus.FlushAllAsync(cancellationToken);");
                        }
                    }
                }
            }, order: -100);

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
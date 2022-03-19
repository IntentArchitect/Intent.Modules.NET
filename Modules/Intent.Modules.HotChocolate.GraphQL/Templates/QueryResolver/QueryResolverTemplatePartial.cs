using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    partial class QueryResolverTemplate : CSharpTemplateBase<ServiceModel>
    {
        public const string TemplateId = "Intent.HotChocolate.GraphQL.QueryResolver";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryResolverTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource("Application.Contract.Dto");
            AddTypeSource("Application.Contract.Query");
            FulfillsRole("Api.GraphQL.QueryResolver");
            AddNugetDependency(NuGetPackages.HotChocolate);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name.RemoveSuffix("Service", "Controller", "Query", "Queries", "QueryResolver")}QueryResolver",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }

        public override bool CanRunTemplate()
        {
            return Model.Operations.Any(IsQuery);
        }

        private string GetReturnType(OperationModel operation)
        {
            if (operation.ReturnType != null)
            {
                return $"async Task<{GetTypeName(operation.ReturnType)}>";
            }

            return $"async Task<Unit>";
        }

        private string GetParameters(OperationModel operation)
        {
            var parameters = new List<string>();
            parameters.Add($"[Service(ServiceKind.Synchronized)] {UseType("MediatR.IMediator")} mediator");
            foreach (var operationParameter in operation.Parameters)
            {
                parameters.Add($"{GetTypeName(operationParameter)} {operationParameter.Name.ToCamelCase()}");
            }
            return string.Join(", ", parameters);
        }

        private string GetImplementation(OperationModel operation)
        {
            var payload = GetPayloadParameter(operation)?.Name
                          ?? (operation.InternalElement.IsMapped ? GetMappedPayload(operation) : "UNKNOWN");
            if (operation.ReturnType != null)
            {
                return $@"
            return await mediator.Send({payload});";
            }
            return $@"
            await mediator.Send({payload});
            return true;";
        }

        private bool IsQuery(OperationModel operation)
        {
            return operation.InternalElement.MappedElement.Element.IsQueryModel();
        }

        private ParameterModel GetPayloadParameter(OperationModel operationModel)
        {
            return operationModel.Parameters.SingleOrDefault(x =>
                x.Type.Element.SpecializationTypeId == CommandModel.SpecializationTypeId ||
                x.Type.Element.SpecializationTypeId == QueryModel.SpecializationTypeId);
        }

        private string GetMappedPayload(OperationModel operationModel)
        {
            var mappedElement = operationModel.InternalElement.MappedElement;
            if (GetMappedParameters(operationModel).Any())
            {
                return $"new {GetTypeName(mappedElement)} {{ {string.Join(", ", GetMappedParameters(operationModel).Select(x => x.InternalElement.MappedElement.Element.Name.ToPascalCase() + " = " + x.Name))}}}";
            }
            return $"new {GetTypeName(mappedElement)}()";
        }

        public IList<ParameterModel> GetMappedParameters(OperationModel operationModel)
        {
            return operationModel.Parameters.Where(x => x.InternalElement.IsMapped).ToList();
        }
    }
}
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.Application.ServiceCallHandlers.Templates.ServiceCallHandler
{
    partial class ServiceCallHandlerImplementationTemplate : CSharpTemplateBase<OperationModel>
    {
        public const string Identifier = "Intent.Application.ServiceCallHandlers.Handler";

        public ServiceCallHandlerImplementationTemplate(IOutputTarget outputTarget, OperationModel model)
            : base(Identifier, outputTarget, model)
        {
            SetDefaultTypeCollectionFormat("List<{0}>");
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DtoModelTemplate.TemplateId, "List<{0}>"));
        }

        public ServiceModel Service { get; set; }

        protected override CSharpFileConfig DefineFileConfig()
        {
            var additionalFolders = Model.ParentService.GetParentFolderNames()
                .Concat(new[] { Model.ParentService.Name })
                .ToArray();

            return new CSharpFileConfig(
                className: $"{Model.Name}SCH",
                @namespace: $"{this.GetNamespace(additionalFolders)}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders)}");
        }

        private string GetOperationDefinitionParameters(OperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.TypeReference)}>" : GetTypeName(o.TypeReference);
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Application")
            );
        }
    }
}

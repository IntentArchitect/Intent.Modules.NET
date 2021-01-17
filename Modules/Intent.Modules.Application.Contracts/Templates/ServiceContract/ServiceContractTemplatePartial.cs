using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ServiceContractTemplate : CSharpTemplateBase<ServiceModel, ServiceContractDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Contracts.ServiceContract";

        [IntentManaged(Mode.Ignore)]
        public ServiceContractTemplate(IOutputTarget project, ServiceModel model, string identifier = TemplateId)
            : base(identifier, project, model)
        {
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            SetDefaultTypeCollectionFormat("List<{0}>");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        public string FolderBasedNamespace => string.Join(".", new[] { OutputTarget.GetNamespace() }.Concat(GetNamespaceParts()));

        public string ContractAttributes()
        {
            return GetDecorators().Aggregate(x => x.ContractAttributes(Model));
        }

        public string OperationAttributes(OperationModel operation)
        {
            return GetDecorators().Aggregate(x => x.OperationAttributes(Model, operation));
        }

        public string EnterClass()
        {
            return GetDecorators().Aggregate(x => x.EnterClass());
        }

        public string ExitClass()
        {
            return GetDecorators().Aggregate(x => x.EnterClass());
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetParentFolders()
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") ?? x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
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
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.TypeReference);
        }
    }
}

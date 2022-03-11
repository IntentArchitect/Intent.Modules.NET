using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Application.Dtos.Templates.DtoModel
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class DtoModelTemplate : CSharpTemplateBase<DTOModel, DtoModelDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.Dtos.DtoModel";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public DtoModelTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
            AddTypeSource("Domain.Enums", "List<{0}>");
            FulfillsRole("Application.Contracts.Dtos");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        public string ClassAttributes()
        {
            return GetDecorators().Aggregate(x => x.ClassAttributes(Model));
        }

        public string PropertyAttributes(DTOFieldModel field)
        {
            return GetDecorators().Aggregate(x => x.PropertyAttributes(Model, field));
        }

        public string EnterClass()
        {
            return GetDecorators().Aggregate(x => x.EnterClass());
        }

        public string ExitClass()
        {
            return GetDecorators().Aggregate(x => x.ExitClass());
        }

        public string GetBaseTypes()
        {
            var baseTypes = new List<string>();
            if (GetDecorators().Any(x => !string.IsNullOrWhiteSpace(x.BaseClass())))
            {
                baseTypes.Add(GetDecorators().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.BaseClass()))?.BaseClass());
            }
            baseTypes.AddRange(GetDecorators().SelectMany(x => x.BaseInterfaces()));
            return baseTypes.Any() ? $" : {string.Join(", ", baseTypes)}" : "";
        }

        public string ConstructorParameters()
        {
            var parameters = new List<string>();
            foreach (var field in Model.Fields)
            {
                parameters.Add($"{GetTypeName(field.TypeReference)} {field.Name.ToParameterName()}");
            }
            return $@"
            {string.Join(@",
            ", parameters)}";
        }

        public string GenericTypes => Model.GenericTypes.Any() ? $"<{ string.Join(", ", Model.GenericTypes) }>" : "";

    }
}

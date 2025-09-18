using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Mapperly.Templates.DtoMappingProfile
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DtoMappingProfileTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Dtos.Mapperly.DtoMappingProfile";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoMappingProfileTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.RiokMapperly(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Riok.Mapperly.Abstractions")
                .AddClass($"{Model.Name}Mapper", @class =>
                {
                    @class.Partial();
                    @class.AddAttribute("Mapper");
                    var entityTemplate = MappingHelper.GetEntityTemplate(this, Model);
                    string dtoModelName = GetTypeName(TemplateRoles.Application.Contracts.Dto, Model);
                    @class.AddMethod(dtoModelName, $"{this.GetTypeName(entityTemplate)}To{dtoModelName}", method =>
                    {
                        method.Public().Partial();
                        method.AddParameter(this.GetTypeName(entityTemplate), this.GetTypeName(entityTemplate).ToPascalCase()).WithoutMethodModifier();
                    });

                    @class.AddMethod($"List<{dtoModelName}>", $"{this.GetTypeName(entityTemplate)}To{dtoModelName}List", method =>
                    {
                        method.Public().Partial();
                        method.AddParameter($"List<{this.GetTypeName(entityTemplate)}>", this.GetTypeName(entityTemplate).ToPascalCase().Pluralize()).WithoutMethodModifier();
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
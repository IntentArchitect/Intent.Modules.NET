using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IntegrationEventDtoTemplate : CSharpTemplateBase<EventingDTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.IntegrationEventDto";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IntegrationEventDtoTemplate(IOutputTarget outputTarget, EventingDTOModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateId);
            AddTypeSource(IntegrationEventEnumTemplate.TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);

            CSharpFile = new CSharpFile(
                    @namespace: Model.InternalElement.Package.Name.ToPascalCase(),
                    relativeLocation: this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (Model.ParentDtoTypeReference != null)
                    {
                        @class.WithBaseType(GetTypeName(Model.ParentDtoTypeReference));
                    }

                    @class.AddConstructor(constructor =>
                    {
                        if (Model.IsAbstract)
                        {
                            constructor.Protected();
                        }
                    });

                    if (!Model.IsAbstract)
                    {
                        var typeName = !Model.GenericTypes.Any()
                            ? ClassName
                            : $"{ClassName}<{string.Join(", ", Model.GenericTypes)}>";

                        @class.AddMethod(typeName, "Create", method =>
                        {
                            method.Static();

                            foreach (var field in Model.Fields)
                            {
                                method.AddParameter(GetTypeName(field), field.Name.ToLocalVariableName());
                            }

                            method.AddStatement(new CSharpObjectInitializerBlock($"return new {typeName}"), objectInitializer =>
                            {
                                foreach (var field in Model.Fields)
                                {
                                    objectInitializer.AddInitStatement(field.Name.ToPascalCase(), field.Name.ToLocalVariableName());
                                }

                                objectInitializer.WithSemicolon();
                            });
                        });
                    }

                    foreach (var field in Model.Fields)
                    {
                        @class.AddProperty(GetTypeName(field), field.Name.ToPascalCase());
                    }
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
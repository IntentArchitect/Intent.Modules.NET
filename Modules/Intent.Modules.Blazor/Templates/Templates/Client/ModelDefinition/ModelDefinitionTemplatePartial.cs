using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ModelDefinitionTemplate : CSharpTemplateBase<ModelDefinitionModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Templates.Client.ModelDefinitionTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ModelDefinitionTemplate(IOutputTarget outputTarget, ModelDefinitionModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateId);
            AddTypeSource("Blazor.HttpClient.Contracts.Dto");

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}", @class =>
                {
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    foreach (var constructorModel in Model.Constructors)
                    {
                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameters(constructorModel.Parameters);
                        });
                    }

                    foreach (var propertyModel in Model.Properties)
                    {
                        @class.AddProperty(propertyModel, prop =>
                        {
                            if (!propertyModel.TypeReference.IsNullable && prop.InitialValue == null)
                            {
                                if (propertyModel.TypeReference.IsCollection)
                                {
                                    prop.WithInitialValue("[]");
                                }
                                else if (GetTypeInfo(propertyModel.TypeReference).Template != null)
                                {
                                    prop.WithInitialValue("new()");
                                }
                            }
                        });
                    }

                    foreach (var operationModel in Model.Operations)
                    {
                        @class.AddMethod(operationModel, method =>
                        {
                            method.AddParameters(operationModel.Parameters);
                        });
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
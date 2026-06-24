using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Wolverine.Templates;
using Intent.Modules.Application.Wolverine.Templates.QueryInterface;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.Templates.QueryModels
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryModelsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.QueryModels";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryModelsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource("Domain.Enum");
            AddTypeSource("Application.Contract.Dto");
            AddTypeSource("Application.Contract.Enum");
            AddTypeSource("Application.Contracts.Client.Dto");
            AddTypeSource("Application.Contracts.Client.Enum");

            CSharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), this.GetFolderPath(additionalFolders: Model.GetConceptName()))
                .AddUsing("System")
                .AddClass(Model.Name, @class =>
                {
                    @class.RepresentsModel(model);
                    @class.ImplementsInterface(this.GetQueryInterfaceName());

                    @class.AddConstructor();
                    var ctor = @class.Constructors.First();

                    // get the last property which has no value. All items occurring before this cannot have a default value set in the constructor
                    var lastNonNullable = Model.Properties.LastOrDefault(p => string.IsNullOrEmpty(p.Value))?.InternalElement.Order ?? 0;

                    List<string> nulledFields = [];
                    foreach (var property in Model.Properties)
                    {
                        // should the default value be set, based on the position of it as an argument
                        var setDefaultValue = property.ShouldSetDefaultValue(lastNonNullable);
                        // set the type
                        var typeValue = property.GetTypeReferenceName(setDefaultValue, this);

                        ctor.AddParameter(typeValue, property.Name.ToParameterName(), param =>
                        {
                            param.AddMetadata("model", property);

                            // only parameters with a value AFTER the last parameter with a value get the value specified
                            if (setDefaultValue)
                            {
                                param.WithDefaultValue(property.Value.AsFormattedValidTypeValue(this, property.TypeReference));

                                // if is a collection, with a default value, set to null instead
                                if (property.TypeReference?.IsCollection ?? false)
                                {
                                    param.WithDefaultValue("null");
                                    nulledFields.Add(property.Id);
                                }
                            }

                            @class.AddProperty(GetTypeName(property), property.Name.ToPropertyName(), prop =>
                            {
                                prop.AddMetadata("model", property);
                                prop.RepresentsModel(property);

                                // Do the assignment in the constructor
                                var rhs = setDefaultValue && nulledFields.Contains(property.Id) ? $"{property.Name.ToParameterName()} ?? {property.Value}" :
                                    property.Name.ToParameterName();
                                var assignmentStatement = new CSharpFieldAssignmentStatement(prop.Name, rhs);
                                ctor.AddStatement(assignmentStatement);
                            });
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

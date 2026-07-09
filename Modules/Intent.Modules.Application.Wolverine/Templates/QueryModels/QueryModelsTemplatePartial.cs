using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
using Intent.Modules.Metadata.Security.Models;
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
                    @class.TryAddXmlDocComments(Model.InternalElement);
                    CqrsTemplateHelpers.AddAuthorization(this, @class, Model.InternalElement);
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

                            // AddProperty is used instead of IntroduceProperty as the property and the parameter might not have the same type
                            // One could be non-nullable and the other not, specifically when its a collection with default value
                            @class.AddProperty(GetTypeName(property), property.Name.ToPropertyName(), prop =>
                            {
                                prop.TryAddXmlDocComments(property.InternalElement);
                                prop.RepresentsModel(property);

                                var defaultValueKind = property.GetDefaultValueAttributeKind();
                                if (!string.IsNullOrWhiteSpace(property.Value) && defaultValueKind != DefaultValueAttributeKind.None)
                                {
                                    prop.AddAttribute(UseType("System.ComponentModel.DefaultValue"), attribute =>
                                    {
                                        if (defaultValueKind == DefaultValueAttributeKind.TypeAndString)
                                        {
                                            attribute.AddArgument($"typeof({GetTypeName(property.TypeReference)})");
                                            attribute.AddArgument($"\"{property.Value.AsFormattedValidTypeValue(this, property.TypeReference)}\"");
                                        }
                                        else
                                        {
                                            attribute.AddArgument(property.Value.AsFormattedValidTypeValue(this, property.TypeReference));
                                        }
                                    });
                                }

                                if (property.HasStereotype("OpenAPI Settings")
                                    && !string.IsNullOrWhiteSpace(property.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value))
                                {
                                    prop.WithComments(xmlComments: $"/// <example>{property.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value}</example>");
                                }

                                // Do the assignment in the constructor, if the parameter has a default value, we need to use the null-coalescing operator to assign the default value to the property if the parameter is null
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

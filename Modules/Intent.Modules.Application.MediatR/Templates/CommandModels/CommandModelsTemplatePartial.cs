using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.DependencyInjection.MediatR;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Metadata.Security.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.CommandModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class CommandModelsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.CommandModels";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommandModelsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatR(outputTarget));

            FulfillsRole("Application.Contract.Command");

            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            CSharpFile = new CSharpFile($"{this.GetCommandNamespace()}", $"{this.GetCommandFolderPath()}")
                .AddUsing("MediatR")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.RepresentsModel(model);
                    @class.TryAddXmlDocComments(Model.InternalElement);
                    CqrsTemplateHelpers.AddAuthorization(this, @class, Model.InternalElement);
                    @class.ImplementsInterface(Model.TypeReference.Element != null ? $"IRequest<{GetTypeName(Model.TypeReference)}>" : "IRequest");
                    @class.ImplementsInterface(this.GetCommandInterfaceName());

                    @class.AddConstructor();
                    var ctor = @class.Constructors.First();

                    // get the last property which has no value. All items occuring before this cannot have a default value set in the constructor
                    var lastNonNullable = Model.Properties.LastOrDefault(p => string.IsNullOrEmpty(p.Value))?.InternalElement.Order ?? 0;

                    List<string> nulledFields = [];
                    foreach (var property in Model.Properties)
                    {
                        // should the default value be set, based on the position of it as a argument
                        var setDefaultValue = property.ShouldSetDefaultValue(lastNonNullable);
                        // set the type
                        var typeValue = property.GetTypeReferenceName(setDefaultValue, this);

                        ctor.AddParameter(typeValue, property.Name.ToParameterName(), param =>
                        {
                            param.AddMetadata("model", property);

                            // only parameters with a value AFTER the last parameter with a value get the value specified
                            if (setDefaultValue)
                            {
                                param.WithDefaultValue(property.Value);

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
                                prop.AddMetadata("model", property);
                                prop.TryAddXmlDocComments(property.InternalElement);
                                prop.RepresentsModel(property);

                                if (property.HasStereotype("OpenAPI Settings")
                                    && !string.IsNullOrWhiteSpace(property.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value))
                                {
                                    prop.WithComments(xmlComments: $"/// <example>{property.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value}</example>");
                                }

                                // Do the assigmentment in the constructor, if the parameter has a default value, we need to use the null-coalescing operator to assign the default value to the property if the parameter is null
                                var rhs = setDefaultValue && nulledFields.Contains(property.Id) ? $"{property.Name.ToParameterName()} ?? {property.Value}" :
                                    property.Name.ToParameterName();
                                var assignmentStatement = new CSharpFieldAssignmentStatement(prop.Name, rhs);
                                ctor.AddStatement(assignmentStatement);
                            });
                        });
                    }
                });

            if (ExecutionContext.Settings.GetCQRSSettings().ConsolidateCommandQueryAssociatedFilesIntoSingleFile())
            {
                FulfillsRole("Application.Command.Handler");
                FulfillsRole(TemplateRoles.Application.Validation.Command);
                CommandHandlerTemplate.Configure(this, model);
            }
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
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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

                    foreach (var property in Model.Properties)
                    {
                        ctor.AddParameter(GetTypeName(property), property.Name.ToParameterName(), param =>
                        {
                            param.AddMetadata("model", property);
                            param.IntroduceProperty(prop =>
                            {
                                prop.TryAddXmlDocComments(property.InternalElement);
                                prop.RepresentsModel(property);

                                if (property.HasStereotype("OpenAPI Settings")
                                    && !string.IsNullOrWhiteSpace(property.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value))
                                {
                                    prop.WithComments(xmlComments: $"/// <example>{property.GetStereotype("OpenAPI Settings").GetProperty("Example Value")?.Value}</example>");
                                }

                            });

                            // only parameters with a value AFTER the last parameter with a value get the value specified
                            if (property.InternalElement.Order >= lastNonNullable && !string.IsNullOrEmpty(property.Value))
                            {
                                param.WithDefaultValue(property.Value);
                            }
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
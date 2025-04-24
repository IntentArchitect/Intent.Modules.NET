using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.QueryHandler;
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

namespace Intent.Modules.Application.MediatR.Templates.QueryModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class QueryModelsTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.QueryModels";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryModelsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.MediatR(outputTarget));

            FulfillsRole("Application.Contract.Query");

            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Clients.Enum);

            CSharpFile = new CSharpFile($"{this.GetQueryNamespace()}", $"{this.GetQueryFolderPath()}")
                .AddUsing("MediatR")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.TryAddXmlDocComments(Model.InternalElement);
                    CqrsTemplateHelpers.AddAuthorization(this, @class, Model.InternalElement);
                    @class.ImplementsInterface($"IRequest<{GetTypeName(Model.TypeReference)}>");
                    @class.ImplementsInterface(this.GetQueryInterfaceName());

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
                FulfillsRole("Application.Query.Handler");
                FulfillsRole(TemplateRoles.Application.Validation.Query);
                QueryHandlerTemplate.Configure(this, model);
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
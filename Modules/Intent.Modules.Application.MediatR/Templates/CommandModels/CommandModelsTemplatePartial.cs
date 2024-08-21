using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.Api;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Settings;
using Intent.Modules.Application.MediatR.Templates.CommandHandler;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
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
                    AddAuthorization(@class);
                    @class.ImplementsInterface(Model.TypeReference.Element != null ? $"IRequest<{GetTypeName(Model.TypeReference)}>" : "IRequest");
                    @class.ImplementsInterface(this.GetCommandInterfaceName());

                    @class.AddConstructor();
                    var ctor = @class.Constructors.First();
                    foreach (var property in Model.Properties)
                    {
                        ctor.AddParameter(GetTypeName(property), property.Name.ToParameterName(), param =>
                        {
                            param.AddMetadata("model", property);
                            param.IntroduceProperty(prop => prop.RepresentsModel(property));
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

        private void AddAuthorization(CSharpClass @class)
        {
            if (Model.HasAuthorize())
            {
                if (!string.IsNullOrWhiteSpace(Model.GetAuthorize().Roles()) && Model.GetAuthorize().Roles().Contains('+'))
                {
                    var roleGroups = Model.GetAuthorize().Roles().Split('+');
                    foreach (var group in roleGroups)
                    {
                        @class.AddAttribute(TryGetTypeName("Application.Identity.AuthorizeAttribute")?.RemoveSuffix("Attribute") ?? "Authorize", att =>
                        {
                            att.AddArgument($"Roles = \"{group}\"");
                        });
                    }
                }
                else
                {
                    var rolesPolicies = new List<string>();
                    if (!string.IsNullOrWhiteSpace(Model.GetAuthorize().Roles()))
                    {
                        rolesPolicies.Add($"Roles = \"{Model.GetAuthorize().Roles()}\"");
                    }
                    else if (Model.GetAuthorize().SecurityRoles().Any())
                    {
                        rolesPolicies.Add($"Roles = \"{string.Join(",", Model.GetAuthorize().SecurityRoles().Select(e => e.Name))}\"");
                    }
                    if (!string.IsNullOrWhiteSpace(Model.GetAuthorize().Policy()))
                    {
                        rolesPolicies.Add($"Policy = \"{Model.GetAuthorize().Policy()}\"");
                    }
                    else if (Model.GetAuthorize().SecurityPolicies().Any())
                    {
                        rolesPolicies.Add($"Policy = \"{string.Join(",", Model.GetAuthorize().SecurityPolicies().Select(e => e.Name))}\"");
                    }

                    @class.AddAttribute(TryGetTypeName("Application.Identity.AuthorizeAttribute")?.RemoveSuffix("Attribute") ?? "Authorize", att =>
                    {
                        foreach (var arg in rolesPolicies)
                        {
                            att.AddArgument(arg);
                        }
                    });
                }
            }
        }
    }
}
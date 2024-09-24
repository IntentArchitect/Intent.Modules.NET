using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Blazor.FluentValidation.Templates;
using Intent.Modules.Blazor.HttpClients.Templates.DtoContract;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Contracts.Clients.Shared;
using Intent.Modules.FluentValidation.Shared;
using Intent.RoslynWeaver.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.Dtos.FluentValidation.Templates.DtoValidator
{
    [IntentManaged(Mode.Merge)]
    public partial class DtoValidatorTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate, IFluentValidationTemplate
    {
        public const string TemplateId = "Intent.Blazor.HttpClients.Dtos.FluentValidation.DtoValidator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoValidatorTemplate(IOutputTarget outputTarget, DTOModel model)
            : base(TemplateId, outputTarget, model)
        {
            FulfillsRole(TemplateRoles.Blazor.Client.Model.Validator);
            CSharpFile = new CSharpFile(@namespace: ExtensionMethods.GetPackageBasedNamespace(model, outputTarget),
                relativeLocation: ExtensionMethods.GetPackageBasedRelativeLocation(model, outputTarget));

            this.ConfigureForValidation(
                dtoModel: model.InternalElement,
                configureFieldValidations: [
                    (methodChain, field) => methodChain.AddCustomValidations(this, field),
                    (methodChain, field) => methodChain.AddMaxLengthValidatorsFromMappedDomain(Model.InternalElement, field),
                    (methodChain, field) => methodChain.AddCheckUniqueConstraintsPlaceholdersForField(this, Model.InternalElement, field)
                ],
                configureClassValidations: [
                    (methodChain) => methodChain.AddCheckUniqueConstraintsPlaceholders(this, Model.InternalElement)
                ]);
        }

        public string ToValidateTemplateId => DtoContractTemplate.TemplateId;
        public string DtoTemplateId => DtoContractTemplate.TemplateId;
        public string ValidatorProviderTemplateId => "Blazor.Client.Validation.ValidatorProviderInterface";

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

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "2.0"), new ConstructorSignatureMigration());
        }

        public class ConstructorSignatureMigration : ITemplateMigration
        {
            public string Execute(string currentText)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(currentText);

                var root = syntaxTree.GetRoot();
                using var workspace = new AdhocWorkspace();

                var services = workspace.Services;
                var editor = new SyntaxEditor(root, services);

                var attributeLists = root.DescendantNodes()
                    .OfType<ConstructorDeclarationSyntax>()
                    .SelectMany(c => c.AttributeLists);

                foreach (var attributeList in attributeLists)
                {
                    var intentManagedAttributes = attributeList.Attributes
                        .Where(attr => attr.Name.ToString().EndsWith("IntentManaged"));

                    foreach (var attribute in intentManagedAttributes)
                    {
                        var newArgumentList = SyntaxFactory.ParseAttributeArgumentList("(Mode.Merge)");

                        editor.ReplaceNode(attribute, attribute.WithArgumentList(newArgumentList));
                    }
                }

                var newRoot = editor.GetChangedRoot();

                return newRoot.ToFullString();
            }

            public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
        }
    }
}
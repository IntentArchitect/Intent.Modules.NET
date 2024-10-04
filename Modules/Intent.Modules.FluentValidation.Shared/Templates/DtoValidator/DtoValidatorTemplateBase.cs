using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Intent.Modules.FluentValidation.Shared.Templates.DtoValidator;

public abstract class DtoValidatorTemplateBase : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
{
    protected DtoValidatorTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        DTOModel model,
        string toValidateTemplateId,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        string modelParameterName,
        string validatorProviderInterfaceTemplateId,
        bool uniqueConstraintValidationEnabled,
        bool repositoryInjectionEnabled,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd> associationedElements,
        params string[] additionalFolders)
        : this(
            templateId: templateId,
            outputTarget: outputTarget,
            dtoModel: model,
            toValidateTemplateId: toValidateTemplateId,
            dtoTemplateId: dtoTemplateId,
            dtoValidatorTemplateId: dtoValidatorTemplateId,
            modelParameterName: modelParameterName,
            @namespace: null,
            relativeLocation: null,
            validatorProviderInterfaceTemplateId: validatorProviderInterfaceTemplateId,
            uniqueConstraintValidationEnabled: uniqueConstraintValidationEnabled,
            repositoryInjectionEnabled: repositoryInjectionEnabled,
            customValidationEnabled: customValidationEnabled,
            associationedElements: associationedElements,
            additionalFolders: additionalFolders)
    {
    }

    protected DtoValidatorTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        DTOModel model,
        string toValidateTemplateId,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        string modelParameterName,
        string @namespace,
        string relativeLocation,
        string validatorProviderInterfaceTemplateId,
        bool uniqueConstraintValidationEnabled,
        bool repositoryInjectionEnabled,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd> associationedElements)
        : this(
            templateId: templateId,
            outputTarget: outputTarget,
            dtoModel: model,
            toValidateTemplateId: toValidateTemplateId,
            dtoTemplateId: dtoTemplateId,
            dtoValidatorTemplateId: dtoValidatorTemplateId,
            modelParameterName: modelParameterName,
            @namespace: @namespace,
            relativeLocation: relativeLocation,
            validatorProviderInterfaceTemplateId: validatorProviderInterfaceTemplateId,
            uniqueConstraintValidationEnabled: uniqueConstraintValidationEnabled,
            repositoryInjectionEnabled: repositoryInjectionEnabled,
            customValidationEnabled: customValidationEnabled,
            associationedElements: associationedElements,
            additionalFolders: Array.Empty<string>())
    {
    }

    private DtoValidatorTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        DTOModel dtoModel,
        string toValidateTemplateId,
        string dtoTemplateId,
        string dtoValidatorTemplateId,
        string modelParameterName,
        string @namespace,
        string relativeLocation,
        string validatorProviderInterfaceTemplateId,
        bool uniqueConstraintValidationEnabled,
        bool repositoryInjectionEnabled,
        bool customValidationEnabled,
        IEnumerable<IAssociationEnd> associationedElements,
        params string[] additionalFolders)
        : base(templateId, outputTarget, dtoModel)
    {
        AddNugetDependency(NugetPackages.FluentValidation(outputTarget));

        CSharpFile = new CSharpFile(
            @namespace: @namespace ?? this.GetNamespace(additionalFolders),
            relativeLocation: relativeLocation ?? this.GetFolderPath(additionalFolders));

        this.ConfigureForValidation(
            dtoModel: dtoModel,
            toValidateTemplateId: toValidateTemplateId,
            modelParameterName: modelParameterName,
            dtoTemplateId: dtoTemplateId,
            dtoValidatorTemplateId: dtoValidatorTemplateId,
            validatorProviderInterfaceTemplateId: validatorProviderInterfaceTemplateId,
            uniqueConstraintValidationEnabled: uniqueConstraintValidationEnabled,
            repositoryInjectionEnabled: repositoryInjectionEnabled,
            customValidationEnabled: customValidationEnabled,
            associationedElements: associationedElements);
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
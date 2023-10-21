using System;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

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
        bool repositoryInjectionEnabled)
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
        params string[] additionalFolders)
        : base(templateId, outputTarget, dtoModel)
    {
        AddNugetDependency(NuGetPackages.FluentValidation);

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
            repositoryInjectionEnabled: repositoryInjectionEnabled);
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

    private class ConstructorSignatureMigration : ITemplateMigration
    {
        public string Execute(string currentText)
        {
            const string pattern = @"\[IntentManaged\((Mode\.Fully[^]]+)\)\]";
            int counter = 0;  // Counter to keep track of replacements made

            return Regex.Replace(currentText, pattern, match =>
            {
                // If the Mode.Fully pattern is detected and this is the first occurrence, replace it
                if (counter == 0 && match.Groups[1].Value.Contains("Mode.Fully"))
                {
                    counter++;  // Increase the counter to prevent further replacements
                    return "[IntentManaged(Mode.Merge)]";
                }
                // If not the first occurrence or Mode.Fully is not detected, keep the original string
                return match.Value;
            });
        }

        public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
    }
}
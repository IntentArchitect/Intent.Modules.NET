using System;
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
        string validatorProviderInterfaceTemplateName,
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
            validatorProviderInterfaceTemplateName: validatorProviderInterfaceTemplateName,
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
        string validatorProviderInterfaceTemplateName)
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
            validatorProviderInterfaceTemplateName: validatorProviderInterfaceTemplateName,
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
        string validatorProviderInterfaceTemplateName,
        params string[] additionalFolders)
        : base(templateId, outputTarget, dtoModel)
    {
        AddNugetDependency(NuGetPackages.FluentValidation);

        CSharpFile = new CSharpFile(
                @namespace: @namespace ?? this.GetNamespace(additionalFolders),
                relativeLocation: relativeLocation ?? this.GetFolderPath(additionalFolders))
            .AddUsing("FluentValidation")
            .AddClass($"{Model.Name}Validator", @class =>
            {
                this.ConfigureForValidation(
                    validatorClass: @class,
                    dtoModel: dtoModel,
                    toValidateTypeName: GetTypeName(toValidateTemplateId, Model),
                    modelParameterName: modelParameterName,
                    dtoTemplateId: dtoTemplateId,
                    dtoValidatorTemplateId: dtoValidatorTemplateId,
                    validatorProviderInterfaceTemplateName: validatorProviderInterfaceTemplateName);
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
    
    public override RoslynMergeConfig ConfigureRoslynMerger()
    {
        return new RoslynMergeConfig(new TemplateMetadata(Id, "2.0"), new ConstructorSignatureMigration());
    }

    private class ConstructorSignatureMigration : ITemplateMigration
    {
        public string Execute(string currentText)
        {
            return currentText
                .Replace(
                    "[IntentManaged(Mode.Fully, Body = Mode.Ignore",
                    "[IntentManaged(Mode.Fully, Body = Mode.Merge");
        }

        public TemplateMigrationCriteria Criteria => TemplateMigrationCriteria.Upgrade(1, 2);
    }
}
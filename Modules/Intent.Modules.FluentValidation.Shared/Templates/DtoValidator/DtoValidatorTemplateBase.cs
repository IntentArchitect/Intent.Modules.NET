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
        string dtoTemplateId,
        string modelParameterName,
        params string[] additionalFolders)
        : this(
            templateId: templateId,
            outputTarget: outputTarget,
            model: model,
            dtoTemplateId: dtoTemplateId,
            modelParameterName: modelParameterName,
            @namespace: null,
            relativeLocation: null,
            additionalFolders: additionalFolders)
    {
    }

    protected DtoValidatorTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        DTOModel model,
        string dtoTemplateId,
        string modelParameterName,
        string @namespace,
        string relativeLocation)
        : this(
            templateId: templateId,
            outputTarget: outputTarget,
            model: model,
            dtoTemplateId: dtoTemplateId,
            modelParameterName: modelParameterName,
            @namespace: @namespace,
            relativeLocation: relativeLocation,
            additionalFolders: Array.Empty<string>())
    {
    }

    private DtoValidatorTemplateBase(
        string templateId,
        IOutputTarget outputTarget,
        DTOModel model,
        string dtoTemplateId,
        string modelParameterName,
        string @namespace,
        string relativeLocation,
        params string[] additionalFolders)
        : base(templateId, outputTarget, model)
    {
        AddNugetDependency(NuGetPackages.FluentValidation);

        CSharpFile = new CSharpFile(
                @namespace: @namespace ?? this.GetNamespace(additionalFolders),
                relativeLocation: relativeLocation ?? this.GetFolderPath(additionalFolders))
            .AddUsing("FluentValidation")
            .AddClass($"{Model.Name}Validator", @class =>
            {
                // To prevent SF noise after the refactor of all validators into using this as a base class, we're excluding
                // the following template ids (ideally they shouldn't be):
                if (templateId is not (
                    "Intent.Application.MediatR.FluentValidation.CommandValidator" or
                    "Intent.Application.MediatR.FluentValidation.QueryValidator"))
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                }

                this.ConfigureForValidation(
                    @class: @class,
                    properties: Model.Fields,
                    modelTypeName: GetTypeName(dtoTemplateId, Model),
                    modelParameterName: modelParameterName);
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
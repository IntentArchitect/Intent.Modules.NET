using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.JsonPatch.Templates.Templates.PatchIgnoreMetadataProvider
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PatchIgnoreMetadataProviderTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Controllers.JsonPatch.Templates.PatchIgnoreMetadataProvider";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PatchIgnoreMetadataProviderTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Mvc.ModelBinding.Metadata")
                .AddClass("PatchIgnoreMetadataProvider", @class =>
                {
                    @class.WithComments(
                        $"""
                        /// <summary>
                        /// Tells ASP.NET Core's model binding to skip validation on properties 
                        /// that are read-only or specifically of type {this.GetPatchExecutorInterfaceName()}.
                        /// </summary>
                        """);

                    @class.ImplementsInterface("IValidationMetadataProvider");

                    @class.AddMethod("void", "CreateValidationMetadata", method =>
                    {
                        method.AddParameter("ValidationMetadataProviderContext", "context");

                        method.AddIfStatement("context.Key.PropertyInfo == null", @if => { @if.AddReturn(""); });

                        method.AddIfStatement("!context.Key.PropertyInfo.CanWrite || context.Key.PropertyInfo.GetSetMethod(nonPublic: false) == null",
                            ifStmt =>
                            {
                                ifStmt.AddStatement("context.ValidationMetadata.ValidateChildren = false;");
                                ifStmt.AddStatement("context.ValidationMetadata.HasValidators = false;");
                            });

                        method.AddStatement("var propertyType = context.Key.PropertyInfo.PropertyType;");
                        method.AddIfStatement($"propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof({this.GetPatchExecutorInterfaceName()}<>)",
                            ifStmt =>
                            {
                                ifStmt.AddStatement("context.ValidationMetadata.ValidateChildren = false;");
                                ifStmt.AddStatement("context.ValidationMetadata.HasValidators = false;");
                            });
                    });
                });
        }

        public override void AfterTemplateRegistration()
        {
            var templates = ExecutionContext.FindTemplateInstances<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            foreach (var startupTemplate in templates)
            {
                startupTemplate.CSharpFile.OnBuild(file =>
                {
                    startupTemplate.StartupFile.AddServiceConfigurationLambda(
                        methodName: "AddControllers",
                        parameters: ["opt"],
                        configure: (statement, lambda, context) =>
                        {
                            lambda.AddStatement($"{context.Parameters[0]}.ModelMetadataDetailsProviders.Add(new {startupTemplate.GetPatchIgnoreMetadataProviderName()}());");
                        });
                }, 1);
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
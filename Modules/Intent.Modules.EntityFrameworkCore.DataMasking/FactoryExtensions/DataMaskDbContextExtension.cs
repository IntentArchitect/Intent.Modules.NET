using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.DataMasking.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.DataMasking.Templates.DataMaskConverter;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DataMasking.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DataMaskDbContextExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.DataMasking.DataMaskDbContextExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext);
            template?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();

                var ctor = @class.Constructors.First();
                if (!ctor.Parameters.Any(p => p.Name == "currentUserService" && p.Type == template.GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface")))
                {
                    @class.Constructors.First().AddParameter(template.GetTypeName("Intent.Application.Identity.CurrentUserServiceInterface"), "currentUserService",
                         param => { param.IntroduceReadonlyField(); });
                }

                var saveMethod = template.GetSaveChangesMethod();
                saveMethod.InsertStatement(0, "PreventMaskedDataSave();");

                var saveAsyncMethod = template.GetSaveChangesAsyncMethod();
                saveAsyncMethod.InsertStatement(0, "PreventMaskedDataSave();");

                @class.AddMethod("void", "PreventMaskedDataSave", method =>
                {
                    var invocation = new CSharpInvocationStatement("ChangeTracker.Entries")
                        .OnNewLine()
                        .AddInvocation("Where", cfg =>
                        {
                            cfg.AddArgument("t => t.State == EntityState.Modified");
                        })
                        .OnNewLine()
                        .AddInvocation("SelectMany", cfg =>
                        {
                            cfg.AddArgument("t => t.Properties");
                        })
                        .OnNewLine()
                        .AddInvocation("Where", cfg =>
                        {
                            cfg.AddArgument($"p => p.Metadata?.GetValueConverter() is {template.GetTypeName(DataMaskConverterTemplate.TemplateId)} dataConverter && dataConverter.IsMasked()");
                        })
                        .OnNewLine();

                    method.AddObjectInitStatement("var properties", invocation);

                    method.AddForEachStatement("prop", "properties", @for =>
                    {
                        @for.AddStatement("prop.IsModified = false;");
                        @for.SeparatedFromPrevious();
                    });
                });

                var entities = application.MetadataManager.Domain(application).GetClassModels().Where(c => c.Attributes.Any(a => a.HasDataMasking()));
                var modelCreatingMethod = @class.Methods.FirstOrDefault(m => m.Name == "OnModelCreating");

                foreach (var entity in entities)
                {
                    // this should always return a valid statement
                    var statement = modelCreatingMethod.Statements.FirstOrDefault(s => s.Text.StartsWith("modelBuilder.ApplyConfiguration")
                    && s.TryGetMetadata("model", out ClassModel model) && model.Name == entity.Name);

                    statement?.FindAndReplace("Configuration()", "Configuration(_currentUserService)");
                }

            }, 1000);
        }
    }
}
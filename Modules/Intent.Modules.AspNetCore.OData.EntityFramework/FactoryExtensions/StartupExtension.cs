using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Templates.DomainEntity;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.OData.EntityFramework.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class StartupExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.OData.EntityFramework.StartupExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        private void TraverseAssociatedChildren(IEnumerable<AssociationEndModel> associations, Dictionary<string, IEnumerable<AttributeModel>> exposedAsODataEntityNames, ICSharpFileBuilderTemplate template,
            IAppStartupTemplate startupTemplate)
        {
            foreach (var association in associations.Where(a => a.Association.AssociationType == AssociationType.Composition))
            {
                var classModel = association.Class;
                template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, classModel, out ICSharpFileBuilderTemplate entityTemplate);
                var entityName = startupTemplate.GetTypeName(entityTemplate);
                if (exposedAsODataEntityNames.ContainsKey(entityName))
                    continue;
                var primaryKeys = classModel.Attributes.Where(att => att.HasStereotype("Primary Key"));
                exposedAsODataEntityNames.Add(entityName, primaryKeys);
                TraverseAssociatedChildren(classModel.AssociatedClasses, exposedAsODataEntityNames, template, startupTemplate);
            }
        }

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary);

            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate is null)
            {
                return;
            }

            var exposedAsODataEntityNames = new Dictionary<string, IEnumerable<AttributeModel>>();
            foreach (var template in templates)
            {
                template.TryGetModel<ClassModel>(out var templateModel);
                if (templateModel.HasStereotype("Expose As OData"))
                {
                    template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, templateModel, out ICSharpFileBuilderTemplate entityTemplate);
                    var entityName = startupTemplate.GetTypeName(entityTemplate);
                    var primaryKeys = templateModel.Attributes.Where(att => att.HasStereotype("Primary Key"));
                    exposedAsODataEntityNames.Add(entityName, primaryKeys);
                    TraverseAssociatedChildren(templateModel.AssociatedClasses, exposedAsODataEntityNames, template, startupTemplate);
                }
            }

            if (exposedAsODataEntityNames.Count <= 0)
            {
                return;
            }

            startupTemplate.CSharpFile.OnBuild(file =>
            {
                startupTemplate.StartupFile.ConfigureServices((statements, context) =>
                {
                    // Until we can make the "AddController" statement in the Intent.AspNetCore.Controllers be
                    // a CSharpInvocationStatement that supports method chaining, this will have to do.
                    // It's our original hack approach anyway and turning this into a CSharpMethodChainStatement will
                    // only make the CSharpInvocationStatement change later difficult. 
                    file.AfterBuild(nestedFile =>
                    {
                        nestedFile.AddUsing("Microsoft.AspNetCore.OData");
                        nestedFile.AddUsing("Microsoft.OData.ModelBuilder");

                        var statementsToCheck = new List<CSharpStatement>();
                        ExtractPossibleStatements(statements, statementsToCheck);

                        var lastConfigStatement = (CSharpInvocationStatement)statementsToCheck.Last(p => p.HasMetadata("configure-services-controllers"));
                        var addODataStatement = statements.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "odata")
                            as CSharpInvocationStatement;
                        if (addODataStatement is null)
                        {
                            addODataStatement = new CSharpInvocationStatement(".AddOData");
                            addODataStatement.AddMetadata("configure-services-controllers", "odata");
                            lastConfigStatement.InsertBelow(addODataStatement);
                        }

                        lastConfigStatement.WithoutSemicolon();

                        var lambda = addODataStatement.Statements.FirstOrDefault() as CSharpLambdaBlock;
                        if (lambda is null)
                        {
                            lambda = new CSharpLambdaBlock("options");
                            addODataStatement.AddArgument(lambda);
                        }

                        lambda.AddStatement($"var odataBuilder = new ODataConventionModelBuilder();");

                        foreach (var entityName in exposedAsODataEntityNames)
                        {
                            lambda.AddStatement($"""odataBuilder.EntitySet<{entityName.Key}>("{entityName.Key.Pluralize()}");""");
                            if (entityName.Value.Count() > 1)
                            {
                                // Has more than one key add key bindings on model.
                                foreach (var key in entityName.Value)
                                {
                                    lambda.AddStatement($"""odataBuilder.EntitySet<{entityName.Key}>("{entityName.Key.Pluralize()}").EntityType.HasKey(m => m.{key.Name});""");
                                }
                            }
                        }

                        lambda.AddStatement("options", c => c
                               .AddInvocation("Select")
                               .AddInvocation("Expand", c => c.OnNewLine())
                               .AddInvocation("Filter", c => c.OnNewLine())
                               .AddInvocation("OrderBy", c => c.OnNewLine())
                               .AddInvocation("SetMaxTop", c => c.AddArgument("100").OnNewLine())
                               .AddInvocation("Count", c => c.OnNewLine())
                               .AddInvocation("AddRouteComponents", c => c.AddArgument("\"odata\", odataBuilder.GetEdmModel()").OnNewLine()));

                        addODataStatement.WithSemicolon();
                    });
                });
            }, 16);
        }

        private static void ExtractPossibleStatements(IHasCSharpStatements targetBlock, List<CSharpStatement> statementsToCheck)
        {
            foreach (var statement in targetBlock.Statements)
            {
                if (statement is CSharpInvocationStatement)
                {
                    statementsToCheck.Add(statement);
                }
                else if (statement is IHasCSharpStatements container)
                {
                    foreach (var nested in container.Statements)
                    {
                        statementsToCheck.Add(nested);
                    }
                }
            }
        }
    }
}
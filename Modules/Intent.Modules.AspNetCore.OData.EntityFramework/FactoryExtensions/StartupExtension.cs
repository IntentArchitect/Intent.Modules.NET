using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.AppStartup;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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
            var startupTemplate = application.FindTemplateInstance<IAppStartupTemplate>(IAppStartupTemplate.RoleName);
            if (startupTemplate is null)
            {
                return;
            }

            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary);

            var exposedAsODataEntityNames = new Dictionary<string, IEnumerable<AttributeModel>>();
            foreach (var template in templates)
            {
                if (!template.TryGetModel<ClassModel>(out var templateModel) ||
                    !templateModel.HasStereotype("Expose As OData"))
                {
                    continue;
                }

                template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, templateModel, out ICSharpFileBuilderTemplate entityTemplate);
                var entityName = startupTemplate.GetTypeName(entityTemplate);
                var primaryKeys = templateModel.Attributes.Where(att => att.HasStereotype("Primary Key"));
                exposedAsODataEntityNames.Add(entityName, primaryKeys);
                TraverseAssociatedChildren(templateModel.AssociatedClasses, exposedAsODataEntityNames, template, startupTemplate);
            }

            if (exposedAsODataEntityNames.Count <= 0)
            {
                return;
            }

            startupTemplate.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("Microsoft.AspNetCore.OData");
                file.AddUsing("Microsoft.OData.ModelBuilder");

                startupTemplate.StartupFile.ConfigureServices((statements, _) =>
                {
                    // Until we can make the "AddController" statement in the Intent.AspNetCore.Controllers be
                    // a CSharpInvocationStatement that supports method chaining, this will have to do.
                    // It's our original hack approach anyway and turning this into a CSharpMethodChainStatement will
                    // only make the CSharpInvocationStatement change later difficult. 
                    var statementsToCheck = ExtractPossibleStatements(statements);

                    var lastConfigStatement = (CSharpInvocationStatement)statementsToCheck.Last(p => p.HasMetadata("configure-services-controllers"));
                    var addODataStatement = statements.FindStatement(s => s.TryGetMetadata<string>("configure-services-controllers", out var v) && v == "odata") as CSharpInvocationStatement;
                    if (addODataStatement is null)
                    {
                        addODataStatement = new CSharpInvocationStatement(".AddOData");
                        addODataStatement.AddMetadata("configure-services-controllers", "odata");
                        lastConfigStatement.InsertBelow(addODataStatement);
                    }

                    lastConfigStatement.WithoutSemicolon();

                    if (addODataStatement.Statements.FirstOrDefault() is not CSharpLambdaBlock lambda)
                    {
                        lambda = new CSharpLambdaBlock("options");
                        addODataStatement.AddArgument(lambda);
                    }

                    lambda.AddStatement("var odataBuilder = new ODataConventionModelBuilder();");

                    foreach (var entityName in exposedAsODataEntityNames)
                    {
                        lambda.AddStatement($"""odataBuilder.EntitySet<{entityName.Key}>("{entityName.Key.Pluralize()}");""");

                        if (entityName.Value.Count() <= 1)
                        {
                            continue;
                        }

                        foreach (var key in entityName.Value)
                        {
                            lambda.AddStatement($"""odataBuilder.EntitySet<{entityName.Key}>("{entityName.Key.Pluralize()}").EntityType.HasKey(m => m.{key.Name});""");
                        }
                    }

                    foreach (var template in templates)
                    {
                        if (!template.TryGetModel<ClassModel>(out var templateModel) ||
                            !templateModel.HasStereotype("Expose As OData"))
                        {
                            continue;
                        }

                        var @class = template.CSharpFile.Classes.First();
                        foreach (var property in @class.Properties)
                        {
                            if (property.TryGetMetadata("non-persistent", out bool nonPersistent) && nonPersistent)
                            {
                                lambda.AddStatement($"""odataBuilder.EntitySet<{@class.Name}>("{@class.Name.Pluralize()}").EntityType.Ignore(m => m.{property.Name});""");
                            }
                        }
                    }

                    lambda.AddStatement("options", statement => statement
                        .AddInvocation("Select")
                        .AddInvocation("Expand", c => c.OnNewLine())
                        .AddInvocation("Filter", c => c.OnNewLine())
                        .AddInvocation("OrderBy", c => c.OnNewLine())
                        .AddInvocation("SetMaxTop", c => c.AddArgument("100").OnNewLine())
                        .AddInvocation("Count", c => c.OnNewLine())
                        .AddInvocation("AddRouteComponents", c => c.AddArgument("\"odata\", odataBuilder.GetEdmModel()").OnNewLine()));

                    addODataStatement.WithSemicolon();
                });
            }, 16);
        }

        private static void TraverseAssociatedChildren(IEnumerable<AssociationEndModel> associations, Dictionary<string, IEnumerable<AttributeModel>> exposedAsODataEntityNames, ICSharpFileBuilderTemplate template,
            IAppStartupTemplate startupTemplate)
        {
            foreach (var association in associations.Where(a => a.Association.AssociationType == AssociationType.Composition))
            {
                var classModel = association.Class;
                if (!template.TryGetTemplate(TemplateRoles.Domain.Entity.Primary, classModel, out ICSharpFileBuilderTemplate entityTemplate))
                {
                    continue;
                }

                var entityName = startupTemplate.GetTypeName(entityTemplate);
                if (exposedAsODataEntityNames.ContainsKey(entityName))
                {
                    continue;
                }

                var primaryKeys = classModel.Attributes.Where(att => att.HasStereotype("Primary Key"));
                exposedAsODataEntityNames.Add(entityName, primaryKeys);
                TraverseAssociatedChildren(classModel.AssociatedClasses, exposedAsODataEntityNames, template, startupTemplate);
            }
        }

        private static IEnumerable<CSharpStatement> ExtractPossibleStatements(IHasCSharpStatements targetBlock)
        {
            foreach (var statement in targetBlock.Statements)
            {
                if (statement is CSharpInvocationStatement)
                {
                    yield return statement;
                }
                else if (statement is IHasCSharpStatements container)
                {
                    foreach (var nested in container.Statements)
                    {
                        yield return nested;
                    }
                }
            }
        }
    }
}
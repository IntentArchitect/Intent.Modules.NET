using System;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using static Intent.Modules.Constants.TemplateRoles.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.ModularMonolith.Module.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CrudUnitOfWorkSaveExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.ModularMonolith.Module.CrudUnitOfWorkSaveExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            MakeSureUnitOfWorkIsRegistered(application);
            DoCommandHandlers(application);
            DoServiceOperations(application);
        }

        private void MakeSureUnitOfWorkIsRegistered(IApplication application)
        {
            
            var containerRegistrations = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Infrastructure.DependencyInjection.DependencyInjection");

            containerRegistrations?.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var regMethod = @class.FindMethod("AddInfrastructure");
                if (regMethod == null)
                {
                    return;
                }
                if (regMethod.FindStatement(s => s.GetText("").StartsWith("services.AddScoped<IUnitOfWork>"))  is null)
                {       
                    
                    regMethod.Statements.Last().InsertAbove($"services.AddScoped<{containerRegistrations.GetTypeName("Intent.Entities.Repositories.Api.UnitOfWorkInterface")}>(sp => sp.GetRequiredService<{containerRegistrations.GetTypeName("Intent.EntityFrameworkCore.DbContext")}>());");
                }

            }, 1000);
        }

        private void DoServiceOperations(IApplication application)
        {
            var serviceImplementationTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Application.ServiceImplementations.ServiceImplementation"));

            foreach (var template in serviceImplementationTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();

                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<OperationModel>("model", out var model))
                        {
                            if (HasCrudImplementation(model.InternalElement))
                            {
                                ImplementUOWSave(template, @class, method);
                            }
                        }
                    }
                }, 1000);
            }
        }

        private void DoCommandHandlers(IApplication application)
        {
            var commandHandlerTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Intent.Application.MediatR.CommandHandler"));
            foreach (var template in commandHandlerTemplates)
            {
                var templateModel = ((CSharpTemplateBase<CommandModel>)template).Model;

                if (!HasCrudImplementation(templateModel.InternalElement))
                {
                    continue;
                }

                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("Handle");
                    ImplementUOWSave(template, @class, method);

                }, 1000);
            }
        }

        private static void ImplementUOWSave(ICSharpFileBuilderTemplate template, CSharpClass @class, CSharpClassMethod method)
        {
            if (!method.Statements.Any() || method.Statements.Last().GetText("").Trim().StartsWith("throw"))
            {
                return;
            }

            string cancellationTokenName = method.Parameters.FirstOrDefault(p => p.Type.EndsWith("CancellationToken"))?.Name ?? "";
            var commitClause = $"await _unitOfWork.SaveChangesAsync({cancellationTokenName});";
            CSharpStatement lastNonReturnStatement;
            if (method.Statements.Last().GetText("").Trim().StartsWith("return"))
            {
                lastNonReturnStatement = method.Statements[method.Statements.Count - 2];
            }
            else
            {
                lastNonReturnStatement = method.Statements.Last();
            }
            if (!lastNonReturnStatement.GetText("").Contains(".SaveChanges"))
            {
                lastNonReturnStatement.InsertBelow(commitClause, s => s.SeparatedFromPrevious());

                var ctor = @class.Constructors.FirstOrDefault();
                if (ctor == null)
                {
                    return;
                }
                var unitOfWorkTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>("Intent.Entities.Repositories.Api.UnitOfWorkInterface");
                ctor.AddParameter(template.GetTypeName(unitOfWorkTemplate), "unitOfWork", p => p.IntroduceReadonlyField());

            }
        }

        private bool HasCrudImplementation(IElement internalElement)
        {
            return internalElement.AssociatedElements.Any(ae => IsCrudAssociation(ae.Association));
        }

        private bool IsCrudAssociation(IAssociation association)
        {
            return association.SpecializationTypeId == "7a3f0474-3cf8-4249-baac-8c07c49465e0" //Create Entity Action
                || association.SpecializationTypeId == "bfc823fb-60ab-451d-ba62-12671fe7e28e" //Delete Entity Action
                || association.SpecializationTypeId == "9ea0382a-4617-412a-a8c8-af987bbce226";//Update Entity Action
        }
    }
}
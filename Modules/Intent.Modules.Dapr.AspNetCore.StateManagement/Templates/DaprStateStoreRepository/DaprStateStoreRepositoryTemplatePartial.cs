using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Intent.Dapr.AspNetCore.StateManagement.Api;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DaprStateStoreRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DaprStateStoreRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("Dapr.Client")
                .AddClass($"{Model.Name}DaprStateStoreRepository", @class =>
                {
                    var pkAttribute = Model.Attributes.Single(x => x.HasPrimaryKey());
                    var pkPropertyName = pkAttribute.Name.ToPascalCase();
                    var pkTypeName = GetTypeName((IElement)pkAttribute.TypeReference.Element);
                    var pkToString = pkTypeName switch
                    {
                        "string" => string.Empty,
                        "int" or "long" => $".ToString({UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                        _ => ".ToString()"
                    };
                    var pkToStringPlural = pkToString != string.Empty
                        ? $".{UseType("System.Linq.Select")}(id => id{pkToString}).ToArray()"
                        : string.Empty;

                    var settings = Model.InternalElement.Package
                        .AsDomainPackageModel()
                        .GetDaprStateStoreSettings();
                    var stateStoreName = !string.IsNullOrWhiteSpace(settings?.Name())
                        ? settings.Name()
                        : "statestore";
                    var enableTransactions = settings?.EnableTransactions() == true
                        ? "true"
                        : "false";

                    @class
                        .ExtendsClass($"{this.GetDaprStateStoreRepositoryBaseName()}<{EntityInterfaceName}>")
                        .ImplementsInterface(this.GetEntityRepositoryInterfaceName())
                        .AddConstructor(constructor => constructor
                            .AddParameter("DaprClient", "daprClient")
                            .AddParameter(this.GetDaprStateStoreUnitOfWorkName(), "unitOfWork")
                            .CallsBase(@base => @base
                                .AddArgument("daprClient: daprClient")
                                .AddArgument("unitOfWork: unitOfWork")
                                .AddArgument($"enableTransactions: {enableTransactions}")
                                .AddArgument($"storeName: \"{stateStoreName}\"")
                            )
                        )
                        .AddMethod("void", "Add", method =>
                        {
                            method.AddParameter(EntityInterfaceName, "entity");

                            var shouldAutoSet = pkTypeName is "Guid" or "string";
                            if (shouldAutoSet)
                            {
                                method.AddIfStatement($"entity.{pkPropertyName} == default", s => s
                                    .AddStatement($"entity.{pkPropertyName} = Guid.NewGuid().ToString();")
                                );
                            }

                            method.AddStatement($"Upsert(entity.{pkPropertyName}{pkToString}, entity);", s =>
                            {
                                if (shouldAutoSet)
                                {
                                    s.SeparatedFromPrevious();
                                }
                            });
                        })
                        .AddMethod("void", "Update", method => method
                            .AddParameter(EntityInterfaceName, "entity")
                            .AddStatement($"Upsert(entity.{pkPropertyName}{pkToString}, entity);")
                        )
                        .AddMethod("void", "Remove", method => method
                            .AddParameter(EntityInterfaceName, "entity")
                            .AddStatement($"Remove(entity.{pkPropertyName}{pkToString}, entity);")
                        )
                        .AddMethod($"Task<{EntityInterfaceName}>", "FindByIdAsync", method => method
                            .AddParameter(pkTypeName, "id")
                            .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                            .AddStatement($"return FindByKeyAsync(id{pkToString}, cancellationToken);")
                        )
                        .AddMethod($"Task<List<{EntityInterfaceName}>>", "FindByIdsAsync", method => method
                            .AddParameter($"{pkTypeName}[]", "ids")
                            .AddParameter("CancellationToken", "cancellationToken", parameter => parameter.WithDefaultValue("default"))
                            .AddStatement($"return FindByKeysAsync(ids{pkToStringPlural}, cancellationToken);")
                        )
                    ;
                });
        }

        public string EntityInterfaceName => GetTypeName("Domain.Entity.Interface", Model);

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();

            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }

            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForConcern("Infrastructure")
                .ForInterface(contractTemplate)
                .WithPerServiceCallLifeTime()
            );
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
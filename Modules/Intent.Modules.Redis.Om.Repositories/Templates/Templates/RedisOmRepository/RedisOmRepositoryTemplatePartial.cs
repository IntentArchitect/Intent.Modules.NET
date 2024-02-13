using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmRepository
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmRepositoryTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmRepository";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmRepositoryTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            var createEntityInterfaces = ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces();
            string nullableChar = OutputTarget.GetProject().NullableEnabled ? "?" : "";

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}RedisOmRepository", @class =>
                {
                    var pkAttribute = Model.GetPrimaryKeyAttribute();
                    var pkFieldName = pkAttribute.IdAttribute.Name.ToCamelCase();
                    var genericTypeParameters = Model.GenericTypes.Any()
                        ? $"<{string.Join(", ", Model.GenericTypes)}>"
                        : string.Empty;
                    var entityDocumentName = $"{this.GetRedisOmDocumentName()}{genericTypeParameters}";
                    var entityDocumentInterfaceName = $"{this.GetRedisOmDocumentInterfaceName()}{genericTypeParameters}";

                    @class.Internal();
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    var entityStateGenericTypeArgument = createEntityInterfaces
                        ? $", {EntityStateTypeName}"
                        : string.Empty;
                    @class.ExtendsClass(
                        $"{this.GetRedisOmRepositoryBaseName()}<{EntityTypeName}{entityStateGenericTypeArgument}, {entityDocumentName}, {entityDocumentInterfaceName}>");
                    @class.ImplementsInterface($"{this.GetEntityRepositoryInterfaceName()}{genericTypeParameters}");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter(this.GetRedisOmUnitOfWorkName(), "unitOfWork");
                        ctor.AddParameter(UseType($"Redis.OM.RedisConnectionProvider"), "connectionProvider");
                        ctor.CallsBase(callBase => callBase
                            .AddArgument("unitOfWork")
                            .AddArgument("connectionProvider")
                        );
                    });

                    @class.AddMethod($"{UseType("System.Threading.Tasks.Task")}<{EntityTypeName}{nullableChar}>", "FindByIdAsync", method =>
                    {
                        method
                            .Async()
                            .AddParameter(GetPKType(pkAttribute), "id")
                            .AddOptionalCancellationTokenParameter(this)
                            .WithExpressionBody($"await base.FindByIdAsync({GetPKUsage(pkAttribute)}, cancellationToken: cancellationToken)");
                    });

                    if (pkAttribute.IdAttribute.TypeReference?.Element.Name != "string")
                    {
                        @class.AddMethod($"{UseType("System.Threading.Tasks.Task")}<{UseType("System.Collections.Generic.List")}<{EntityStateTypeName}>>", "FindByIdsAsync",
                            method =>
                            {
                                AddUsing("System.Linq");
                                method
                                    .Async()
                                    .AddParameter($"{GetTypeName(pkAttribute.IdAttribute)}[]", "ids")
                                    .AddOptionalCancellationTokenParameter(this)
                                    .WithExpressionBody($"await FindByIdsAsync(ids.Select(id => id{pkAttribute.IdAttribute.GetToString(this)}).ToArray(), cancellationToken)");
                            });
                    }

                    @class.AddMethod("string", "GetIdValue", method =>
                    {
                        method.Override().Protected();
                        method.AddParameter(EntityTypeName, "entity");
                        method.WithExpressionBody($"entity.{pkAttribute.IdAttribute.Name}{pkAttribute.IdAttribute.GetToString(this)}");
                    });

                    @class.AddMethod("void", "SetIdValue", method =>
                    {
                        method.Override().Protected();
                        method.AddParameter(EntityTypeName, "domainEntity");
                        method.AddParameter(entityDocumentName, "document");
                        method.WithExpressionBody($@"ReflectionHelper.ForceSetProperty(domainEntity, ""{pkAttribute.IdAttribute.Name}"", document.{pkAttribute.IdAttribute.Name})");
                    });
                });
        }

        internal string GetPKType(AttributeModelExtensionMethods.PrimaryKeyData pkAttribute)
        {
            return GetTypeName(pkAttribute.IdAttribute);
        }

        private string GetPKUsage(AttributeModelExtensionMethods.PrimaryKeyData pkAttribute)
        {
            return $"id: id{(pkAttribute.IdAttribute.TypeReference?.Element.Name != "string" ? pkAttribute.IdAttribute.GetToString(this) : "")}";
        }


        public string GenericTypeParameters => Model.GenericTypes.Any()
            ? $"<{string.Join(", ", Model.GenericTypes)}>"
            : string.Empty;

        public string EntityTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Interface, Model)}{GenericTypeParameters}";
        public string EntityStateTypeName => $"{GetTypeName(TemplateRoles.Domain.Entity.Primary, Model)}{GenericTypeParameters}";

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(EntityRepositoryInterfaceTemplate.TemplateId, Model);
            if (contractTemplate == null)
            {
                return;
            }

            ((ICSharpFileBuilderTemplate)contractTemplate).CSharpFile.Interfaces[0].AddMetadata("requires-explicit-update", true);

            if (!Model.GenericTypes.Any())
            {
                ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                    .ForConcern("Infrastructure")
                    .ForInterface(contractTemplate)
                    .WithPerServiceCallLifeTime()
                );
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
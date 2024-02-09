using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmDocumentInterface;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Redis.Om.Repositories.Templates.Templates.RedisOmRepositoryInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RedisOmRepositoryInterfaceTemplate : CSharpTemplateBase<IList<ClassModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Redis.Om.Repositories.Templates.RedisOmRepositoryInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RedisOmRepositoryInterfaceTemplate(IOutputTarget outputTarget, IList<ClassModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq.Expressions")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddInterface($"IRedisOmRepository", @interface =>
                {
                    var pagedListInterface = TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
                        ? name : GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility

                    @interface
                        .AddGenericParameter("TDomain", out var tDomain)
                        .AddGenericParameter("TDocumentInterface", out var toDocumentInterface);

                    @interface
                        .ImplementsInterfaces($"{this.GetRepositoryInterfaceName()}<{tDomain}>")
                        .AddProperty(this.GetRedisOmUnitOfWorkInterfaceName(), "UnitOfWork", property => property
                            .WithoutSetter()
                        )
                        .AddMethod($"Task<{tDomain}{GetNullablePostfix(true)}>", "FindAsync", method => method
                            .AddParameter($"Expression<Func<{toDocumentInterface}, bool>>", "filterExpression")
                            .AddOptionalCancellationTokenParameter(this))
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddOptionalCancellationTokenParameter(this)
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{toDocumentInterface}, bool>>", "filterExpression")
                            .AddOptionalCancellationTokenParameter(this)
                        )
                        .AddMethod($"Task<{pagedListInterface}<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"int", "pageNo")
                            .AddParameter($"int", "pageSize")
                            .AddOptionalCancellationTokenParameter(this)
                        )
                        .AddMethod($"Task<{pagedListInterface}<{tDomain}>>", "FindAllAsync", method => method
                            .AddParameter($"Expression<Func<{toDocumentInterface}, bool>>", "filterExpression")
                            .AddParameter($"int", "pageNo")
                            .AddParameter($"int", "pageSize")
                            .AddOptionalCancellationTokenParameter(this)
                        )
                        .AddMethod($"Task<List<{tDomain}>>", "FindByIdsAsync", method => method
                            .AddParameter($"IEnumerable<string>", "ids")
                            .AddOptionalCancellationTokenParameter(this)
                        );
                });
        }

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();

            foreach (var model in Model)
            {
                var template = GetTemplate<ICSharpFileBuilderTemplate>(EntityRepositoryInterfaceTemplate.TemplateId, model.Id);
                template.CSharpFile.AfterBuild(file =>
                {
                    var @interface = file.Interfaces.Single();
                    @interface.Interfaces.Clear();
                    var genericTypeParameters = model.GenericTypes.Any()
                        ? $"<{string.Join(", ", model.GenericTypes)}>"
                        : string.Empty;
                    var tDomainGenericArgument = template.GetTypeName(TemplateRoles.Domain.Entity.Interface, model);
                    var tDocumentInterfaceGenericArgument = template.GetTypeName(RedisOmDocumentInterfaceTemplate.TemplateId, model);
                    @interface.ImplementsInterfaces($"{this.GetRedisOmRepositoryInterfaceName()}<{tDomainGenericArgument}{genericTypeParameters}, {tDocumentInterfaceGenericArgument}{genericTypeParameters}>");

                    if (model.GetPrimaryKeyAttribute()?.IdAttribute.TypeReference?.Element.Name == "string")
                    {
                        var toRemove = @interface.Methods
                            .Where(x => x.Name == "FindByIdsAsync")
                            .ToArray();

                        foreach (var method in toRemove)
                        {
                            @interface.Methods.Remove(method);
                        }
                    }
                }, 1000);
            }
        }

        private string GetNullablePostfix(bool isNullable)
        {
            return isNullable && OutputTarget.GetProject().NullableEnabled ? "?" : "";
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
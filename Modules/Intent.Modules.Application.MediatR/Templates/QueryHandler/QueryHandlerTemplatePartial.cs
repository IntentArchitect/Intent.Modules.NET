using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public partial class QueryHandlerTemplate : CSharpTemplateBase<QueryModel, QueryHandlerDecorator>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.QueryHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryHandlerTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource("Application.Contract.Dto", "List<{0}>");

            CSharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), "")
                .AddClass($"{Model.Name}Handler", @class =>
                {
                    @class.WithBaseType(GetRequestHandlerInterface());
                    @class.AddAttribute("IntentManaged(Mode.Merge, Signature = Mode.Fully)");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute("IntentManaged(Mode.Ignore)");
                    });
                    @class.AddMethod($"Task<{GetTypeName(Model.TypeReference)}>", "Handle", method =>
                    {
                        method.Async();
                        method.AddAttribute($"IntentManaged(Mode.Fully, Body = Mode.{(HasImplementation() ? "Fully" : "Ignore")})");
                        method.AddParameter(GetQueryModelName(), "request");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatements(GetImplementation());
                    });
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        public override void BeforeTemplateExecution()
        {
            AddRequiredServices(CSharpFile.Classes.First());
        }

        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private void AddRequiredServices(CSharpClass @class)
        {
            var ctor = @class.Constructors.First();
            foreach (var requiredService in GetDecorators().SelectMany(x => x.GetRequiredServices()).Distinct())
            {
                @class.AddField(requiredService.Type, requiredService.FieldName, x => x.Private());
                ctor.AddParameter(requiredService.Type, requiredService.Name.ToParameterName())
                    .AddStatement($@"{requiredService.FieldName} = {requiredService.Name.ToParameterName()};");
            }
        }

        private string GetQueryModelName()
        {
            return GetTypeName(QueryModelsTemplate.TemplateId, Model);
        }

        private IEnumerable<string> GetImplementation()
        {
            var decoratorStatements = GetDecorators()
                .Select(s => s.GetImplementation())
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();
            if (!decoratorStatements.Any())
            {
                return new[] { $@"throw new NotImplementedException(""Your implementation here..."");" };
            }

            return decoratorStatements;
        }

        private bool HasImplementation()
        {
            return GetDecorators().Any(p => !string.IsNullOrWhiteSpace(p.GetImplementation()));
        }

        private string GetRequestHandlerInterface()
        {
            return Model.TypeReference.Element != null
                ? $"IRequestHandler<{GetQueryModelName()}, {GetTypeName(Model.TypeReference)}>"
                : $"IRequestHandler<{GetQueryModelName()}>";
        }
    }
}
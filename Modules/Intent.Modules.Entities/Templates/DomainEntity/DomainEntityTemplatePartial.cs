using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    partial class DomainEntityTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasDecorators<DomainEntityDecoratorBase>, ITemplatePostCreationHook, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntity";
        public const string Identifier = "Intent.Entities.DomainEntity";
        private readonly IList<DomainEntityDecoratorBase> _decorators = new List<DomainEntityDecoratorBase>();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEntityTemplate(IOutputTarget outputTarget, ClassModel model)
: base(TemplateId, outputTarget, model)
        {
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityInterfaceTemplate.Identifier, "IEnumerable<{0}>"));
            AddTypeSource(DomainEnumTemplate.TemplateId).WithCollectionFormat("ICollection<{0}>");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public void AddDecorator(DomainEntityDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityDecoratorBase> GetDecorators()
        {
            return _decorators;
        }

        public string Constructors(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

        public string GetParametersDefinition(OperationModel operation)
        {
            return string.Join(", ", operation.Parameters.Select(x => $"{GetTypeName(x.Type)} {x.Name.ToCamelCase()}"));
        }

        public string EmitOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "async Task" : "void";
            }
            return o.IsAsync() ? $"async Task<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.ReturnType);
        }

        public IEnumerable<string> DeclareUsings()
        {
            if (Model.Operations.Any(x => x.IsAsync()))
            {
                yield return "System.Threading.Tasks";
            }
        }
    }
}

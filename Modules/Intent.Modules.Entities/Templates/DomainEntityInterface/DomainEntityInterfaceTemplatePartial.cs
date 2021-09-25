using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntityInterface
{
    partial class DomainEntityInterfaceTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasDecorators<DomainEntityInterfaceDecoratorBase>, ITemplatePostCreationHook, IDeclareUsings
    {
        private readonly IMetadataManager _metadataManager;
        public const string Identifier = "Intent.Entities.DomainEntityInterface";
        public const string InterfaceContext = "Interface";

        private readonly IList<DomainEntityInterfaceDecoratorBase> _decorators = new List<DomainEntityInterfaceDecoratorBase>();

        public DomainEntityInterfaceTemplate(ClassModel model, IOutputTarget outputTarget, IMetadataManager metadataManager)
            : base(Identifier, outputTarget, model)
        {
            _metadataManager = metadataManager;
        }

        public override void OnCreated()
        {
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityStateTemplate.TemplateId, "ICollection<{0}>"));
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEnumTemplate.TemplateId, "ICollection<{0}>"));
            Types.AddTypeSource(CSharpTypeSource.Create(ExecutionContext, Identifier, "IEnumerable<{0}>"), InterfaceContext);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public void AddDecorator(DomainEntityInterfaceDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityInterfaceDecoratorBase> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        public string GetInterfaces(ClassModel @class)
        {
            var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
            if (Model.GetStereotypeProperty("Base Type", "Has Interface", false) && GetBaseTypeInterface() != null)
            {
                interfaces.Insert(0, GetBaseTypeInterface());
            }

            return string.Join(", ", interfaces);
        }

        private string GetBaseTypeInterface()
        {
            var typeId = Model.GetStereotypeProperty<string>("Base Type", "Type");
            if (typeId == null)
            {
                return null;
            }


            // GCB - There is definitely a better way to handle this now (V3.0)
            var type = _metadataManager.Domain(OutputTarget.Application).GetTypeDefinitionModels().FirstOrDefault(x => x.Id == typeId);
            if (type != null)
            {
                return $"I{type.Name}";
            }
            throw new Exception($"Could not find Base Type for class {Model.Name}");
        }

        public string InterfaceAnnotations(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.InterfaceAnnotations(@class));
        }

        public string BeforeProperties(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.BeforeProperties(@class));
        }

        public string PropertyBefore(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyBefore(attribute));
        }

        public string PropertyAnnotations(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        }

        public string PropertyBefore(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyBefore(associationEnd));
        }

        public string PropertyAnnotations(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        }

        public string AttributeAccessors(AttributeModel attribute)
        {
            return GetDecorators().Select(x => x.AttributeAccessors(attribute)).FirstOrDefault(x => x != null) ?? "get; set;";
        }

        public string AssociationAccessors(AssociationEndModel associationEnd)
        {
            return GetDecorators().Select(x => x.AssociationAccessors(associationEnd)).FirstOrDefault(x => x != null) ?? "get; set;";
        }

        public bool CanWriteDefaultAttribute(AttributeModel attribute)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAttribute(attribute));
        }

        public bool CanWriteDefaultAssociation(AssociationEndModel association)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAssociation(association));
        }

        public bool CanWriteDefaultOperation(OperationModel operation)
        {
            return GetDecorators().All(x => x.CanWriteDefaultOperation(operation));
        }

        public string GetParametersDefinition(OperationModel operation)
        {
            return operation.Parameters.Any()
                ? operation.Parameters.Select(x => NormalizeNamespace(Types.InContext(InterfaceContext).Get(x.TypeReference).Name) + " " + x.Name.ToCamelCase()).Aggregate((x, y) => x + ", " + y)
                : "";
        }

        public string EmitOperationReturnType(OperationModel o)
        {
            if (o.TypeReference.Element == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{NormalizeNamespace(Types.InContext(InterfaceContext).Get(o.TypeReference).Name)}>" : NormalizeNamespace(Types.InContext(InterfaceContext).Get(o.TypeReference).Name);
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

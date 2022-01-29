using System;
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

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    partial class DomainEntityStateTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasDecorators<DomainEntityStateDecoratorBase>, ITemplatePostCreationHook
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntityState";
        public const string InterfaceContext = "Interface";

        private readonly IList<DomainEntityStateDecoratorBase> _decorators = new List<DomainEntityStateDecoratorBase>();

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEntityStateTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateId, "ICollection<{0}>");
            AddTypeSource(DomainEnumTemplate.TemplateId, "ICollection<{0}>");
            Types.AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityInterfaceTemplate.Identifier, "IEnumerable<{0}>"), InterfaceContext);
        }

        public string EntityInterfaceName => Project.FindTemplateInstance<IClassProvider>(TemplateDependency.OnModel(DomainEntityInterfaceTemplate.Identifier, Model))?.ClassName
                                             ?? $"I{Model.Name}";


        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}",
                fileName: $"{Model.Name}State");
        }

        public void AddDecorator(DomainEntityStateDecoratorBase decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<DomainEntityStateDecoratorBase> GetDecorators()
        {
            return _decorators.OrderBy(x => x.Priority);
        }

        public string GetBaseTypes(ClassModel @class)
        {
            try
            {
                var baseTypes = new List<string>();
                baseTypes.Add(GetDecorators().Select(x => x.GetBaseClass(@class)).SingleOrDefault(x => x != null) ?? @class.ParentClass?.Name);
                baseTypes.AddRange(GetInterfaces(Model));
                return string.Join(", ", baseTypes.Where(x => !string.IsNullOrWhiteSpace(x)));
            }
            catch (InvalidOperationException)
            {
                throw new Exception($"Multiple decorators attempting to modify 'base class' on {@class.Name}");
            }
        }

        public IEnumerable<string> GetInterfaces(ClassModel @class)
        {
            var interfaces = new List<string>()
            {
                GetTypeName(DomainEntityInterfaceTemplate.Identifier, Model)
            };
            interfaces.AddRange(GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList());
            return interfaces;
        }

        public string ClassAnnotations(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.ClassAnnotations(@class));
        }

        public string Constructors(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

        public string BeforeProperties(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.BeforeProperties(@class));
        }
        public string AfterProperties(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.AfterProperties(@class));
        }

        public string PropertyBefore(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyBefore(attribute));
        }

        public string PropertyFieldAnnotations(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyFieldAnnotations(attribute));
        }

        public string PropertyAnnotations(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        }

        public string PropertySetterBefore(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterBefore(attribute));
        }

        public string PropertySetterAfter(AttributeModel attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterAfter(attribute));
        }

        public string AssociationBefore(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.AssociationBefore(associationEnd));
        }

        public string PropertyAnnotations(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        }

        public string PropertySetterBefore(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterBefore(associationEnd));
        }

        public string PropertySetterAfter(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterAfter(associationEnd));
        }

        public string AssociationAfter(AssociationEndModel associationEnd)
        {
            return GetDecorators().Aggregate(x => x.AssociationAfter(associationEnd));
        }

        public bool CanWriteDefaultAttribute(AttributeModel attribute)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAttribute(attribute));
        }

        public bool CanWriteDefaultAssociation(AssociationEndModel association)
        {
            return GetDecorators().All(x => x.CanWriteDefaultAssociation(association));
        }
    }
}

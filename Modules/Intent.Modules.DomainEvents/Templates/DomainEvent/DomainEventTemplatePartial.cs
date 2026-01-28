using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Events.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.DomainEvents.Templates.DomainEventBase;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.DomainEvents.Templates.DomainEvent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class DomainEventTemplate : CSharpTemplateBase<DomainEventModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.DomainEvents.DomainEvent";

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEventTemplate(IOutputTarget outputTarget, DomainEventModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(DomainEventTemplate.TemplateId);
            AddTypeSource("Domain.Entity");
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.ValueObject);
            AddTypeSource(TemplateRoles.Domain.DataContract);

            var csharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());
            csharpFile.AddClass($"{Model.Name}");
            csharpFile.OnBuild(file =>
            {
                file.AddUsing("System");
                file.AddUsing("System.Collections.Generic");

                var @class = file.Classes.First();
                
                var baseDomainEvent = Model.Generalizations().Select(x => x.Element?.AsDomainEventModel()).FirstOrDefault();
                if (baseDomainEvent is null)
                {
                    @class.WithBaseType(GetBaseClass());
                }
                else
                {
                    @class.WithBaseType(this.GetDomainEventName(baseDomainEvent));
                }

                var inheritedProperties = GetPropertiesHierarchally(Model).ToList();
                
                @class.TryAddXmlDocComments(Model.InternalElement);
                @class.AddConstructor(ctor =>
                {
                    foreach (var inheritedProperty in inheritedProperties)
                    {
                        ctor.AddParameter(base.GetTypeName(inheritedProperty.PropertyModel.TypeReference), inheritedProperty.PropertyModel.Name.ToParameterName(), arg =>
                        {
                            if (!inheritedProperty.IsInherited)
                            {
                                arg.IntroduceProperty(property =>
                                {
                                    property.TryAddXmlDocComments(inheritedProperty.PropertyModel.InternalElement);
                                    property.ReadOnly();
                                    property.RepresentsModel(inheritedProperty.PropertyModel);
                                });
                            }
                        });
                    }

                    if (inheritedProperties.Count != 0)
                    {
                        ctor.CallsBase(callBase =>
                        {
                            foreach (var inheritedProperty in inheritedProperties)
                            {
                                if (inheritedProperty.IsInherited)
                                {
                                    callBase.AddArgument(inheritedProperty.PropertyModel.Name.ToParameterName());
                                }
                            }
                        });
                    }
                });
            });
            CSharpFile = csharpFile;
        }

        private sealed record InheritedPropertyModel(PropertyModel PropertyModel, bool IsInherited);
        private static IEnumerable<InheritedPropertyModel> GetPropertiesHierarchally(DomainEventModel model, bool isInheritModel = false)
        {
            var inheritModel = model.DomainEventGeneralizationEnds().FirstOrDefault(x => x.IsTargetEnd())?.Element.AsDomainEventModel();
            if (inheritModel is not null)
            {
                return model.Properties.Select(x => new InheritedPropertyModel(x, isInheritModel)).Concat(GetPropertiesHierarchally(inheritModel, true));
            }
            return model.Properties.Select(x => new InheritedPropertyModel(x, isInheritModel));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        private string GetBaseClass()
        {
            return GetTypeName(DomainEventBaseTemplate.TemplateId);
        }
    }
}
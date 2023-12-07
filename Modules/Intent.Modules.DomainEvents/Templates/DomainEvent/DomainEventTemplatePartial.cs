using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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
            csharpFile.OnBuild((Action<CSharpFile>)(file =>
                {
                    file.AddUsing("System");
                    file.AddUsing("System.Collections.Generic");

                    var @class = file.Classes.First();
                    @class.WithBaseType(GetBaseClass());
                    @class.TryAddXmlDocComments(Model.InternalElement);
                    @class.AddConstructor(ctor =>
                    {
                        foreach (var propertyModel in Model.Properties)
                        {
                            ctor.AddParameter(base.GetTypeName(propertyModel.TypeReference), propertyModel.Name.ToParameterName(), arg =>
                            {
                                arg.IntroduceProperty(property =>
                                {
                                    property.TryAddXmlDocComments(propertyModel.InternalElement);
                                    property.ReadOnly();
                                    property.RepresentsModel(propertyModel);
                                });
                            });
                        }
                    });

                }));
            CSharpFile = csharpFile;
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
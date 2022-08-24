using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEnum;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

namespace Intent.Modules.Entities.Templates.DomainEntityState
{
    [IntentManaged(Mode.Ignore, Body = Mode.Merge)]
    partial class DomainEntityStateTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Entities.DomainEntityState";
        public const string InterfaceContext = "Interface";

        public CSharpFile Output { get; set; }

        [IntentManaged(Mode.Ignore, Signature = Mode.Fully)]
        public DomainEntityStateTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateId, "ICollection<{0}>");
            AddTypeSource(DomainEnumTemplate.TemplateId, "ICollection<{0}>");
            AddTypeSource("Domain.ValueObject", "ICollection<{0}>");
            Types.AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DomainEntityInterfaceTemplate.Identifier, "IEnumerable<{0}>"), InterfaceContext);
            Output = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass(Model.Name, @class =>
                {
                    if (ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
                    {
                        @class.Partial();
                    }
                    if (ExecutionContext.Settings.GetDomainSettings().CreateEntityInterfaces())
                    {
                        @class.ImplementsInterface(this.GetDomainEntityInterfaceName());
                    }
                    foreach (var attribute in Model.Attributes)
                    {
                        var property = @class.AddProperty(GetTypeName(attribute), attribute.Name.ToPascalCase());
                        if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                        {
                            property.PrivateSetter();
                        }
                    }

                    foreach (var associationEnd in Model.AssociatedClasses.Where(x => x.IsNavigable))
                    {
                        var property = @class.AddProperty(GetTypeName(associationEnd), associationEnd.Name.ToPascalCase())
                            .Virtual();
                        if (ExecutionContext.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
                        {
                            property.PrivateSetter();
                        }
                    }
                });
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            var config = Output.GetConfig();
            if (ExecutionContext.Settings.GetDomainSettings().SeparateStateFromBehaviour())
            {
                config.FileName = $"{Model.Name}State";
            }
            return config;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return Output.ToString();
        }
        
        //[IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        //protected override CSharpFileConfig DefineFileConfig()
        //{
        //    return new CSharpFileConfig(
        //        className: $"{Model.Name}",
        //        @namespace: $"{this.GetNamespace()}",
        //        relativeLocation: $"{this.GetFolderPath()}",
        //        fileName: $"{Model.Name}State");
        //}
    }
}

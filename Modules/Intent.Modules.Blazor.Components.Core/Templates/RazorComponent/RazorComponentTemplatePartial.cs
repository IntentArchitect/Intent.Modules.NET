using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.Core.Templates.RazorComponent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class RazorComponentTemplate : IntentTemplateBase<ComponentModel>
    {
        private readonly IComponentRendererResolver _componentResolver;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.Blazor.Components.Core.RazorComponentTemplate";

        [IntentManaged(Mode.Merge)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model, IComponentRendererResolver componentResolver) : base(TemplateId, outputTarget, model)
        {
            _componentResolver = componentResolver;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: $"{Model.Name}",
                fileExtension: "razor"
            );
        }

    }

    public interface IComponentRendererResolver
    {
        IComponentRenderer ResolveFor(IElement component);
    }

    public interface IComponentRenderer
    {
        IRazorFileNode Render();
    }

    public class ComponentRendererResolver : IComponentRendererResolver
    {
        private Dictionary<string, Func<IElement, IComponentRenderer>> _componentRenderers = new();

        public ComponentRendererResolver()
        {
            _componentRenderers[FormModel.SpecializationTypeId] = (component) => new FormComponentRenderer(component, this);
        }
        public IComponentRenderer ResolveFor(IElement component)
        {
            if (!_componentRenderers.ContainsKey(component.SpecializationTypeId))
            {
                return new EmptyElementRenderer(component, this);
            }   
            return _componentRenderers[component.SpecializationTypeId](component);
        }
    }

    public class FormComponentRenderer : IComponentRenderer
    {
        private readonly IElement _component;
        private readonly IComponentRendererResolver _componentResolver;

        public FormComponentRenderer(IElement component, IComponentRendererResolver componentResolver)
        {
            _component = component;
            _componentResolver = componentResolver;
        }

        public IRazorFileNode Render()
        {
            var htmlElement = new HtmlElement(_component.Name);
            foreach (var child in _component.ChildElements)
            {
                htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render());
            }
            return htmlElement;
        }
    }


    public class EmptyElementRenderer : IComponentRenderer
    {
        private readonly IElement _component;
        private readonly IComponentRendererResolver _componentResolver;

        public EmptyElementRenderer(IElement component, IComponentRendererResolver componentResolver)
        {
            _component = component;
            _componentResolver = componentResolver;
        }

        public IRazorFileNode Render()
        {
            var htmlElement = new HtmlElement(_component.Name);
            foreach (var child in _component.ChildElements)
            {
                htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render());
            }
            return htmlElement;
        }
    }
}
using Intent.Metadata.Models;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.RazorBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer
{
    internal class AutoCompleteComponentBuilder : IRazorComponentBuilder
    {
        private readonly IRazorComponentBuilderProvider _componentResolver;
        private readonly IRazorComponentTemplate _componentTemplate;
        private readonly BindingManager _bindingManager;

        public AutoCompleteComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
        {
            _componentResolver = componentResolver;
            _componentTemplate = template;
            _bindingManager = template.BindingManager;
        }

        public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
        {
            var model = new AutoCompleteModel(component);
            var htmlElement = new HtmlElement("MudAutocomplete", _componentTemplate.RazorFile);
            var valueMapping = _bindingManager.GetMappedEndFor(model);
            var valueBinding = _bindingManager.GetBinding(valueMapping, parentNode)?.ToString();
            
            var onSelectedMapping = _bindingManager.GetMappedEndFor(model, "f07859f5-d5dc-4883-bc6f-5767a4b550cf"); // On Selected
            var onSelectedBinding = _bindingManager.GetBinding(onSelectedMapping, parentNode);

            var searchFuncMapping = _bindingManager.GetMappedEndFor(model, "a0232a41-ecc0-4ce6-86f2-de05f408770e"); // Search Function
            var searchFuncBinding = _bindingManager.GetBinding(searchFuncMapping, parentNode);


            htmlElement.AddAttributeIfNotEmpty("@bind-Value", _bindingManager.GetBinding(valueMapping, parentNode)?.ToString());
            htmlElement.AddAttribute("Label", model.TryGetLabelAddon(out var label) ? label.Label() : model.Name);
            htmlElement.AddAttributeIfNotEmpty("@bind-Value:after", onSelectedBinding?.ToLambda());
            htmlElement.AddAttributeIfNotEmpty("SearchFunc", searchFuncBinding?.ToLambda());

            var mappedEnd = _bindingManager.GetMappedEndFor(model, "Options");
            if (mappedEnd == null)
            {
                return [htmlElement];
            }
            htmlElement.AddCodeBlock($"@foreach (var item in {_bindingManager.GetBinding(model, "Options")})", code =>
            {
                htmlElement.AddMappingReplacement(mappedEnd.SourceElement, "item");
                code.AddHtmlElement("MudSelectItem", selectItem =>
                {
                    var selectItemMapping = _bindingManager.GetMappedEndFor(model, "Value");
                    selectItem.AddAttribute("T", _componentTemplate.GetTypeName(valueMapping.SourceElement.TypeReference));
                    // TODO: Use bindings:
                    selectItem.AddAttribute("Value", _bindingManager.GetBinding(model, "Value", htmlElement)?.ToString())
                        .WithText(_bindingManager.GetBinding(model, "Text", htmlElement)?.ToString());
                });
            });
            parentNode.AddChildNode(htmlElement);
            if (parentNode.GetAllNodesInHierarchy().OfType<HtmlElement>().Any(x => x.Name == "MudForm"))
            {
                if (valueMapping != null)
                {
                    htmlElement.AddAttribute("For", $"@(() => {valueBinding})");
                }
            }
            return [htmlElement];
        }
    }
}

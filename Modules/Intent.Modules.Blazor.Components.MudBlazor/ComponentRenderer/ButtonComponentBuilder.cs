using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Blazor.Components.MudBlazor.ComponentRenderer;

public class ButtonComponentBuilder : IRazorComponentBuilder
{
    private readonly IRazorComponentBuilderProvider _componentResolver;
    private readonly IRazorComponentTemplate _componentTemplate;
    private readonly BindingManager _bindingManager;

    public ButtonComponentBuilder(IRazorComponentBuilderProvider componentResolver, IRazorComponentTemplate template)
    {
        _componentResolver = componentResolver;
        _componentTemplate = template;
        _bindingManager = template.BindingManager;
    }

    public void BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var button = new ButtonModel(component);

        var onClickMapping = _bindingManager.GetMappedEndFor(button, "On Click");
        var htmlElement = new HtmlElement("MudButton", _componentTemplate.RazorFile)
            .AddAttribute("Variant", "Variant.Filled")
            .AddAttribute("Class", "my-2 mr-2")
            .AddAttribute("Color", button.GetInteraction().Type().IsSubmit() ? "Color.Primary" : "Color.Default");

        foreach (var child in component.ChildElements)
        {
            _componentResolver.ResolveFor(child).BuildComponent(child, htmlElement);
        }

        htmlElement.WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name);
        if (onClickMapping != null)
        {
            htmlElement.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");

            _componentTemplate.RazorFile.AfterBuild(file =>
            {
                _componentTemplate.GetClass().TryGetReferenceForModel(onClickMapping.SourceElement.Id, out var reference);
                if (reference is CSharpClassMethod method)
                {
                    var form = component.GetParentPath().Reverse().FirstOrDefault(x => x.IsFormModel());
                    if (form != null && button.GetInteraction().Type().IsSubmit())
                    {
                        method.Async();
                        ((IHasCSharpStatements)method.FindStatement(x => x is CSharpTryBlock) ?? method)?.InsertStatements(0,
                            $$"""
                                  await {{form.Name.ToPrivateMemberName()}}!.Validate();
                                  if (!{{form.Name.ToPrivateMemberName()}}.IsValid)
                                  {
                                      return;
                                  }
                                  """.ConvertToStatements().ToList());
                    }

                    if (!method.IsAsync)
                    {
                        return;
                    }
                    htmlElement.WithText(null); // clear text
                    var processingFieldName = $"{method.Name}Processing".ToPrivateMemberName();
                    _componentTemplate.GetClass().AddField("bool", processingFieldName, f => f.WithAssignment("false"));
                    ((CSharpTryBlock)method.FindStatement(x => x is CSharpTryBlock))?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "true").WithSemicolon());
                    ((CSharpFinallyBlock)method.FindStatement(x => x is CSharpFinallyBlock))?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "false").WithSemicolon());

                    htmlElement.AddAttribute("Disabled", $"@{processingFieldName}");
                    htmlElement.AddCodeBlock($"@if ({processingFieldName})", code =>
                    {
                        code.AddHtmlElement("MudProgressCircular", spinner =>
                        {
                            spinner.AddAttribute("Class", "ms-n1");
                            spinner.AddAttribute("Size", "Size.Small");
                            spinner.AddAttribute("Indeterminate", "true");
                        });
                        code.AddHtmlElement("MudText", text =>
                        {
                            text.AddAttribute("Class", "ms-2");
                            text.WithText("Processing");
                        });
                    });
                    htmlElement.AddCodeBlock("else", code =>
                    {
                        code.AddHtmlElement("MudText", text => text.WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name));
                    });
                }


            });

            //if (onClickMapping.SourceElement.AsComponentOperationModel()?.CallServiceOperationActionTargets().Any() == true)
            //{
            //    htmlElement.AddAttribute("Color", "Color.Primary");
            //}
            //if (onClickMapping?.SourceElement?.IsNavigationTargetEndModel() == true)
            //{
            //    var route = onClickMapping.SourceElement.AsNavigationTargetEndModel().Element.AsComponentModel().GetPage()?.Route();
            //    htmlElement.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");
            //}
            //else
            //{
            //    htmlElement.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");
            //}
        }

        //foreach (var child in component.ChildElements)
        //{
        //    htmlElement.Nodes.Add(_componentResolver.ResolveFor(child).Render(child));
        //}
        parentNode.AddChildNode(htmlElement);
    }
}
using System.Collections.Generic;
using System.Linq;
using Intent.Blazor.Components.MudBlazor.Api;
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

    public IEnumerable<IRazorFileNode> BuildComponent(IElement component, IRazorFileNode parentNode)
    {
        var button = new ButtonModel(component);
        var onClickMapping = _bindingManager.GetMappedEndFor(button, "On Click");
        var htmlElement = new HtmlElement("MudButton", _componentTemplate.RazorFile)
            .AddAttribute("Variant", "Variant.Filled")
            .AddAttribute("Class", "my-2 mr-2");

        htmlElement.AddAttribute("Color", _bindingManager.GetBinding(button, "3a04c387-3b5b-4a3d-b03a-4ecd0dcc301a", parentNode)?.ToString()
            ?? "Color." + (button.GetAppearance()?.Color().Name ?? (button.GetInteraction().Type().IsSubmit() ? "Primary" : "Default")));

        foreach (var child in component.ChildElements)
        {
            _componentResolver.BuildComponent(child, htmlElement);
        }

        htmlElement.WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name);
        if (onClickMapping != null)
        {
            htmlElement.AddAttribute("OnClick", $"{_bindingManager.GetBinding(onClickMapping, parentNode).ToLambda()}");

            _componentTemplate.RazorFile.AfterBuild(file =>
            {
                _componentTemplate.GetCodeBehind().TryGetReferenceForModel(onClickMapping.SourceElement.Id, out var reference);
                if (reference is CSharpClassMethod method)
                {
                    var form = button.GetInteraction().Form() ?? component.GetParentPath().Reverse().FirstOrDefault(x => x.IsFormModel());
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
                    _componentTemplate.GetCodeBehind().AddField("bool", processingFieldName, f => f.WithAssignment("false"));
                    ((CSharpTryBlock)method.FindStatement(x => x is CSharpTryBlock))?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "true").WithSemicolon());
                    var finallyBlock = ((CSharpFinallyBlock)method.FindStatement(x => x is CSharpFinallyBlock));
                    if (finallyBlock is null)
                    {
                        finallyBlock = new CSharpFinallyBlock();
                        ((CSharpCatchBlock)method.FindStatement(x => x is CSharpCatchBlock))?.InsertBelow(finallyBlock);
                    }

                    finallyBlock?.InsertStatement(0, new CSharpAssignmentStatement(processingFieldName, "false").WithSemicolon());

                    htmlElement.AddAttribute("Disabled", $"@{processingFieldName}");
                    htmlElement.AddCodeBlock($"@if ({processingFieldName})", code =>
                    {
                        code.AddHtmlElement("MudProgressCircular", spinner =>
                        {
                            spinner.AddAttribute("Class", "ms-n1");
                            spinner.AddAttribute("Size", "Size.Small");
                            spinner.AddAttribute("Indeterminate", "true");
                        });
                    });
                    htmlElement.AddHtmlElement("MudText", text => text.WithText(!string.IsNullOrWhiteSpace(button.InternalElement.Value) ? button.InternalElement.Value : button.Name));
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
        return [htmlElement];
    }
}
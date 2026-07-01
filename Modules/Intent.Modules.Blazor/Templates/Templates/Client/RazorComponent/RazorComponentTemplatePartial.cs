using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition;
using Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentCodeBehind;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using ComponentModel = Intent.Modelers.UI.Api.ComponentModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.RazorTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorComponent
{
    using Intent.Blazor.Api;
    using Intent.Templates;

    /// <summary>
    /// A Razor template.
    /// </summary>
    [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Ignore, Comments = Mode.Fully)]
    public partial class RazorComponentTemplate : RazorComponentTemplateBase<ComponentModel>
    {
        public const string SecuredStereotypeId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";
        /// <inheritdoc cref="IntentTemplateBase.Id"/>
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Blazor.Templates.Client.RazorComponentTemplate";

        /// <summary>
        /// Creates a new instance of <see cref="RazorComponentTemplate"/>.
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public RazorComponentTemplate(IOutputTarget outputTarget, ComponentModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(ModelDefinitionTemplate.TemplateId);
            AddTypeSource("Blazor.HttpClient.Contracts.Dto");
            AddTypeSource("Blazor.HttpClient.ServiceContract");
            AddTypeSource("Intent.Application.Dtos.DtoModel");
            AddTypeSource(TemplateId);

            RazorFile = IRazorFile.Create(this, $"{Model.Name.ToSanitized()}")
                .Configure(file =>
                {
                    if (Model.HasPage())
                    {
                        var route = Model.GetPage().Route();
                        file.AddPageDirective(route.StartsWith('/') ? route : "/" + route);
                        if (!string.IsNullOrWhiteSpace(Model.GetPage().Title()))
                        {
                            file.AddHtmlElement("PageTitle", x => x.WithText(Model.GetPage().Title()));
                        }
                    }

                    if (Model.HasSecured())
                    {
                        foreach (var secure in Model.GetSecureds())
                        {
                            file.AddAttributeDirective(secure.AuthorizationAttribute(this));
                        }
                    }

                    var block = GetCodeBehind();
                    block.AddCodeBlockMembers(this, Model.InternalElement);

                    file.AfterBuild(_ =>
                    {
                        if (Model.View is not null)
                        {
                            ComponentBuilderProvider.BuildComponent(Model.View.InternalElement, file);
                        }
                    });


                    if (Model.HasPage())
                    {
                        foreach (var declaration in block.Declarations)
                        {
                            if (declaration is CSharpProperty property && new RouteManager(Model.GetPage().Route()).HasParameterExpression(property.Name)
                                && property.Attributes.All(x => x.Name != "Parameter" && !x.Name.EndsWith(".Parameter")))
                            {
                                property.AddAttribute(block.Template.UseType("Microsoft.AspNetCore.Components.Parameter"));
                            }
                        }
                    }
                });
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public sealed override IRazorFile RazorFile { get; }

        protected override string CodeBehindTemplateId => RazorComponentCodeBehindTemplate.TemplateId;

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        protected override RazorFileConfig DefineRazorConfig()
        {
            var config = RazorFile.GetConfig();

            return new RazorFileConfig(Modules.Common.CodeGenType.UserControlledWeave, config.ClassName, config.Namespace,
                config.LocationInProject, OverwriteBehaviour.Always)
                .WithAIContext(GetIntentionContext());
        }

        /// <inheritdoc />
        [IntentManaged(Mode.Ignore)]
        public override string TransformText()
        {
            var filePath = GetMetadata().GetFilePath();

            string baseContent;
            if (!System.IO.File.Exists(filePath))
            {
                var razorContent = RazorFile.ToString();
                if (string.IsNullOrWhiteSpace(razorContent))
                    return "@* To be replaced with your razor content *@";
                baseContent = razorContent;
            }
            else
            {
                baseContent = System.IO.File.ReadAllText(filePath);
            }

            var pageDirective = BuildPageDirective();
            var attributeDirectives = BuildAttributeDirectives();
            var pageTitle = BuildPageTitle();

            if (string.IsNullOrEmpty(pageDirective) && string.IsNullOrEmpty(pageTitle))
                return baseContent;

            baseContent = NormalizeLineEndings(baseContent);
            var stripped = RemoveManagedDirectives(baseContent);
            var (userDirectives, contentBody) = SplitLeadingDirectives(stripped);
            contentBody = contentBody.TrimStart('\r', '\n');

            var sb = new System.Text.StringBuilder();
            sb.Append(pageDirective);
            sb.Append(userDirectives);
            sb.Append(attributeDirectives);
            if (!string.IsNullOrEmpty(pageTitle))
            {
                sb.Append('\n').Append(pageTitle).Append('\n');
                if (!string.IsNullOrEmpty(contentBody))
                    sb.Append('\n');
            }
            sb.Append(contentBody);

            return sb.ToString();
        }

        private static string NormalizeLineEndings(string content)
        {
            return content.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        private string BuildPageDirective()
        {
            if (!Model.HasPage()) return string.Empty;
            var route = Model.GetPage().Route();
            var fullRoute = route.StartsWith('/') ? route : "/" + route;
            return $"@page \"{fullRoute}\"\n";
        }

        private string BuildAttributeDirectives()
        {
            if (!Model.HasSecured()) return string.Empty;
            var sb = new System.Text.StringBuilder();
            foreach (var secure in Model.GetSecureds())
                sb.Append("@attribute ").Append(secure.AuthorizationAttribute(this)).Append('\n');
            return sb.ToString();
        }

        private string BuildPageTitle()
        {
            if (!Model.HasPage()) return string.Empty;
            var title = Model.GetPage().Title();
            return string.IsNullOrWhiteSpace(title) ? string.Empty : $"<PageTitle>{title}</PageTitle>";
        }

        private static (string directives, string body) SplitLeadingDirectives(string content)
        {
            var lines = content.Split('\n');
            var directiveLines = new List<string>();
            var i = 0;
            while (i < lines.Length)
            {
                var t = lines[i].Trim();
                if (t.StartsWith('@'))
                    directiveLines.Add(lines[i]);
                else if (t.Length != 0)
                    break;
                i++;
            }
            var directives = directiveLines.Count > 0 ? string.Join('\n', directiveLines) + '\n' : string.Empty;
            var body = i < lines.Length ? string.Join('\n', lines.Skip(i)) : string.Empty;
            return (directives, body);
        }

        private static string RemoveManagedDirectives(string content)
        {
            content = PageTitleRegex().Replace(content, string.Empty);

            var lines = content.Split('\n');
            var kept = lines.Where(line =>
            {
                var t = line.Trim();
                return !t.StartsWith("@page ") &&
                       !(t.StartsWith("@attribute [") && t.Contains("Authorize"));
            });
            return string.Join('\n', kept);
        }

        private string GetIntentionContext()
        {
            var intention = new StringBuilder();
            AddLayoutNavigationContext(intention);
            AddNavigatesToContext(intention);
            AddShowDialogContext(intention);
            AddCallServiceOperationContext(intention);
            AddCompositionContext(intention);
            return intention.ToString();
        }

        private void AddLayoutNavigationContext(StringBuilder intention)
        {
            foreach (var associationEnd in Model.InternalElement.AssociatedElements
                .Where(a => a.IsNavigationSourceEndModel() && !a.IsNavigable))
            {
                intention.AppendLine($"- This page is navigated to from a {associationEnd.TypeReference.Element.Name} menu item");
            }
        }

        private void AddNavigatesToContext(StringBuilder intention)
        {
            foreach (var navigation in Model.InternalElement.AssociatedElements.Where(e => e.IsNavigationEndModel() && e.IsNavigable))
            {
                var navEndModel = navigation.AsNavigationEndModel();
                intention.AppendLine($"- This pages navigates to the {navEndModel.TypeReference.Element.Name} component");
            }
        }

        private void AddShowDialogContext(StringBuilder intention)
        {
            foreach (var operation in Model.Operations.Where(o => o.InternalElement.AssociatedElements.Any(e => e.IsShowDialogTargetEndModel())))
            {
                foreach (var association in operation.InternalElement.AssociatedElements.Where(e => e.IsShowDialogTargetEndModel()))
                {
                    var dialogTargetEnd = association.AsShowDialogTargetEndModel();
                    intention.AppendLine($"- The {operation.Name} operation opens a dialog to show the {dialogTargetEnd.TypeReference.Element.Name} component");
                }
            }

            foreach (var association in Model.InternalElement.AssociatedElements.Where(e => e.IsShowDialogTargetEndModel()))
            {
                var dialogTargetEnd = association.AsShowDialogTargetEndModel();
                intention.AppendLine($"- {Model.Name} opens a dialog to show the {dialogTargetEnd.TypeReference.Element.Name} component");
            }
        }

        private void AddCallServiceOperationContext(StringBuilder intention)
        {
            foreach (var serviceCall in Model.InternalElement.AssociatedElements.Where(o => o.IsCallServiceOperationActionEndModel()))
            {
                var serviceCallEnd = serviceCall.AsCallServiceOperationActionEndModel();
                intention.AppendLine($"- The {Model.Name} page calls the {serviceCallEnd.TypeReference.Element.Name} service");
            }
        }

        private void AddCompositionContext(StringBuilder intention)
        {
            foreach (var association in Model.InternalElement.AssociatedElements.Where(e => e.IsCompositionTargetEndModel()))
            {
                var compositionTargetEnd = association.AsCompositionTargetEndModel();
                intention.AppendLine($"- {Model.Name} is composed of the {compositionTargetEnd.TypeReference.Element.Name} component.");
            }
        }

        [GeneratedRegex(@"<PageTitle\b[^>]*>.*?</PageTitle>", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-ZA")]
        private static partial Regex PageTitleRegex();
    }
}
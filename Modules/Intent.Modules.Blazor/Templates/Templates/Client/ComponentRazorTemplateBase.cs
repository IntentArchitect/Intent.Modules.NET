using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modelers.UI.Core.Api;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Blazor.Templates;
using Intent.Modules.Blazor.Templates.Templates.Client.ModelDefinition;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Blazor.Templates.Templates.Client
{
    using Intent.Blazor.Api;

    /// <summary>
    /// Shared logic for every Client Razor component variant (plain Component, Page, Dialog): page
    /// directive/title, secured "@attribute" directives, edit-preserving <see cref="TransformText"/>
    /// merging, the auth-mode output file name suffix, and the AI intention-context builder.
    /// </summary>
    public abstract partial class ComponentRazorTemplateBase : RazorComponentTemplateBase<ComponentModel>, IAuthPageRazorTemplate
    {
        public const string SecuredStereotypeId = "012f5173-6419-4006-a9a8-ab5c20b8a42e";

        // Element metadata key stamped by the "Security Type" stereotype's page-tagging script onto
        // the modelled Login/Register/etc. Component/Page elements it creates (e.g. "jwt-login").
        private const string AuthPageIdMetadataKey = "blazor-auth-page-id";

        // Namespaces required by directives this template emits (e.g. "@attribute [Authorize]"),
        // reconciled against the existing file's usings when merge-regenerating in TransformText().
        private readonly HashSet<string> _requiredUsings = new(StringComparer.Ordinal);

        protected ComponentRazorTemplateBase(string templateId, IOutputTarget outputTarget, ComponentModel model) : base(templateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(ModelDefinitionTemplate.TemplateId);
            AddTypeSource("Blazor.HttpClient.Contracts.Dto");
            AddTypeSource("Blazor.HttpClient.ServiceContract");
            AddTypeSource("Intent.Application.Dtos.DtoModel");
            AddTypeSource(templateId);

            BuiltRazorFile = IRazorFile.Create(this, GetOutputFileName(model))
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

        /// <summary>
        /// The built Razor file each leaf's own Mode.Ignore-tagged <c>RazorFile</c> property returns,
        /// kept on the base so leaves stay thin while still owning the interface member Module Builder's
        /// Razor Template scaffold expects.
        /// </summary>
        protected IRazorFile BuiltRazorFile { get; }

        /// <summary>
        /// When set, seeds this page's first-generation Razor content with this literal string instead
        /// of the built Razor file's markup.
        /// </summary>
        public string? DefaultContentOverride { get; set; }

        /// <summary>
        /// Computes this component's physical output file name (without extension), appending an
        /// authentication-mode suffix (e.g. "LoginJwt") when the model is an authentication-generated
        /// page, so pages sharing the same modelled route across auth modes still generate to distinct
        /// files. The <c>@page</c> route itself is unaffected.
        /// </summary>
        public static string GetOutputFileName(ComponentModel model)
        {
            var baseName = model.Name.ToSanitized();
            if (!model.InternalElement.Metadata.TryGetValue(AuthPageIdMetadataKey, out var pageId))
            {
                return baseName;
            }

            var mode = pageId.Split('-', 2)[0];
            var suffix = mode switch
            {
                "identity" => "Identity",
                "jwt" => "Jwt",
                "oidc" => "Oidc",
                _ => string.Empty
            };

            return $"{baseName}{suffix}";
        }

        protected abstract override string CodeBehindTemplateId { get; }

        /// <summary>
        /// Body for each leaf's own Mode.Ignore-tagged <c>DefineRazorConfig()</c> override.
        /// </summary>
        protected RazorFileConfig DefineRazorConfigCore()
        {
            var config = BuiltRazorFile.GetConfig();

            return new RazorFileConfig(Modules.Common.CodeGenType.UserControlledWeave, config.ClassName, config.Namespace,
                config.LocationInProject, OverwriteBehaviour.Always)
                .WithAIContext(GetIntentionContext());
        }

        /// <summary>
        /// Body for each leaf's own Mode.Ignore-tagged <c>TransformText()</c> override.
        /// </summary>
        protected string TransformTextCore()
        {
            var filePath = GetMetadata().GetFilePath();

            string baseContent;
            if (!System.IO.File.Exists(filePath))
            {
                var razorContent = DefaultContentOverride ?? BuiltRazorFile.ToString();
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

            if (string.IsNullOrEmpty(pageDirective) && string.IsNullOrEmpty(pageTitle) && string.IsNullOrEmpty(attributeDirectives))
                return baseContent;

            baseContent = NormalizeLineEndings(baseContent);
            var stripped = RemoveManagedDirectives(baseContent);
            var (userDirectives, contentBody) = SplitLeadingDirectives(stripped);
            contentBody = contentBody.TrimStart('\r', '\n');
            userDirectives = AddMissingUsings(userDirectives);

            var sb = new System.Text.StringBuilder();
            sb.Append(pageDirective);
            sb.Append(userDirectives);
            sb.Append(attributeDirectives);
            if (!string.IsNullOrEmpty(pageTitle))
                sb.Append('\n').Append(pageTitle).Append('\n');
            if (!string.IsNullOrEmpty(contentBody))
                sb.Append('\n').Append(contentBody);

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
            {
                _requiredUsings.Add("Microsoft.AspNetCore.Authorization");
                sb.Append("@attribute ").Append(secure.AuthorizationAttribute(this)).Append('\n');
            }
            return sb.ToString();
        }

        private string BuildPageTitle()
        {
            if (!Model.HasPage()) return string.Empty;
            var title = Model.GetPage().Title();
            return string.IsNullOrWhiteSpace(title) ? string.Empty : $"<PageTitle>{title}</PageTitle>";
        }

        /// <summary>
        /// Prepends any namespace registered in <see cref="_requiredUsings"/> (e.g. by
        /// <see cref="BuildAttributeDirectives"/> for "@attribute [Authorize]") that isn't already
        /// covered by an "@using" line in <paramref name="userDirectives"/>.
        /// </summary>
        private string AddMissingUsings(string userDirectives)
        {
            if (_requiredUsings.Count == 0) return userDirectives;

            var existingUsings = UsingNamespaceRegex().Matches(userDirectives)
                .Select(m => m.Groups[1].Value)
                .ToHashSet(StringComparer.Ordinal);

            var missingUsings = _requiredUsings.Where(ns => !existingUsings.Contains(ns)).ToList();
            if (missingUsings.Count == 0) return userDirectives;

            var sb = new System.Text.StringBuilder();
            foreach (var ns in missingUsings)
                sb.Append("@using ").Append(ns).Append('\n');
            sb.Append(userDirectives);
            return sb.ToString();
        }

        private static (string directives, string body) SplitLeadingDirectives(string content)
        {
            var lines = content.Split('\n');
            var directiveLines = new List<string>();
            var i = 0;
            while (i < lines.Length)
            {
                var t = lines[i].Trim();
                if (RazorDirectiveRegex().IsMatch(t))
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

        [GeneratedRegex(@"^@(page|using|attribute|inject|implements|typeparam|layout|namespace|inherits|preservewhitespace|rendermode)\b")]
        private static partial Regex RazorDirectiveRegex();

        [GeneratedRegex(@"^@using\s+(\S+)", RegexOptions.Multiline)]
        private static partial Regex UsingNamespaceRegex();
    }
}

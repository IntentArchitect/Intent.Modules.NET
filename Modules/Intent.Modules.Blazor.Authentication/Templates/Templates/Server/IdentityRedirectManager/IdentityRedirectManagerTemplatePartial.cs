using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication.Templates.Templates.Server.IdentityRedirectManager
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class IdentityRedirectManagerTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.Authentication.Templates.Server.IdentityRedirectManagerTemplate";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public IdentityRedirectManagerTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("Microsoft.AspNetCore.Identity")
                .AddUsing("Microsoft.AspNetCore.Components")
                .AddUsing("Microsoft.AspNetCore.Http")
                .AddUsing("System")
                .AddUsing("System.Diagnostics.CodeAnalysis")
                .AddUsing("System.Collections.Generic")
                .AddClass($"IdentityRedirectManager", @class =>
                {
                    @class.Sealed().Internal();

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("NavigationManager", "navigationManager", navigationManager =>
                        {
                            navigationManager.IntroduceReadonlyField();
                        });
                    });

                    @class.AddField("string", "StatusCookieName", statusCookieName =>
                    {
                        statusCookieName.Constant("\"Identity.StatusMessage\"");
                    });

                    @class.AddField("CookieBuilder", "StatusCookieBuilder", statusCookieBuilder =>
                    {
                        statusCookieBuilder.Private().Static().PrivateReadOnly();
                        statusCookieBuilder.WithAssignment(new CSharpStatement("new() { SameSite = SameSiteMode.Strict, HttpOnly = true, IsEssential = true, MaxAge = TimeSpan.FromSeconds(5) }"));
                    });

                    @class.AddMethod("void", "RedirectTo", redirectTo =>
                    {
                        redirectTo.AddParameter("string?", "uri");
                        redirectTo.AddAttribute("DoesNotReturn");

                        redirectTo.AddStatement("uri ??= \"\";");

                        redirectTo.AddIfStatement("!Uri.IsWellFormedUriString(uri, UriKind.Relative)", @if =>
                        {
                            @if.AddStatement("uri = _navigationManager.ToBaseRelativePath(uri);");
                        });

                        redirectTo.AddStatement("_navigationManager.NavigateTo(uri);");
                        redirectTo.AddStatement("throw new InvalidOperationException($\"{nameof(IdentityRedirectManager)} can only be used during static rendering.\");");
                    });

                    @class.AddMethod("void", "RedirectTo", redirectTo =>
                    {
                        redirectTo.AddParameter("string?", "uri");
                        redirectTo.AddParameter("Dictionary<string, object?>", "queryParameters");
                        redirectTo.AddAttribute("DoesNotReturn");

                        redirectTo.AddStatement("var uriWithoutQuery = _navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);");
                        redirectTo.AddStatement("var newUri = _navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);");
                        redirectTo.AddStatement("RedirectTo(newUri);");
                    });

                    @class.AddMethod("void", "RedirectToWithStatus", redirectTo =>
                    {
                        redirectTo.AddParameter("string?", "uri");
                        redirectTo.AddParameter("string", "message");
                        redirectTo.AddParameter("HttpContext", "context");
                        redirectTo.AddAttribute("DoesNotReturn");

                        redirectTo.AddStatement("context.Response.Cookies.Append(StatusCookieName, message, StatusCookieBuilder.Build(context));");
                        redirectTo.AddStatement("RedirectTo(uri);");
                    });

                    @class.AddMethod("void", "RedirectToCurrentPage", redirectTo =>
                    {
                        redirectTo.AddAttribute("DoesNotReturn");
                        redirectTo.AddStatement("RedirectTo(_navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path));");
                    });

                    @class.AddMethod("void", "RedirectToCurrentPageWithStatus", redirectTo =>
                    {
                        redirectTo.AddParameter("string", "message");
                        redirectTo.AddParameter("HttpContext", "context");
                        redirectTo.AddAttribute("DoesNotReturn");
                        redirectTo.AddStatement("RedirectToWithStatus(_navigationManager.ToAbsoluteUri(_navigationManager.Uri).GetLeftPart(UriPartial.Path), message, context);");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
using System.Linq;
using Intent.Engine;
using Intent.Modules.Application.Identity.Templates;
using Intent.Modules.Application.Identity.Templates.CurrentUserInterface;
using Intent.Modules.Application.Identity.Templates.CurrentUserService;
using Intent.Modules.Application.Identity.Templates.CurrentUserServiceInterface;
using Intent.Modules.AzureFunctions.Jwt.Templates.CurrentUser;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Jwt.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CurrentUserServiceExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.Jwt.CurrentUserServiceExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // Customize ICurrentUser interface to add AccessToken property
            var currentUserInterface = application.FindTemplateInstance<CurrentUserInterfaceTemplate>(
                TemplateDependency.OnTemplate(CurrentUserInterfaceTemplate.TemplateId));
            
            if (currentUserInterface != null)
            {
                currentUserInterface.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    @interface.AddProperty("string?", "AccessToken", prop => prop.ReadOnly());
                }, 1);
            }

            // Customize CurrentUserService to implement the required behavior
            var currentUserService = application.FindTemplateInstance<CurrentUserServiceTemplate>(
                TemplateDependency.OnTemplate(CurrentUserServiceTemplate.TemplateId));
            
            if (currentUserService != null)
            {
                currentUserService.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("Microsoft.AspNetCore.Http");

                    var @class = file.Classes.First();
                    
                    // Remove the default constructor and add IHttpContextAccessor dependency
                    var defaultCtor = @class.Constructors.FirstOrDefault();
                    if (defaultCtor != null)
                    {
                        @class.Constructors.Remove(defaultCtor);
                    }
                    
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("IHttpContextAccessor", "httpContextAccessor", param =>
                        {
                            param.IntroduceReadonlyField();
                        });
                    });

                    // Update GetAsync method
                    var getAsyncMethod = @class.FindMethod("GetAsync");
                    if (getAsyncMethod != null)
                    {
                        getAsyncMethod.Statements.Clear();
                        getAsyncMethod.AddStatements(@"
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return Task.FromResult<ICurrentUser?>(null);
            }

            var principal = httpContext.User;
            if (principal?.Identity?.IsAuthenticated == true)
            {
                var rawToken = ResolveToken(httpContext);
                return Task.FromResult<ICurrentUser?>(new CurrentUser(principal, rawToken));
            }

            return Task.FromResult<ICurrentUser?>(null);");
                    }

                    // Update AuthorizeAsync method
                    var authorizeAsyncMethod = @class.FindMethod("AuthorizeAsync");
                    if (authorizeAsyncMethod != null)
                    {
                        authorizeAsyncMethod.Statements.Clear();
                        authorizeAsyncMethod.AddStatements(@"
            var currentUser = await GetAsync();
            if (currentUser == null)
                return false;

            // Basic authorization logic - can be extended based on your policy requirements
            // For now, any authenticated user is authorized
            return true;");
                    }

                    // Update IsInRoleAsync method
                    var isInRoleAsyncMethod = @class.FindMethod("IsInRoleAsync");
                    if (isInRoleAsyncMethod != null)
                    {
                        isInRoleAsyncMethod.Statements.Clear();
                        isInRoleAsyncMethod.AddStatements(@"
            var currentUser = await GetAsync();
            if (currentUser == null)
                return false;

            return currentUser.Principal.IsInRole(role);");
                    }

                    // Add ResolveToken private method
                    @class.AddMethod("string?", "ResolveToken", method =>
                    {
                        method.Private().Static();
                        method.AddParameter("HttpContext", "httpContext");
                        method.AddStatements(@"
            if (httpContext.Items.TryGetValue(""RawToken"", out var tokenObject) && tokenObject is string rawToken)
            {
                return rawToken;
            }

            return null;");
                    });
                }, 1);
            }
        }
    }
}
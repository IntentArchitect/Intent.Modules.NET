using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// What the User Interface model states about ASP.NET Core MVC views, read by <c>intent.mvc</c> from the views the C# language
/// reads and the controllers that render them: a view's kind, a page's URLs, its regions, the partials it renders, its links and
/// the actions its forms post to. The fixture adapts eShop's Identity.API views and controllers.
/// </summary>
public class MvcViewsTests
{
    private static readonly Dictionary<string, string> Code = new()
    {
        ["src/Web/Web.csproj"] = """<Project Sdk="Microsoft.NET.Sdk.Web"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>""",
        ["src/Web/Program.cs"] = """
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            var app = builder.Build();
            app.MapDefaultControllerRoute();
            app.Run();
            """,
        ["src/Web/Controllers/AccountController.cs"] = """
            using Microsoft.AspNetCore.Mvc;
            namespace Web.Controllers;

            public class AccountController : Controller
            {
                [HttpGet]
                public IActionResult Login(string returnUrl) => View(new LoginViewModel());

                [HttpPost]
                public IActionResult Login(LoginInputModel model, string button)
                {
                    if (button == "cancel")
                    {
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return View(new LoginViewModel());
                }

                [HttpGet]
                public async Task<IActionResult> Logout(string logoutId) => await Logout(new LogoutInputModel());

                [HttpPost]
                public async Task<IActionResult> Logout(LogoutInputModel model) => View("LoggedOut", new LoggedOutViewModel());
            }

            public static class ControllerExtensions
            {
                public static IActionResult LoadingPage(this Controller controller, string viewName, string redirectUri)
                {
                    return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
                }
            }

            public class LoginViewModel { }
            public class LoginInputModel { public string ReturnUrl { get; set; } }
            public class LogoutInputModel { }
            public class LoggedOutViewModel { }
            public class RedirectViewModel { public string RedirectUrl { get; set; } }
            """,
        ["src/Web/Controllers/HomeController.cs"] = """
            using Microsoft.AspNetCore.Mvc;
            namespace Web.Controllers;

            public class HomeController : Controller
            {
                public IActionResult Index() => View();
            }
            """,
        ["src/Web/Views/_ViewStart.cshtml"] = """
            @{
                Layout = "_Layout";
            }
            """,
        ["src/Web/Views/_ViewImports.cshtml"] = """
            @using Web.Controllers
            @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
            """,
        ["src/Web/Views/Account/Login.cshtml"] = """
            @model LoginViewModel
            <div class="login-page">
                <form asp-route="Login">
                    <input type="hidden" asp-for="ReturnUrl" />
                    <partial name="_ValidationSummary" />
                    <button class="btn btn-primary" name="button" value="login">Login</button>
                </form>
            </div>
            """,
        ["src/Web/Views/Account/LoggedOut.cshtml"] = """
            @{
                ViewData["Title"] = "Logged out";
            }
            <h1>Logout <small>You are now logged out</small></h1>
            @section scripts
            {
                <script src="~/js/signout-redirect.js"></script>
            }
            """,
        ["src/Web/Views/Home/Index.cshtml"] = """
            <h1>Welcome</h1>
            <p>Click <a href="~/account/login">here</a> to sign in.</p>
            """,
        ["src/Web/Views/Shared/_Layout.cshtml"] = """
            <!DOCTYPE html>
            <html lang="en">
            <body>
                <div class="container body-container">@RenderBody()</div>
                @await RenderSectionAsync("scripts", required: false)
            </body>
            </html>
            """,
        ["src/Web/Views/Shared/_ValidationSummary.cshtml"] = """
            @if (ViewContext.ModelState.IsValid == false)
            {
                <div class="alert alert-danger"><div asp-validation-summary="All" class="danger"></div></div>
            }
            """,
        ["src/Web/Views/Shared/Redirect.cshtml"] = """
            @model RedirectViewModel
            <meta http-equiv="refresh" content="0;url=@Model.RedirectUrl" data-url="@Model.RedirectUrl">
            """
    };

    private const string Services = """
        "references": {
          "Services": {
            "packages": [{
              "id": "services", "name": "Web.Services",
              "elements": [
                { "id": "account-controller", "type": "Service", "name": "AccountController", "children": [
                  { "id": "login-op", "type": "Operation", "name": "Login", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Account/Login" } } },
                  { "id": "logout-op", "type": "Operation", "name": "Logout", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "~/Account/Logout" } } }
                ] }
              ]
            }]
          }
        }
        """;

    private static JsonNode Model(string loginRoute = "/Account/Login", string summaryType = "Component", string loginStereotypes = "", string optIn = "", bool redirect = true) =>
        JsonNode.Parse($$"""
        {
          "designers": {
            "User Interface": {
              "packages": [{
                "id": "ui", "name": "Web", "type": "User Interface Package",
                "elements": [
                  { "id": "views", "type": "Folder", "name": "Views", "children": [
                    { "id": "account", "type": "Folder", "name": "Account", "children": [
                      { "id": "login", "type": "Page", "name": "Login", "stereotypes": { "Page": { "Route": "{{loginRoute}}" }{{loginStereotypes}} } },
                      { "id": "logged-out", "type": "Page", "name": "LoggedOut", "stereotypes": { "Page": { "Title": "Logged out" } } }
                    ] },
                    { "id": "home", "type": "Folder", "name": "Home", "children": [
                      { "id": "home-index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/, /Home/Index" } } }
                    ] },
                    { "id": "shared", "type": "Folder", "name": "Shared", "children": [
                      { "id": "layout", "type": "Layout", "name": "_Layout", "children": [
                        { "id": "main", "type": "Region", "name": "Main" },
                        { "id": "scripts", "type": "Region", "name": "scripts" }
                      ] },
                      { "id": "summary", "type": "{{summaryType}}", "name": "_ValidationSummary" }
                      {{(redirect ? """, { "id": "redirect", "type": "Page", "name": "Redirect", "stereotypes": { "Page": { } } }""" : "")}}
                    ] }
                  ] }
                ]
              }],
              "associations": [
                { "id": "main-login", "type": "Composition", "source": { "element": "main" }, "target": { "element": "login" } },
                { "id": "scripts-logged-out", "type": "Composition", "source": { "element": "scripts" }, "target": { "element": "logged-out" } },
                { "id": "login-summary", "type": "Composition", "source": { "element": "login" }, "target": { "element": "summary" } },
                { "id": "login-posts", "type": "Call Service Operation Action", "source": { "element": "login" }, "target": { "element": "login-op" } },
                { "id": "home-login", "type": "Navigation", "source": { "element": "home-index" }, "target": { "element": "login" } }
              ]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Web", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] }{{optIn}} } }]
            }
          },
          {{Services}}
        }
        """)!;

    [Fact]
    public void Views_the_model_states_are_clean()
    {
        var result = Run(Model(), Code);

        Errors(result).ShouldBeEmpty(Describe(result));
        Verified(result, "type.exists").ShouldBe(6, Describe(result));
        Verified(result, "component.kind").ShouldBe(6, Describe(result));
        Verified(result, "page.route").ShouldBe(2, Describe(result));
        Verified(result, "page.title").ShouldBe(1, Describe(result));
        Verified(result, "region.outlet").ShouldBe(2, Describe(result));
        Verified(result, "security").ShouldBe(6, Describe(result));
        Verified(result, "intent.uses").ShouldBe(3, Describe(result));
        Verified(result, "intent.navigates").ShouldBe(1, Describe(result));
        Verified(result, "intent.invokes").ShouldBe(1, Describe(result));
        Verified(result, "intent.target").ShouldBe(4, Describe(result));
    }

    [Fact]
    public void Each_view_is_what_the_code_makes_it_at_the_statement_that_says_so()
    {
        var result = Run(Model(), Code);

        Where(Claim(result, "layout", "component.kind")).ShouldBe("src/Web/Views/_ViewStart.cshtml:2");
        Where(Claim(result, "login", "component.kind")).ShouldBe("src/Web/Controllers/AccountController.cs:7"); // the GET Login's View(...)
        Where(Claim(result, "redirect", "component.kind")).ShouldBe("src/Web/Controllers/AccountController.cs:14"); // through LoadingPage
        Claim(result, "logged-out", "component.kind").Reading!.Value.ShouldBe("Page"); // through Logout(model), the POST overload
        Where(Claim(result, "summary", "component.kind")).ShouldBe("src/Web/Views/Account/Login.cshtml:5");
        Claim(result, "home-index", "page.route").Reading!.Value.ShouldBe("Home, Home/Index"); // and /, which a set's text leaves out
        Claim(result, "main", "region.outlet").Reading!.At!.Line.ShouldBe(4);
        Claim(result, "scripts", "region.outlet").Reading!.At!.Line.ShouldBe(5);
    }

    [Fact]
    public void A_form_posting_to_another_action_is_one_invokes_error()
    {
        var result = Run(Model(), With("src/Web/Views/Account/Login.cshtml", x => x.Replace("asp-route=\"Login\"", "asp-action=\"Logout\"")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/intent.invokes");
        finding.Message.ShouldBe("`Login` invokes `AccountController.Login` (POST): in the model, not in the code. `src/Web/Views/Account/Login.cshtml` posts `asp-action=\"Logout\"` (POST `AccountController.Logout`). The model states it in its Call Service Operation Action. Post a form to the operation's action, or link the operation it posts to.");
    }

    [Fact]
    public void A_removed_partial_is_one_uses_error_and_one_rendered_through_the_html_helper_is_used()
    {
        var removed = Run(Model(), With("src/Web/Views/Account/Login.cshtml", x => x.Replace("<partial name=\"_ValidationSummary\" />", "")));
        var helper = Run(Model(), With("src/Web/Views/Account/Login.cshtml", x => x.Replace("<partial name=\"_ValidationSummary\" />", "@await Html.PartialAsync(\"_ValidationSummary\")")));

        var finding = Single(removed);
        finding.Rule.ShouldBe("claim/intent.uses");
        finding.Message.ShouldBe("`Login` renders `_ValidationSummary`: in the model, not in the code. `src/Web/Views/Account/Login.cshtml` renders no partial or view component. The model states it in its Composition. Render it with `<partial name=\"_ValidationSummary\" />`, or remove the Composition.");
        Errors(helper).ShouldBeEmpty(Describe(helper));
        Claim(helper, "login-summary", "intent.uses").Reading!.At!.Line.ShouldBe(5);
    }

    [Fact]
    public void A_route_no_get_action_serves_is_one_page_route_error()
    {
        var result = Run(Model(loginRoute: "/Account/SignIn"), Code);

        Single(result).Rule.ShouldBe("claim/page.route");
    }

    [Fact]
    public void A_partial_modelled_as_a_page_is_one_kind_error()
    {
        var result = Run(Model(summaryType: "Page"), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/component.kind");
        finding.Message.ShouldBe("`_ValidationSummary` component kind: model `Page`, code `Component`. No controller action renders it; `Login` renders it as a partial. Model it as a Component.");
    }

    [Fact]
    public void A_secured_view_is_one_security_error()
    {
        var result = Run(Model(loginStereotypes: """, "Secured": { "Roles": "Admin" }"""), Code);

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/security");
        finding.Message.ShouldBe("`Login` security: model `Secured`, code `None`. An MVC view carries no authorization of its own: the controller action that renders it does, and the Services model states it on that operation. Leave the view's security unset.");
    }

    [Fact]
    public void A_link_that_reaches_another_page_is_one_navigates_error()
    {
        var result = Run(Model(), With("src/Web/Views/Home/Index.cshtml", x => x.Replace("~/account/login", "~/home/index")));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/intent.navigates");
        finding.Message.ShouldBe("`Index` navigates to `Login`: in the model, not in the code. `Login` is at `/Account/Login`, and `src/Web/Views/Home/Index.cshtml` links to `href=\"~/home/index\"`. The model states it in its Navigation. Link to the page, or remove the Navigation.");
    }

    [Fact]
    public void A_page_shown_in_no_layout_is_one_composition_error_on_its_region()
    {
        var result = Run(Model(), With("src/Web/Views/Account/Login.cshtml", x => "@{ Layout = null; }\n" + x));

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/intent.uses");
        finding.Subject.Element.ShouldBe("main-login");
        finding.Message.ShouldStartWith("`_Layout` places `Login` in its region `Main`: in the model, not in the code. `src/Web/Views/Account/Login.cshtml` is shown in no layout.");
    }

    [Fact]
    public void A_renamed_section_is_one_region_error()
    {
        var result = Run(Model(), With("src/Web/Views/Shared/_Layout.cshtml", x => x.Replace("\"scripts\"", "\"footer\"")));

        var findings = Errors(result);
        string.Join(", ", findings.Select(x => x.Rule)).ShouldBe("claim/region.outlet", Describe(result));
        findings[0].Message.ShouldStartWith("`_Layout` region `scripts`: model `scripts`, code `footer`.");
    }

    [Fact]
    public void A_view_no_element_describes_is_reported_by_the_stock_opt_in_rules_and_its_members_are_never_asked_for()
    {
        const string optIn = """, "Drift Check": { "Opt-in Rules": [ { "Rule": "optin/unmodelled.type" }, { "Rule": "optin/unmodelled.file" }, { "Rule": "optin/unmodelled.member" }, { "Rule": "optin/unmodelled.region" } ] }""";
        var injecting = With("src/Web/Views/Home/Index.cshtml", x => "@inject IConfiguration Configuration\n@functions { private int clicks; }\n" + x);
        var clean = Run(Model(optIn: optIn), injecting);
        var result = Run(Model(optIn: optIn, redirect: false), injecting);

        ViewFindings(clean).ShouldBeEmpty(Describe(clean));
        string.Join(", ", ViewFindings(result).Select(x => x.Rule).Order()).ShouldBe("optin/unmodelled.file, optin/unmodelled.type", Describe(result));
        ViewFindings(result).Single(x => x.Rule == "optin/unmodelled.type").Message.ShouldStartWith("`Redirect` isn't in the model.");
    }

    [Fact]
    public void Html_and_url_helpers_post_and_link_as_the_tag_helpers_do()
    {
        var helpers = With("src/Web/Views/Account/Login.cshtml", x => x.Replace("<form asp-route=\"Login\">", "@using (Html.BeginForm(\"Login\", \"Account\", FormMethod.Post))\n    {").Replace("</form>", "}"));
        helpers["src/Web/Views/Home/Index.cshtml"] = """
            <h1>Welcome</h1>
            <p>@Html.ActionLink("Sign in", "Login", "Account")</p>
            <p><a href="@Url.Action("Index", "Home")">Home</a></p>
            """;
        var result = Run(Model(), helpers);
        var elsewhere = Run(Model(), With("src/Web/Views/Home/Index.cshtml", _ => """<p><a href="@Url.Action("Index", "Home")">Home</a> @Html.ActionLink("Out", "Logout", "Account")</p>"""));

        Errors(result).ShouldBeEmpty(Describe(result));
        Where(Claim(result, "login-posts", "intent.invokes")).ShouldBe("src/Web/Views/Account/Login.cshtml:3");
        Where(Claim(result, "home-login", "intent.navigates")).ShouldBe("src/Web/Views/Home/Index.cshtml:2");
        var finding = Single(elsewhere);
        finding.Rule.ShouldBe("claim/intent.navigates");
        finding.Message.ShouldContain("links to `href=\"@Url.Action(\"Index\", \"Home\")\"`, `Html.ActionLink(\"Out\", \"Logout\", \"Account\")`");
    }

    [Fact]
    public void A_removed_view_isnt_in_the_code()
    {
        var result = Run(Model(), Code.Where(x => x.Key != "src/Web/Views/Account/LoggedOut.cshtml").ToDictionary());

        var finding = Single(result);
        finding.Rule.ShouldBe("claim/type.exists");
        finding.Message.ShouldStartWith("`LoggedOut` isn't in the code");
    }

    [Fact]
    public void Razor_pages_are_pages_at_their_path_and_asp_page_links_reach_them()
    {
        var code = new Dictionary<string, string>
        {
            ["src/Pages/Pages.csproj"] = """<Project Sdk="Microsoft.NET.Sdk.Web"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>""",
            ["src/Pages/Pages/_ViewStart.cshtml"] = "@{ Layout = \"_Layout\"; }",
            ["src/Pages/Pages/Shared/_Layout.cshtml"] = "<main>@RenderBody()</main>",
            ["src/Pages/Pages/Index.cshtml"] = """
                @page
                @model IndexModel
                <a asp-page="/Orders/Details" asp-route-id="5">Order 5</a>
                """,
            ["src/Pages/Pages/Index.cshtml.cs"] = "namespace Pages.Pages; public class IndexModel { }",
            ["src/Pages/Pages/Orders/Details.cshtml"] = """
                @page "{id:int}"
                @attribute [Authorize]
                <h1>Order</h1>
                """
        };
        var model = JsonNode.Parse("""
            {
              "designers": {
                "User Interface": {
                  "packages": [{
                    "id": "ui", "name": "Pages", "type": "User Interface Package",
                    "elements": [
                      { "id": "pages", "type": "Folder", "name": "Pages", "children": [
                        { "id": "index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/" } } },
                        { "id": "orders", "type": "Folder", "name": "Orders", "children": [
                          { "id": "details", "type": "Page", "name": "Details", "stereotypes": { "Page": { "Route": "/Orders/Details/{id}" }, "Secured": { } } }
                        ] },
                        { "id": "shared", "type": "Folder", "name": "Shared", "children": [ { "id": "layout", "type": "Layout", "name": "_Layout" } ] }
                      ] }
                    ]
                  }],
                  "associations": [
                    { "id": "index-details", "type": "Navigation", "source": { "element": "index" }, "target": { "element": "details" } }
                  ]
                },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "Pages", "type": "Root Folder",
                    "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
                }
              }
            }
            """)!;

        var result = Run(model, code);

        Errors(result).ShouldBeEmpty(Describe(result));
        Claim(result, "index", "page.route").Reading!.Value.ShouldBe("Index"); // and /, which a set's text leaves out
        Claim(result, "details", "page.route").Reading!.Value.ShouldBe("Orders/Details/{id}");
        Claim(result, "details", "security").Outcome.ShouldBe("verified");
        Claim(result, "layout", "component.kind").Outcome.ShouldBe("verified");
        Claim(result, "index-details", "intent.navigates").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void Conventional_urls_drop_trailing_segments_their_defaults_fill_and_keep_an_optional_parameter_the_action_takes()
    {
        var code = new Dictionary<string, string>
        {
            ["src/Web/Web.csproj"] = Code["src/Web/Web.csproj"],
            ["src/Web/App_Start/RouteConfig.cs"] = """
                public static class RouteConfig
                {
                    public static void RegisterRoutes(RouteCollection routes) =>
                        routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}", defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
                }
                """,
            ["src/Web/Controllers/OrdersController.cs"] = """
                public class OrdersController : Controller
                {
                    public ActionResult Index() => View();
                    public ActionResult Details(int id) => View();
                }
                """,
            ["src/Web/Views/Orders/Index.cshtml"] = "<h1>Orders</h1>",
            ["src/Web/Views/Orders/Details.cshtml"] = "<h1>Order</h1>"
        };
        var model = JsonNode.Parse("""
            {
              "designers": {
                "User Interface": {
                  "packages": [{
                    "id": "ui", "name": "Web", "type": "User Interface Package",
                    "elements": [
                      { "id": "views", "type": "Folder", "name": "Views", "children": [
                        { "id": "orders", "type": "Folder", "name": "Orders", "children": [
                          { "id": "index", "type": "Page", "name": "Index", "stereotypes": { "Page": { "Route": "/Orders" } } },
                          { "id": "details", "type": "Page", "name": "Details", "stereotypes": { "Page": { "Route": "/Orders/Details/{id?}" } } }
                        ] }
                      ] }
                    ]
                  }]
                },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "Web", "type": "Root Folder",
                    "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
                }
              }
            }
            """)!;

        var result = Run(model, code);

        Errors(result).ShouldBeEmpty(Describe(result));
        Claim(result, "index", "page.route").Reading!.Value.ShouldBe("Orders, Orders/Index");
        Claim(result, "details", "page.route").Reading!.Value.ShouldBe("Orders/Details, Orders/Details/{id?}");
    }

    /// <summary>
    /// eShop's real Identity.API views and Quickstart controllers, read in place, with the User Interface model the eShop
    /// house module proved them against: every view, kind, route, partial, link and form post verifies.
    /// </summary>
    [Fact]
    public void Real_eShop_Identity_views_are_clean()
    {
        var result = Run(IdentityModel(), Identity().Root);

        Errors(result).Where(x => x.Rule != "engine/noVocabulary").ShouldBeEmpty(Describe(result));
        Verified(result, "type.exists").ShouldBe(16, Describe(result));
        Verified(result, "component.kind").ShouldBe(16, Describe(result));
        Verified(result, "page.route").ShouldBe(9, Describe(result));
        Verified(result, "intent.uses").ShouldBe(6, Describe(result));
        Verified(result, "intent.navigates").ShouldBe(2, Describe(result));
        Verified(result, "intent.invokes").ShouldBe(6, Describe(result));
    }

    /// <summary>The house module's two plants on eShop's Identity views, on a copy: a form posting elsewhere, and a removed partial.</summary>
    [Theory]
    [InlineData("Views/Account/Login.cshtml", "asp-route=\"Login\"", "asp-action=\"Logout\"", "claim/intent.invokes",
        "`Login` invokes `AccountController.Login` (POST): in the model, not in the code. `src/Identity.API/Views/Account/Login.cshtml` posts `asp-action=\"Logout\"` (POST `AccountController.Logout`).")]
    [InlineData("Views/Consent/Index.cshtml", "<partial name=\"_ValidationSummary\" />", "", "claim/intent.uses",
        "`Index` renders `_ValidationSummary`: in the model, not in the code. `src/Identity.API/Views/Consent/Index.cshtml` renders `_ScopeListItem`, `_ScopeListItem`.")]
    public void Real_eShop_Identity_plants_are_one_finding_each(string file, string written, string planted, string rule, string message)
    {
        var source = Path.Combine(Identity().Root, "src", "Identity.API");
        var root = Directory.CreateTempSubdirectory("driftengine-mvc-identity-").FullName;
        try
        {
            foreach (var path in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories).Where(x => x.EndsWith(".cs") || x.EndsWith(".cshtml") || x.EndsWith(".csproj")))
            {
                var relative = Path.GetRelativePath(source, path);
                if (relative.Split(Path.DirectorySeparatorChar)[0] is "bin" or "obj")
                {
                    continue;
                }

                var target = Path.Combine(root, "src", "Identity.API", relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                var text = File.ReadAllText(path);
                if (relative.Replace('\\', '/') == file)
                {
                    text.ShouldContain(written);
                    text = text.Replace(written, planted);
                }

                File.WriteAllText(target, text);
            }

            var result = Run(IdentityModel(), root);

            var finding = Single(result);
            finding.Rule.ShouldBe(rule);
            finding.Message.ShouldStartWith(message);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static Codebase Identity()
    {
        var codebase = Codebases.Get("eShop10");
        if (!codebase.IsPresent)
        {
            Assert.Skip($"eShop10 isn't on this machine (looked in {codebase.Root}).");
        }

        return codebase;
    }

    /// <summary>eShop's Identity.API views as the house module modelled them, with the Quickstart controllers' operations they post to.</summary>
    private static JsonNode IdentityModel()
    {
        static string Page(string id, string name, string? route, string type = "Page") =>
            $$"""{ "id": "{{id}}", "type": "{{type}}", "name": "{{name}}"{{(type == "Page" ? $$""", "stereotypes": { "Page": { {{(route is null ? "" : $"\"Route\": \"{route}\"")}} } }""" : "")}} }""";
        static string Operation(string id, string name, string route) =>
            $$"""{ "id": "{{id}}", "type": "Operation", "name": "{{name}}", "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "{{route}}" } } }""";
        static string Link(string id, string type, string source, string target) =>
            $$"""{ "id": "{{id}}", "type": "{{type}}", "source": { "element": "{{source}}" }, "target": { "element": "{{target}}" } }""";
        static string Folder(string name, params string[] children) =>
            $$"""{ "id": "{{name.ToLowerInvariant()}}", "type": "Folder", "name": "{{name}}", "children": [ {{string.Join(", ", children)}} ] }""";

        return JsonNode.Parse($$"""
            {
              "designers": {
                "User Interface": {
                  "packages": [{
                    "id": "ui", "name": "Identity.API", "type": "User Interface Package",
                    "elements": [
                      { "id": "views", "type": "Folder", "name": "Views", "children": [
                        {{Folder("Account", Page("access-denied", "AccessDenied", "/Account/AccessDenied"), Page("logged-out", "LoggedOut", null), Page("login", "Login", "/Account/Login"), Page("logout", "Logout", "/Account/Logout"))}},
                        {{Folder("Consent", Page("consent", "Index", "/Consent/Index"))}},
                        {{Folder("Device", Page("success", "Success", null), Page("capture", "UserCodeCapture", "/Device/Index"), Page("confirmation", "UserCodeConfirmation", null))}},
                        {{Folder("Diagnostics", Page("diagnostics", "Index", "/Diagnostics/Index"))}},
                        {{Folder("Grants", Page("grants", "Index", "/Grants/Index"))}},
                        {{Folder("Home", Page("home", "Index", "/, /Home/Index"))}},
                        {{Folder("Shared", Page("layout", "_Layout", null, "Layout"), Page("error", "Error", "/Home/Error"), Page("redirect", "Redirect", null),
                            Page("scope", "_ScopeListItem", null, "Component"), Page("summary", "_ValidationSummary", null, "Component"))}}
                      ] }
                    ]
                  }],
                  "associations": [
                    {{Link("login-posts", "Call Service Operation Action", "login", "account-login")}},
                    {{Link("login-summary", "Composition", "login", "summary")}},
                    {{Link("logout-posts", "Call Service Operation Action", "logout", "account-logout")}},
                    {{Link("consent-posts", "Call Service Operation Action", "consent", "consent-index")}},
                    {{Link("consent-summary", "Composition", "consent", "summary")}},
                    {{Link("consent-scope", "Composition", "consent", "scope")}},
                    {{Link("capture-posts", "Call Service Operation Action", "capture", "device-capture")}},
                    {{Link("capture-summary", "Composition", "capture", "summary")}},
                    {{Link("confirmation-posts", "Call Service Operation Action", "confirmation", "device-callback")}},
                    {{Link("confirmation-summary", "Composition", "confirmation", "summary")}},
                    {{Link("confirmation-scope", "Composition", "confirmation", "scope")}},
                    {{Link("grants-posts", "Call Service Operation Action", "grants", "grants-revoke")}},
                    {{Link("home-diagnostics", "Navigation", "home", "diagnostics")}},
                    {{Link("home-grants", "Navigation", "home", "grants")}}
                  ]
                },
                "Codebase Structure": {
                  "packages": [{ "id": "root", "name": "eShop", "type": "Root Folder",
                    "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/Identity.API/**", "Scope": "In" } ] } } }]
                }
              },
              "references": {
                "Services": {
                  "packages": [{
                    "id": "services", "name": "Identity.API.Services",
                    "elements": [
                      { "id": "account", "type": "Service", "name": "AccountController", "children": [ {{Operation("account-login", "Login", "~/Account/Login")}}, {{Operation("account-logout", "Logout", "~/Account/Logout")}} ] },
                      { "id": "consent-controller", "type": "Service", "name": "ConsentController", "children": [ {{Operation("consent-index", "Index", "~/Consent/Index")}} ] },
                      { "id": "device", "type": "Service", "name": "DeviceController", "children": [ {{Operation("device-capture", "UserCodeCapture", "~/Device/UserCodeCapture")}}, {{Operation("device-callback", "Callback", "~/Device/Callback")}} ] },
                      { "id": "grants-controller", "type": "Service", "name": "GrantsController", "children": [ {{Operation("grants-revoke", "Revoke", "~/Grants/Revoke")}} ] }
                    ]
                  }]
                }
              }
            }
            """)!;
    }

    // ------------------------------------------------------------------ harness

    private static Dictionary<string, string> With(string path, Func<string, string> change)
    {
        var code = new Dictionary<string, string>(Code);
        code[path] = change(code[path]);
        return code;
    }

    /// <summary>The findings these tests are about: claims, and the opt-in rules on views. The controllers and C# view models aren't modelled here.</summary>
    private static List<Finding> Errors(RunResult result) =>
        result.Findings.Where(x => x.Rule.StartsWith("claim/", StringComparison.Ordinal) || x.Rule == "engine/noVocabulary" || x.Rule.StartsWith("optin/", StringComparison.Ordinal) && IsView(x)).ToList();

    private static List<Finding> ViewFindings(RunResult result) => result.Findings.Where(x => x.Rule.StartsWith("optin/", StringComparison.Ordinal) && IsView(x)).ToList();

    private static bool IsView(Finding finding) => finding.Subject.File?.EndsWith(".cshtml", StringComparison.Ordinal) == true;

    private static Finding Single(RunResult result)
    {
        var findings = Errors(result);
        findings.Count.ShouldBe(1, Describe(result));
        return findings[0];
    }

    private static int Verified(RunResult result, string kind) => result.Claims.Count(x => x.Kind == kind && x.Outcome == "verified");

    private static Claim Claim(RunResult result, string element, string kind) => result.Claims.Single(x => x.Element == element && x.Kind == kind);

    private static string Where(Claim claim) => $"{claim.Reading?.At?.File}:{claim.Reading?.At?.Line}";

    private static string Describe(RunResult result) =>
        "\nClaims:\n" + string.Join("\n", result.Claims.Select(x => $"  {x.Kind} {x.Element} expected={x.Expected} -> {x.Outcome} {x.Reason} {x.Reading?.Value} {x.Reading?.At?.File}:{x.Reading?.At?.Line}"))
        + "\nFindings:\n" + string.Join("\n", result.Findings.Select(x => $"  {x.Rule} [{x.Severity}] {x.Subject.File}:{x.Subject.Line} {x.Subject.Element} {x.Message}"));

    private static RunResult Run(JsonNode model, IReadOnlyDictionary<string, string> code)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-mvc-").FullName;
        try
        {
            foreach (var (path, source) in code)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            return Run(model, root);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static RunResult Run(JsonNode model, string root)
    {
        var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Concat(new[] { "intent.blazor", "intent.mvc" }.Select(x => StockExtensions.Find(x)!)).ToList();
        var designers = FixtureModel.Build(model, out var references);
        return DriftEngine.Run(new DriftInput(designers, root, [Pack.Stock("intent.core")!, Pack.Stock("intent.ui")!], extensions) { References = references });
    }
}

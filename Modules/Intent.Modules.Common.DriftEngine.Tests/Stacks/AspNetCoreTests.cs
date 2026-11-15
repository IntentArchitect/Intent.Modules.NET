using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// What a Minimal API lambda that doesn't write its return type returns, read from its body; a parameter the container
/// provides because the code registers it; and the type a Service or Repository is when its contract and its class differ.
/// The fixtures adapt eShopCustom's Catalog, Webhooks, WebhookClient and Basket code.
/// </summary>
public class AspNetCoreTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Domain": {
              "packages": [{
                "id": "domain", "name": "Shop.Domain",
                "elements": [
                  { "id": "blob-repository", "type": "Repository", "name": "AzureBlobDocumentRepository" }
                ]
              }]
            },
            "Services": {
              "packages": [{
                "id": "services", "name": "Shop.Api",
                "elements": [
                  { "id": "catalog-type", "type": "DTO", "name": "CatalogType" },
                  { "id": "subscription", "type": "DTO", "name": "WebhookSubscription" },
                  { "id": "received", "type": "DTO", "name": "WebHookReceived" },
                  { "id": "webhook-data", "type": "DTO", "name": "WebhookData" },
                  { "id": "catalog-api", "type": "Service", "name": "CatalogApi",
                    "stereotypes": { "Http Service Settings": { "Route": "api/catalog" } },
                    "children": [
                      { "id": "list-types", "type": "Operation", "name": "ListItemTypes", "typeRef": { "element": "catalog-type", "collection": true },
                        "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "catalogtypes" } } },
                      { "id": "list-subscriptions", "type": "Operation", "name": "ListSubscriptions", "typeRef": { "element": "subscription", "collection": true },
                        "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "subscriptions" } },
                        "children": [
                          { "id": "user-id", "type": "Parameter", "name": "userId", "typeRef": { "type": "string" },
                            "stereotypes": { "Parameter Settings": { "Source": "From Query" } } },
                          { "id": "ids", "type": "Parameter", "name": "ids", "typeRef": { "type": "int", "collection": true, "nullable": true },
                            "stereotypes": { "Parameter Settings": { "Source": "From Query" } } }
                        ] },
                      { "id": "webhook-received", "type": "Operation", "name": "WebhookReceived", "typeRef": { "element": "received" },
                        "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "webhook-received" } },
                        "children": [
                          { "id": "hook", "type": "Parameter", "name": "hook", "typeRef": { "element": "webhook-data" },
                            "stereotypes": { "Parameter Settings": { "Source": "From Body" } } }
                        ] },
                      { "id": "logout", "type": "Operation", "name": "Logout",
                        "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "logout" } } },
                      { "id": "list-names", "type": "Operation", "name": "ListTypeNames", "typeRef": { "type": "string", "collection": true },
                        "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "names" } } }
                    ] },
                  { "id": "basket-repository", "type": "Service", "name": "BasketRepository",
                    "children": [
                      { "id": "get-basket", "type": "Operation", "name": "GetBasketAsync", "typeRef": { "type": "string" },
                        "children": [{ "id": "customer-id", "type": "Parameter", "name": "customerId", "typeRef": { "type": "string" } }] }
                    ] },
                  { "id": "catalog-ai", "type": "Service", "name": "CatalogAI",
                    "children": [
                      { "id": "describe", "type": "Operation", "name": "Describe", "typeRef": { "type": "string" },
                        "children": [{ "id": "describe-item", "type": "Parameter", "name": "item", "typeRef": { "type": "string" } }] }
                    ] },
                  { "id": "search-service", "type": "Service", "name": "DocumentSearchService",
                    "children": [
                      { "id": "search", "type": "Operation", "name": "Search", "typeRef": { "type": "string" },
                        "children": [{ "id": "search-request", "type": "Parameter", "name": "request", "typeRef": { "type": "string" } }] },
                      { "id": "get-setting", "type": "Operation", "name": "GetSetting", "typeRef": { "type": "string" } }
                    ] }
                ]
              }]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    private static readonly Dictionary<string, string> Code = new()
    {
        ["src/Api/Program.cs"] = """
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<HooksRepository>();
            var app = builder.Build();
            app.MapCatalogApi();
            app.Run();
            """,
        ["src/Api/Model.cs"] = """
            namespace Shop.Api;
            public class CatalogType { public string Type { get; set; } = ""; }
            public class WebhookSubscription { public string UserId { get; set; } = ""; }
            public class WebHookReceived { public string Data { get; set; } = ""; }
            public class WebhookData { public string Payload { get; set; } = ""; }

            public class ShopContext : DbContext
            {
                public DbSet<CatalogType> CatalogTypes { get; set; } = null!;
                public DbSet<WebhookSubscription> Subscriptions { get; set; } = null!;
            }

            public class HooksRepository
            {
                public Task AddNew(WebHookReceived hook) => Task.CompletedTask;
            }

            public interface IBasketRepository
            {
                Task<string> GetBasketAsync(string customerId);
            }

            public class RedisBasketRepository : IBasketRepository
            {
                public Task<string> GetBasketAsync(string id) => Task.FromResult(id);
            }

            public interface ICatalogAI
            {
                string Describe(string item);
            }

            public class CatalogAI : ICatalogAI
            {
                public string Describe(string items) => items;
            }

            public interface IDocumentSearchService
            {
                string Search(string query);
            }

            public class DocumentSearchService : IDocumentSearchService
            {
                public string Search(string request) => request;

                public string GetSetting() => "";
            }

            public interface IDocumentRepository
            {
                Task SaveAsync(string document);
            }

            public class AzureBlobDocumentRepository : IDocumentRepository
            {
                public Task SaveAsync(string document) => Task.CompletedTask;
            }
            """,
        ["src/Api/CatalogApi.cs"] = """
            namespace Shop.Api;

            public static class CatalogApi
            {
                public static RouteGroupBuilder MapCatalogApi(this IEndpointRouteBuilder app)
                {
                    var api = app.MapGroup("api/catalog");

                    api.MapGet("/catalogtypes", async (ShopContext context) => await context.CatalogTypes.OrderBy(x => x.Type).ToListAsync())
                        .WithName("ListItemTypes");

                    api.MapGet("/subscriptions", async (ShopContext context, string userId, int[]? ids) =>
                    {
                        var data = await context.Subscriptions.Where(s => s.UserId == userId).ToListAsync();
                        return TypedResults.Ok(data);
                    }).WithName("ListSubscriptions");

                    api.MapPost("/webhook-received", async (WebhookData hook, ILogger<WebhookData> logger, HooksRepository hooksRepository) =>
                    {
                        if (hook.Payload == "")
                        {
                            return Results.BadRequest();
                        }

                        var newHook = new WebHookReceived { Data = hook.Payload };
                        await hooksRepository.AddNew(newHook);
                        return Results.Ok(newHook);
                    }).WithName("WebhookReceived");

                    api.MapPost("/logout", async (HttpContext httpContext) =>
                    {
                        await httpContext.SignOutAsync();
                    }).WithName("Logout");

                    api.MapGet("/names", async (ShopContext context) => await context.CatalogTypes.Select(x => x.Type).ToListAsync())
                        .WithName("ListTypeNames");

                    return api;
                }
            }
            """
    };

    private static readonly RunResult Result = Run();

    [Fact]
    public void A_lambda_returning_an_awaited_query_returns_its_list()
    {
        Claim("list-types", "operation.returns").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_lambda_returning_a_success_result_returns_its_payload_through_a_local_given_a_query()
    {
        Claim("list-subscriptions", "operation.returns").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void An_error_result_isnt_what_a_lambda_returns_and_a_created_payload_is()
    {
        Claim("webhook-received", "operation.returns").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_lambda_returning_no_value_returns_nothing()
    {
        Claim("logout", "operation.returns").Outcome.ShouldBe("verified");
    }

    /// <summary><c>Select</c> changes the element to what its lambda gives, which isn't read: never a guess.</summary>
    [Fact]
    public void A_query_whose_element_a_projection_changes_cant_be_verified()
    {
        var claim = Claim("list-names", "operation.returns");
        claim.Outcome.ShouldBe("cantVerify");
        claim.Reason.ShouldBe("unresolved");
    }

    [Fact]
    public void A_class_the_code_registers_with_the_container_is_injected_not_a_request_parameter()
    {
        Claim("webhook-received", "operation.parameters").Reading!.Value.ShouldBe("hook: WebhookData");
        Claim("webhook-received", "operation.parameters").Outcome.ShouldBe("verified");
        Claim("hook", "parameter.source").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_simple_lambda_parameter_no_route_names_is_the_query_string_and_so_is_an_array_of_them()
    {
        Claim("user-id", "parameter.source").Outcome.ShouldBe("verified");
        Claim("ids", "parameter.source").Outcome.ShouldBe("verified");
    }

    /// <summary>No type is called <c>BasketRepository</c>; <c>RedisBasketRepository</c> implements <c>IBasketRepository</c>.</summary>
    [Fact]
    public void A_service_with_no_type_of_its_name_is_its_interface()
    {
        Claim("basket-repository", "type.exists").Outcome.ShouldBe("verified");
        Claim("get-basket", "member.exists").Outcome.ShouldBe("verified");
    }

    /// <summary>
    /// <c>CatalogAI</c> renames its interface's <c>item</c> to <c>items</c>, and the model states the interface's;
    /// <c>DocumentSearchService</c> renames its interface's <c>query</c> to <c>request</c>, and the model states the class's.
    /// The diagram can't show both names, so either declaration is the operation's.
    /// </summary>
    [Fact]
    public void A_class_may_rename_the_parameters_its_interface_declares_and_the_model_may_state_either()
    {
        Claim("describe", "operation.parameters").Outcome.ShouldBe("verified");
        Claim("search", "operation.parameters").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_service_with_a_class_of_its_name_is_the_class_so_a_method_only_the_class_has_is_there()
    {
        Claim("get-setting", "member.exists").Outcome.ShouldBe("verified");
    }

    [Fact]
    public void A_repository_with_no_interface_of_its_name_is_the_class_of_its_name()
    {
        Claim("blob-repository", "type.exists").Outcome.ShouldBe("verified");
    }

    private static Claim Claim(string element, string kind) => Result.Claims.Single(x => x.Element == element && x.Kind == kind);

    private static RunResult Run()
    {
        var root = Directory.CreateTempSubdirectory("driftengine-aspnetcore-").FullName;
        try
        {
            foreach (var (path, source) in Code)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any()).Append(StockExtensions.Find("intent.aspnetcore")!).ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, [Pack.Stock("intent.domain")!, Pack.Stock("intent.services")!], extensions));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

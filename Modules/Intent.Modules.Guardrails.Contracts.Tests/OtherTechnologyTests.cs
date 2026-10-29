using System.Collections.Generic;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>The same engine reads technologies no Intent module supports: only the recipes differ.</summary>
    public class OtherTechnologyTests
    {
        [Fact]
        public void A_minimal_API_route_is_read_from_its_Map_call()
        {
            const string recipe = """
            {
              "facts": {
                "operation.verb": {
                  "find": "file:src/Api/Program.cs > statement with Map(Get|Post|Put|Delete|Patch)\\(\"[^\"]*\",{Operation}\\)",
                  "read": [{ "match": "Map(Get|Post|Put|Delete|Patch)\\(", "case": "upper" }]
                },
                "operation.route": {
                  "find": "file:src/Api/Program.cs > statement with Map\\w+\\(\"[^\"]*\",{Operation}\\)",
                  "read": [{ "match": "Map\\w+\\(\"([^\"]*)\"" }]
                }
              }
            }
            """;
            const string program = """
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/orders/{id}", GetOrder);
            app.MapPost("/orders", CreateOrder).RequireAuthorization();

            app.Run();
            """;
            var files = new Dictionary<string, string> { ["src/Api/Program.cs"] = program };

            var report = Check("Services", recipe, new List<ModelFact>
            {
                Endpoint(FactKinds.OperationVerb, "GetOrder", "GET"),
                Endpoint(FactKinds.OperationRoute, "GetOrder", "/orders/{orderId}"),
                Endpoint(FactKinds.OperationVerb, "CreateOrder", "PUT"),
                Endpoint(FactKinds.OperationRoute, "CreateOrder", "/orders")
            }, files);

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`CreateOrder` verb: model `PUT`, code `POST`. Location: `src/Api/Program.cs:5`",
                "`GetOrder` route: model `/orders/{orderId}`, code `/orders/{id}`. Location: `src/Api/Program.cs:4`"
            });
        }

        [Fact]
        public void A_FastEndpoints_route_is_read_from_its_Configure_method()
        {
            const string recipe = """
            {
              "facts": {
                "operation.verb": {
                  "find": "class with Endpoint<{Operation}\\b > method:Configure",
                  "read": [{ "match": "\\b(Get|Post|Put|Delete|Patch)\\(\"", "case": "upper" }]
                },
                "operation.route": {
                  "find": "class with Endpoint<{Operation}\\b > method:Configure",
                  "read": [{ "match": "\\b(?:Get|Post|Put|Delete|Patch)\\(\"([^\"]+)\"" }]
                },
                "security": {
                  "find": "class with Endpoint<{Operation}\\b > method:Configure",
                  "read": [{ "match": "AllowAnonymous\\(\\)", "value": "Unsecured" }],
                  "default": "Secured"
                }
              }
            }
            """;
            const string endpoint = """
            namespace Acme.Api.Orders
            {
                public class CreateOrderEndpoint : Endpoint<CreateOrderCommand, OrderDto>
                {
                    public override void Configure()
                    {
                        Post("/api/orders");
                        AllowAnonymous();
                    }

                    public override async Task HandleAsync(CreateOrderCommand req, CancellationToken ct)
                    {
                        await SendAsync(new OrderDto());
                    }
                }
            }
            """;
            var files = new Dictionary<string, string> { ["src/Api/Orders/CreateOrderEndpoint.cs"] = endpoint };

            var agrees = Check("Services", recipe, new List<ModelFact>
            {
                Endpoint(FactKinds.OperationVerb, "CreateOrderCommand", "POST"),
                Endpoint(FactKinds.OperationRoute, "CreateOrderCommand", "/api/orders"),
                Endpoint(FactKinds.Security, "CreateOrderCommand", "Unsecured")
            }, files);
            agrees.WrongLines.ShouldBeEmpty();

            var disagrees = Check("Services", recipe, new List<ModelFact>
            {
                Endpoint(FactKinds.OperationRoute, "CreateOrderCommand", "/api/orders/create"),
                Endpoint(FactKinds.Security, "CreateOrderCommand", "Secured")
            }, files);
            disagrees.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`CreateOrderCommand` route: model `/api/orders/create`, code `/api/orders`. Location: `src/Api/Orders/CreateOrderEndpoint.cs:7`",
                "`CreateOrderCommand` security: model `Secured`, code `Unsecured`. Location: `src/Api/Orders/CreateOrderEndpoint.cs:8`"
            });
        }

        [Fact]
        public void A_Mongo_class_map_is_read_from_its_registration()
        {
            const string recipe = """
            {
              "facts": {
                "column.name": {
                  "find": "statement:RegisterClassMap<{Entity}> > chain with MapMember\\(\\w+=>\\w+\\.{Attribute}\\)",
                  "read": [{ "match": "SetElementName\\(\"([^\"]+)\"" }],
                  "default": "",
                  "absent": ""
                },
                "key": {
                  "find": "statement:RegisterClassMap<{Entity}>",
                  "read": [{ "match": "MapIdMember\\(\\w+=>\\w+\\.(\\w+)\\)" }],
                  "default": "Id"
                }
              }
            }
            """;
            const string maps = """
            public static class MongoMaps
            {
                public static void Register()
                {
                    BsonClassMap.RegisterClassMap<Customer>(map =>
                    {
                        map.AutoMap();
                        map.MapIdMember(c => c.CustomerNo);
                        map.MapMember(c => c.Name).SetElementName("name");
                    });
                    BsonClassMap.RegisterClassMap<Invoice>(map => map.AutoMap());
                }
            }
            """;
            var files = new Dictionary<string, string> { ["src/Infrastructure/MongoMaps.cs"] = maps };

            var report = Check("Domain", recipe, new List<ModelFact>
            {
                Table(FactKinds.Key, "Customer", "CustomerNo"),
                Column(FactKinds.ColumnName, "Customer", "Name", "customer_name"),
                Column(FactKinds.ColumnName, "Customer", "Email", ""),
                Table(FactKinds.Key, "Invoice", "InvoiceNo")
            }, files);

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Customer.Name` column name: model `customer_name`, code `name`. Location: `src/Infrastructure/MongoMaps.cs:9`",
                "`Invoice` primary key: model `InvoiceNo`, code `Id` (the code doesn't state it). Location: `src/Infrastructure/MongoMaps.cs:11`"
            });
        }

        [Fact]
        public void A_Cosmos_indexing_policy_is_read_as_a_JSON_file()
        {
            const string recipe = """
            {
              "facts": {
                "key": {
                  "find": "file:deploy/cosmos/{Entity|lower}s.json",
                  "read": [{ "match": "\"partitionKey\":\\{\"paths\":\\[\"/(\\w+)\"" }]
                }
              }
            }
            """;
            const string policy = """
            {
              "id": "invoices",
              "partitionKey": {
                "paths": [ "/tenantId" ]
              }
            }
            """;
            var files = new Dictionary<string, string> { ["deploy/cosmos/invoices.json"] = policy };

            Check("Domain", recipe, new List<ModelFact> { Table(FactKinds.Key, "Invoice", "tenantId") }, files).WrongLines.ShouldBeEmpty();
            Check("Domain", recipe, new List<ModelFact> { Table(FactKinds.Key, "Invoice", "id") }, files).Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Invoice` primary key: model `id`, code `tenantId`. Location: `deploy/cosmos/invoices.json:3`"
            });
        }

        private static ModelFact Endpoint(FactKind kind, string operation, string value)
        {
            return Fact(kind, operation, value, ("Service", ""), ("Operation", operation), ("Folder", ""));
        }
    }
}

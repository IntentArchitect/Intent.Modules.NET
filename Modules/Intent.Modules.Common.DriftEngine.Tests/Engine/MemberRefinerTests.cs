using System.Text.Json.Nodes;
using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Extensions;
using Intent.Modules.Common.DriftEngine.Packs;
using Intent.Modules.Common.DriftEngine.Stacks;
using Intent.Modules.Common.DriftEngine.Tests.Harness;

namespace Intent.Modules.Common.DriftEngine.Tests.Engine;

/// <summary>
/// A house's own helpers mean something only the house knows: the Java spike's <c>Http.parseEnum(OrderStatus.class, status, "status")</c>
/// binds a <c>String status</c> as an <c>OrderStatus</c>, and <c>Http.created(body)</c> responds with <c>body</c>. A house
/// refiner gives the handler as the house reads it, and the stock Spring MVC stack still reads the claim, so the house
/// repeats none of the stack's comparison.
/// </summary>
public class MemberRefinerTests
{
    private static readonly JsonNode Model = JsonNode.Parse("""
        {
          "designers": {
            "Services": {
              "packages": [{
                "id": "services", "name": "Shop.Backend",
                "elements": [
                  { "id": "order-status", "type": "Enum", "name": "OrderStatus",
                    "children": [ { "id": "order-status-placed", "type": "Enum-Literal", "name": "Placed" } ] },
                  { "id": "order-dto", "type": "DTO", "name": "OrderDto" },
                  { "id": "place-order-input", "type": "DTO", "name": "PlaceOrderInput" },
                  { "id": "orders-service", "type": "Service", "name": "OrdersService",
                    "children": [
                      { "id": "get-orders", "type": "Operation", "name": "GetOrders", "typeRef": { "element": "order-dto", "collection": true },
                        "stereotypes": { "Http Settings": { "Verb": "GET", "Route": "api/orders" } },
                        "children": [
                          { "id": "get-orders-status", "type": "Parameter", "name": "status", "typeRef": { "element": "order-status", "nullable": true } },
                          { "id": "get-orders-page-no", "type": "Parameter", "name": "pageNo", "typeRef": { "type": "int" } }
                        ] },
                      { "id": "place-order", "type": "Operation", "name": "PlaceOrder", "typeRef": { "element": "order-dto" },
                        "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "api/orders" } },
                        "children": [
                          { "id": "place-order-body", "type": "Parameter", "name": "body", "typeRef": { "element": "place-order-input" } }
                        ] },
                      { "id": "cancel-order", "type": "Operation", "name": "CancelOrder",
                        "stereotypes": { "Http Settings": { "Verb": "POST", "Route": "api/orders/{orderId}/cancel" } },
                        "children": [
                          { "id": "cancel-order-order-id", "type": "Parameter", "name": "orderId", "typeRef": { "type": "guid" } }
                        ] }
                    ] }
                ]
              }]
            },
            "Codebase Structure": {
              "packages": [{ "id": "root", "name": "Shop.Backend", "type": "Root Folder",
                "stereotypes": { "Drift Scope": { "Entries": [ { "Glob": "src/**", "Scope": "In" } ] } } }]
            }
          }
        }
        """)!;

    private const string Application = """
        package com.shop.application;

        import java.util.List;
        import java.util.UUID;

        public class OrderService {
            public List<OrderDto> list(OrderStatus status, int pageNo) { return List.of(); }
            public OrderDto place(PlaceOrderInput body) { return null; }
            public void cancel(UUID orderId) { }
        }

        record OrderDto(UUID id) { }

        record PlaceOrderInput(UUID customerId) { }

        enum OrderStatus { Placed }

        enum ReturnStatus { Requested }
        """;

    private const string Http = """
        package com.shop.api;

        import org.springframework.http.HttpStatus;
        import org.springframework.http.ResponseEntity;

        final class Http {
            static ResponseEntity<Object> created(Object body) {
                return ResponseEntity.status(HttpStatus.CREATED).body(body);
            }

            static <E extends Enum<E>> E parseEnum(Class<E> type, String raw, String parameter) {
                return null;
            }
        }
        """;

    private const string Controller = """
        package com.shop.api;

        import com.shop.application.*;
        import java.util.List;
        import java.util.UUID;
        import org.springframework.http.ResponseEntity;
        import org.springframework.web.bind.annotation.*;

        @RestController
        public class OrdersController {
            private final OrderService orders;

            public OrdersController(OrderService orders) {
                this.orders = orders;
            }

            @GetMapping("/api/orders")
            public List<OrderDto> getOrders(@RequestParam(required = false) String status, @RequestParam int pageNo) {
                return orders.list(Http.parseEnum(OrderStatus.class, status, "status"), pageNo);
            }

            @PostMapping("/api/orders")
            public ResponseEntity<Object> placeOrder(@RequestBody PlaceOrderInput body) {
                return Http.created(orders.place(body));
            }

            @PostMapping("/api/orders/{orderId}/cancel")
            public ResponseEntity<Object> cancelOrder(@PathVariable UUID orderId) {
                orders.cancel(orderId);
                return Http.created(null);
            }
        }
        """;

    [Fact]
    public void Without_the_house_the_stack_cant_read_its_helpers()
    {
        var result = Run(Controller, house: false);
        Outcome(result, "operation.parameters", "get-orders").ShouldBe("contradicted");
        Outcome(result, "operation.returns", "place-order").ShouldBe("cantVerify");
        Outcome(result, "operation.returns", "cancel-order").ShouldBe("cantVerify");
    }

    [Fact]
    public void The_stack_reads_the_handler_as_the_house_refines_it()
    {
        var result = Run(Controller, house: true);
        Outcome(result, "operation.parameters", "get-orders").ShouldBe("verified");
        Outcome(result, "operation.returns", "place-order").ShouldBe("verified");
        Outcome(result, "operation.returns", "cancel-order").ShouldBe("verified");
        Outcome(result, "operation.parameters", "place-order").ShouldBe("verified");
    }

    [Fact]
    public void What_the_house_refines_is_still_compared()
    {
        var planted = Controller
            .Replace("Http.parseEnum(OrderStatus.class", "Http.parseEnum(ReturnStatus.class", StringComparison.Ordinal)
            .Replace("Http.created(null)", "Http.created(orders.place(null))", StringComparison.Ordinal);
        var result = Run(planted, house: true);
        Outcome(result, "operation.parameters", "get-orders").ShouldBe("contradicted");
        Outcome(result, "operation.returns", "cancel-order").ShouldBe("contradicted");
        Outcome(result, "operation.returns", "place-order").ShouldBe("verified");
    }

    private static string Outcome(RunResult result, string kind, string element) => result.Claims.Single(x => x.Kind == kind && x.Element == element).Outcome;

    private static RunResult Run(string controller, bool house)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-refiner-").FullName;
        try
        {
            Write(root, "src/com/shop/application/OrderService.java", Application);
            Write(root, "src/com/shop/api/Http.java", Http);
            Write(root, "src/com/shop/api/OrdersController.java", controller);
            string[] stock = ["intent.core", "intent.services", "intent.java", "intent.springmvc"];
            var packs = stock.Select(Pack.Stock).OfType<Pack>().ToList();
            var extensions = StockExtensions.All.Where(x => !x.ClaimReaders.Any())
                .Concat(stock.Select(StockExtensions.Find).OfType<IDriftExtension>())
                .Concat(house ? [new HouseExtension()] : [])
                .Distinct()
                .ToList();
            return DriftEngine.Run(new DriftInput(FixtureModel.Build(Model), root, packs, extensions));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static void Write(string root, string path, string content)
    {
        var file = Path.Combine(root, path);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, content);
    }

    private sealed class HouseExtension : IDriftExtension
    {
        public string Id => "house";

        public IEnumerable<IMemberRefiner> Refiners => [new HttpHelpers()];
    }

    /// <summary><c>Http.parseEnum(X.class, raw, ..)</c> binds <c>raw</c> as an <c>X</c>; <c>Http.created(body)</c> responds with <c>body</c>.</summary>
    private sealed class HttpHelpers : IMemberRefiner
    {
        public IReadOnlyCollection<string> Kinds { get; } = ["operation.parameters", "operation.returns"];

        public MemberNode? Refine(ClaimContext context)
        {
            if (context.Member is not { } member)
            {
                return null;
            }

            if (context.Kind == "operation.returns")
            {
                var created = member.Calls.FirstOrDefault(x => x is { Receiver: "Http", Name: "created", Arguments: [_] });
                var body = created?.Arguments[0] is { Text: "null" } ? Ref("Void") : created?.Arguments[0] is { IsKnown: true, Types: [var only] } ? only : null;
                return body is null ? null : member with { Type = Ref("ResponseEntity", body) };
            }

            var parsed = member.Calls
                .Where(x => x is { Receiver: "Http", Name: "parseEnum", Arguments: [{ Text: var type }, _, _] } && type.EndsWith(".class", StringComparison.Ordinal))
                .ToDictionary(x => x.Arguments[1].Text, x => x.Arguments[0].Text[..^".class".Length]);
            return parsed.Count == 0
                ? null
                : member with { Parameters = member.Parameters.Select(x => parsed.TryGetValue(x.Name, out var type) ? x with { Type = Ref(type) } : x).ToList() };
        }

        private static TypeRefNode Ref(string name, TypeRefNode? argument = null) => new()
        {
            Text = argument is null ? name : $"{name}<{argument.Text}>", Name = name, QualifiedName = name, Arguments = argument is null ? [] : [argument]
        };
    }
}

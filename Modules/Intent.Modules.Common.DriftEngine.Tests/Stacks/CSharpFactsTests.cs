using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.CSharp;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The declaration facts the C# language gives the graph that no claim kind reads yet, so no acceptance case covers them:
/// what house rules (and the Java language, filling the same nodes) are written against.
/// </summary>
public class CSharpFactsTests
{
    private const string Source = """
        namespace Shop;

        public interface IOrders { string Name { get; } }

        public class Paged<T> { }

        class Internal
        {
            public int Count { get; set; }
            int _hidden;
            protected string? Label;
            private readonly int _limit = 10;
            public const int Max = 5;

            public override string ToString() => "";

            public void Get(string name, int pageSize = 10) { }

            class Nested { }
        }

        public class OrderService(IMediator mediator)
        {
            public void Cancel() => mediator.Send(new CancelOrder());
        }

        public record OrderDto(string Name, [property: JsonPropertyName("n")] int Count);

        public enum Status { Draft = 1, Paid }

        [Index(nameof(Code), new[] { "a", "b" }, IsUnique = true)]
        public class Product { public string Code { get; set; } = ""; }

        public class Downloads : IProgress<int>, IComparable<Downloads>
        {
            void IProgress<int>.Report(int value) { }

            public int CompareTo(Downloads? other) => 0;
        }
        """;

    private const string TargetSource = """
        namespace Shop;

        public class OrdersController(OrderService orders)
        {
            public async Task<IResult> Place(PlaceOrder command, List<OrderDto> items)
            {
                var placed = await orders.PlaceAsync(command);
                Audit(placed);
                Audit(orders.Find(1));
                orders.Find("a", 2);
                orders.Log("a", "b", "c");
                orders.Describe();
                Http.Parse("x");
                Audit(orders.First(items));
                items.ForEach(Audit);
                items.Where(x => x != null).ToList();
                return Results.Created("", orders.Place(command));
            }

            private void Audit(object value) { }
        }

        public class ServiceBase { public string Describe() => ""; }

        public class OrderService : ServiceBase
        {
            public Task<OrderDto> PlaceAsync(PlaceOrder command) => null!;
            public OrderDto Place(PlaceOrder command) => null!;
            public OrderDto Find(int id) => null!;
            public OrderDto Find(string name, int page = 1) => null!;
            public void Log(params string[] values) { }
            public T First<T>(List<T> items) => default!;
        }

        public static class Http { public static int Parse(string value) => 0; }
        """;

    private const string LiteralSource = """
        namespace Shop;

        public class Setup
        {
            private const string Schema = "sales";

            public void Configure(IServiceCollection services, int id)
            {
                services.AddScoped<IOrders, Orders>();
                Use("orders", @"C:\data", 'x', -1, 18.4m, true, null, nameof(Order.Total), typeof(Order), ("a\tb"));
                Use($"api/orders/{id}/lines", Schema, "a" + "b");
                var created = Create<Order>();
                Audit(created);
                Audit(Create<Order>());
            }

            private void Use(params object?[] values) { }

            private T Create<T>() => default!;

            private void Audit(object value) { }
        }
        """;

    private const string CreationSource = """
        namespace Shop;

        public class OrdersClient(HttpClient http)
        {
            public async Task Send(Guid id, string token)
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"api/orders/{id}")
                {
                    Content = JsonContent.Create(id),
                    Headers = { { "X-Token", token } },
                    ["trace"] = "on"
                };
                var tags = new List<string> { "a", "b" };
                await http.SendAsync(request);
                await http.SendAsync(id == Guid.Empty ? new HttpRequestMessage() : request);
            }
        }
        """;

    private const string UnitSource = """
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddScoped<IOrders, Orders>();
        builder.Services.AddSwaggerGen(o =>
        {
            o.CustomSchemaIds(x => x.FullName);
            o.Title = "Orders";
        });
        var app = builder.Build();
        app.MapGet("/orders/{id}", async ([FromRoute] Guid id, ISender sender) => await sender.Send(new GetOrder(id)));
        app.MapPost("/orders", (PlaceOrder command) =>
        {
            if (command is null)
            {
                return Results.BadRequest();
            }

            var handle = () => { return 1; };
            return Results.Ok(new Placed());
        });
        await app.RunAsync();

        public partial class Program
        {
            public static void Seed() => Console.WriteLine("seed");
        }

        public class Orders
        {
            public string Name => "x";

            public int Count() => 1;

            public void Log() => Console.WriteLine("x");

            public int Find(int id)
            {
                Func<int> later = () => { return 2; };
                return id;
            }
        }
        """;

    private const string ValueSource = """
        namespace Shop;

        public interface IMessage { }

        public class Placed : IMessage { }

        public class QuoteResult { }

        public class Entities : DbContext
        {
            public virtual ObjectResult<QuoteResult> GetQuotes(int id) => null!;
        }

        public class QuoteService(OrderService orders)
        {
            private readonly DbContext _context = null!;

            public async Task Quote(PlaceOrder command)
            {
                var placed = await orders.PlaceAsync(command).ConfigureAwait(false);
                Audit(placed);
                Audit(await orders.PlaceAsync(command).ConfigureAwait(false));
                var context = (Entities)_context;
                Audit(context.GetQuotes(1).FirstOrDefault());
                Audit((IMessage)new Placed());
                Audit((object)placed);
                Audit(await Task.FromResult(new QuoteResult()));
                Audit(await Task.Run(() => orders.Place(command)));
                Audit(await Task.Run(() => orders.PlaceAsync(command)));
                var model = new object();
                model = "running";
                Audit(model);
                var count = 0;
                count = Http.Count();
                Audit(count);
                Store(command, out OrderDto stored);
                Audit(stored);
                Store(command, out var kept);
                Audit(kept);
                int.TryParse("1", out var number);
                Audit(number);
            }

            private void Audit(object value) { }

            private void Store(PlaceOrder command, out OrderDto stored) => stored = null!;
        }

        public class OrderService
        {
            public Task<OrderDto> PlaceAsync(PlaceOrder command) => null!;
            public OrderDto Place(PlaceOrder command) => null!;
        }
        """;

    private static readonly CodeGraph Graph = Read(Source);

    private static readonly CodeGraph Values = Read(ValueSource);

    private static readonly CodeGraph Creations = Read(CreationSource);

    private static readonly CodeGraph Units = Read(UnitSource);

    private static readonly CodeGraph Targets = Read(TargetSource);

    private static readonly CodeGraph Literals = Read(LiteralSource);

    [Fact]
    public void A_property_or_field_has_its_access_as_written_or_the_language_default()
    {
        var type = Type("Internal");
        Member(type, "Count").Access.ShouldBe("public");
        Member(type, "_hidden").Access.ShouldBe("private");
        Member(type, "Label").Access.ShouldBe("protected");
        Member(Type("IOrders"), "Name").Access.ShouldBe("public");
    }

    [Fact]
    public void An_explicit_implementation_names_the_interface_it_implements_and_an_implicit_one_none()
    {
        var report = Member(Type("Downloads"), "Report");
        report.Implements.ShouldNotBeNull();
        report.Implements.Name.ShouldBe("IProgress");
        report.Implements.Text.ShouldBe("IProgress<int>");
        Member(Type("Downloads"), "CompareTo").Implements.ShouldBeNull();
    }

    [Fact]
    public void A_type_has_its_access_and_its_own_type_parameters()
    {
        Type("Paged").Access.ShouldBe("public");
        Type("Paged").TypeParameters.ShouldBe(["T"]);
        Type("Internal").Access.ShouldBe("internal");
        Type("Nested").Access.ShouldBe("private");
    }

    [Fact]
    public void A_class_primary_constructor_is_a_constructor_not_properties()
    {
        var type = Type("OrderService");
        type.Members.ShouldNotContain(x => x.Kind == "property");
        var constructor = type.Members.Single(x => x.Kind == "constructor");
        constructor.IsPrimary.ShouldBeTrue();
        constructor.Parameters.Select(x => x.Name).ShouldBe(["mediator"]);

        // Its parameters still type what the body calls on them.
        Member(type, "Cancel").Calls.Single().ReceiverType!.Name.ShouldBe("IMediator");
    }

    [Fact]
    public void A_positional_record_has_properties_with_their_attributes_and_a_primary_constructor()
    {
        var type = Type("OrderDto");
        type.Members.Where(x => x.Kind == "property").Select(x => x.Name).ShouldBe(["Name", "Count"]);
        Member(type, "Count").Attributes.Single().Name.ShouldBe("JsonPropertyName");
        type.Members.Single(x => x.Kind == "constructor").IsPrimary.ShouldBeTrue();
    }

    [Fact]
    public void A_member_has_its_other_modifiers_and_its_written_value()
    {
        var type = Type("Internal");
        Member(type, "ToString").Modifiers.ShouldBe(["override"]);
        Member(type, "_limit").Modifiers.ShouldBe(["readonly"]);
        Member(type, "_limit").Value!.Text.ShouldBe("10");
        Member(type, "Max").Modifiers.ShouldBe(["const"]);
        Member(type, "Count").Value.ShouldBeNull();
    }

    [Fact]
    public void An_enum_member_has_its_value_only_when_written()
    {
        var type = Type("Status");
        Member(type, "Draft").Value!.Text.ShouldBe("1");
        Member(type, "Paid").Value.ShouldBeNull();
    }

    [Fact]
    public void A_parameter_has_its_default_as_written()
    {
        var get = Member(Type("Internal"), "Get");
        get.Parameters.Select(x => x.Default).ShouldBe([null, "10"]);
    }

    [Fact]
    public void An_attribute_has_its_arguments_structured()
    {
        var values = Type("Product").Attributes.Single().Values;
        values.Select(x => x.Name).ShouldBe([null, null, "IsUnique"]);
        values[1].Items.Select(x => x.Literal).ShouldBe(["a", "b"]);
        values[2].Literal.ShouldBe("true");

        // nameof(Code) is the string Code.
        values[0].Literal.ShouldBe("Code");
        Type("Product").Attributes.Single().Arguments[0].ShouldBe("Code");
    }

    [Fact]
    public void A_call_has_the_method_it_reaches_by_name_and_arity_through_the_type_it_is_made_on()
    {
        var calls = Calls("Place");
        var find = calls.Where(x => x.Name == "Find").ToList();

        // Both Finds take one argument (one has a default), and Find(1)'s int fits only Find(int id).
        find[0].Target!.Member.Parameters.Select(x => x.Name).ShouldBe(["id"]);
        find[1].Target!.Member.Parameters.Select(x => x.Name).ShouldBe(["name", "page"]);
        calls.Single(x => x.Name == "Log").Target!.Member.Name.ShouldBe("Log");

        // An inherited method is reached on the type that declares it.
        calls.Single(x => x.Name == "Describe").Target!.Type.Name.ShouldBe("ServiceBase");

        // A call on a type name reaches its static methods.
        calls.Single(x => x.Name == "Parse").Target!.Type.Name.ShouldBe("Http");
        calls.Single(x => x.Name == "Created").Target.ShouldBeNull();
    }

    [Fact]
    public void A_call_used_as_a_value_is_what_its_method_returns_and_an_awaited_task_what_it_gives()
    {
        var audits = Calls("Place").Where(x => x is { Name: "Audit", IsReference: false }).ToList();
        audits[0].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
        // Find(1) takes one argument as both overloads can, and its int fits only Find(int).
        audits[1].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");

        // A method's own type parameter isn't a type the code names.
        audits[2].Arguments.Single().IsKnown.ShouldBeFalse();
        Calls("Place").Single(x => x.Name == "Created").Arguments[1].Types.Single().Name.ShouldBe("OrderDto");
    }

    [Fact]
    public void An_await_through_ConfigureAwait_gives_what_the_task_gives_and_has_the_call_it_awaits()
    {
        var audits = ValueAudits();
        audits[0].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
        audits[1].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
        audits[1].Arguments.Single().Call!.Name.ShouldBe("PlaceAsync");
    }

    [Fact]
    public void A_downcast_is_the_type_it_names_and_an_upcast_or_a_cast_to_object_is_what_it_casts()
    {
        // _context is a DbContext; cast to the code's own Entities, its calls reach Entities' methods.
        var getQuotes = Values.Types.Single(x => x.Name == "QuoteService").Members.Single(x => x.Name == "Quote").Calls.Single(x => x.Name == "GetQuotes");
        getQuotes.ReceiverType!.Name.ShouldBe("Entities");
        getQuotes.Target!.Type.Name.ShouldBe("Entities");

        // Placed implements IMessage, so the cast keeps the creation.
        var audits = ValueAudits();
        audits[3].Arguments.Single().Types.Single().Name.ShouldBe("Placed");
        audits[3].Arguments.Single().Creations.ShouldHaveSingleItem();
        audits[4].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
    }

    [Fact]
    public void An_EF6_stored_procedure_result_is_a_query_source()
    {
        ValueAudits()[2].Arguments.Single().Types.Single().Name.ShouldBe("QuoteResult");
    }

    [Fact]
    public void A_task_from_a_result_or_from_a_run_is_a_task_of_what_it_is_given()
    {
        var audits = ValueAudits();
        audits[5].Arguments.Single().Types.Single().Name.ShouldBe("QuoteResult");
        audits[6].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
        // A task the lambda gives is the one Task.Run runs.
        audits[7].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
    }

    [Fact]
    public void A_var_local_whose_values_disagree_or_are_not_all_known_is_its_initializer_s_type()
    {
        var audits = ValueAudits();
        audits[8].Arguments.Single().IsKnown.ShouldBeTrue();
        audits[8].Arguments.Single().Types.Single().Name.ShouldBe("object");
        audits[9].Arguments.Single().IsKnown.ShouldBeTrue();
        audits[9].Arguments.Single().Types.Single().Name.ShouldBe("int");
    }

    [Fact]
    public void A_local_an_out_argument_declares_is_the_type_it_names_or_for_out_var_the_parameter_s_it_binds_to()
    {
        var audits = ValueAudits();
        audits[10].Arguments.Single().IsKnown.ShouldBeTrue();
        audits[10].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
        audits[11].Arguments.Single().IsKnown.ShouldBeTrue();
        audits[11].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
        // A library's method states no out parameter the reader can see.
        audits[12].Arguments.Single().IsKnown.ShouldBeFalse();
    }

    [Fact]
    public void A_fluent_chain_of_library_calls_however_long_is_read_without_exhausting_the_stack()
    {
        // A chain of a library's calls was once typed by building each call's node afresh, a level of recursion per call, so
        // a long enough one crashed the Software Factory with a stack overflow.
        var chain = string.Concat(Enumerable.Repeat(".Append(\"x\")", 1000));
        var graph = Read($$"""
            namespace Shop;

            public class Report
            {
                public string Build(System.Text.StringBuilder text) => text{{chain}}.ToString();
            }
            """);

        graph.Types.Single().Members.Single(x => x.Name == "Build").Calls.Count.ShouldBe(1001);
    }

    [Fact]
    public void A_method_group_passed_as_a_value_is_a_reference_to_the_method()
    {
        var reference = Calls("Place").Single(x => x.IsReference);
        reference.Name.ShouldBe("Audit");
        reference.Receiver.ShouldBeNull();
        reference.Target!.Type.Name.ShouldBe("OrdersController");
        reference.Arguments.ShouldBeEmpty();
    }

    [Fact]
    public void A_call_made_on_a_call_has_that_call_which_is_also_one_of_the_body()
    {
        var calls = Calls("Place");
        var toList = calls.Single(x => x.Name == "ToList");
        toList.On.ShouldBeSameAs(calls.Single(x => x.Name == "Where"));
        toList.On!.On.ShouldBeNull();
        calls.Single(x => x.Name == "Created").On.ShouldBeNull();
    }

    [Fact]
    public void A_literal_argument_has_its_value_the_same_whatever_its_quoting()
    {
        var literals = LiteralCalls().First(x => x.Name == "Use").Arguments.Select(x => (x.Literal!.Kind, x.Literal.Value)).ToList();
        literals.ShouldBe([
            ("string", "orders"), ("string", @"C:\data"), ("char", "x"), ("number", "-1"), ("number", "18.4m"), ("boolean", "true"),
            ("null", null), ("string", "Total"), ("type", null), ("string", "a\tb")
        ]);
        LiteralCalls().First(x => x.Name == "Use").Arguments[8].Literal!.Type!.Name.ShouldBe("Order");

        // nameof is a string, not a call.
        LiteralCalls().ShouldNotContain(x => x.Name == "nameof");
    }

    [Fact]
    public void An_interpolated_string_has_its_text_and_holes_in_order_and_a_name_or_an_expression_is_no_literal()
    {
        var arguments = LiteralCalls().Last(x => x.Name == "Use").Arguments;
        var interpolated = arguments[0].Literal!;
        interpolated.Kind.ShouldBe("interpolated");
        interpolated.Value.ShouldBeNull();
        interpolated.Parts.Select(x => x.Literal?.Value).ShouldBe(["api/orders/", null, "/lines"]);
        interpolated.Parts[1].Text.ShouldBe("id");
        interpolated.Parts[1].Types.Single().Name.ShouldBe("int");

        arguments[1].Literal.ShouldBeNull();
        arguments[2].Literal.ShouldBeNull();
        Literals.Types.Single().Members.Single(x => x.Name == "Schema").Value!.Literal!.Value.ShouldBe("sales");
    }

    [Fact]
    public void A_call_has_its_type_arguments_and_a_method_returning_its_type_parameter_gives_the_one_passed()
    {
        LiteralCalls().Single(x => x.Name == "AddScoped").TypeArguments.Select(x => x.Name).ShouldBe(["IOrders", "Orders"]);
        LiteralCalls().First(x => x.Name == "Use").TypeArguments.ShouldBeEmpty();

        var audits = LiteralCalls().Where(x => x.Name == "Audit").ToList();
        audits[0].Arguments.Single().Types.Single().Name.ShouldBe("Order");
        audits[1].Arguments.Single().Types.Single().Name.ShouldBe("Order");
    }

    [Fact]
    public void A_creation_has_its_arguments_initializer_and_items()
    {
        var send = Creations.Types.Single().Members.Single(x => x.Name == "Send");
        send.Creates.Select(x => x.Name).ShouldBe(["HttpRequestMessage", "List", "HttpRequestMessage"]);

        var request = send.Creations[0];
        request.Arguments.Select(x => x.Text).ShouldBe(["HttpMethod.Post", "$\"api/orders/{id}\""]);
        request.Arguments[1].Literal!.Parts[0].Literal!.Value.ShouldBe("api/orders/");
        request.Initializers.Select(x => x.Member).ShouldBe(["Content", "Headers", "[\"trace\"]"]);
        request.Initializers[0].Value.Text.ShouldBe("JsonContent.Create(id)");
        request.Initializers[2].Value.Literal!.Value.ShouldBe("on");
        request.Items.ShouldBeEmpty();

        send.Creations[1].Items.Select(x => x.Literal!.Value).ShouldBe(["a", "b"]);
        send.Creations[1].Initializers.ShouldBeEmpty();
    }

    [Fact]
    public void A_value_has_the_creations_it_can_be_through_a_local_and_a_conditional()
    {
        var send = Creations.Types.Single().Members.Single(x => x.Name == "Send");
        var sends = send.Calls.Where(x => x.Name == "SendAsync").ToList();
        sends[0].Arguments.Single().Creations.Single().ShouldBeSameAs(send.Creations[0]);
        sends[1].Arguments.Single().Creations.ShouldBe([send.Creations[2], send.Creations[0]]);

        // A call's value isn't a creation.
        send.Creations[0].Initializers[0].Value.Creations.ShouldBeEmpty();
    }

    [Fact]
    public void A_member_s_text_is_Common_Drift_s_canonical_text()
    {
        // The expected text is what Common.Drift's CodeText gives for this method, so converted recipes' regular expressions
        // read the same text (R34). A probe of both over the five copied codebases (2,459 files, 81,170 members and
        // statements) found none that differ.
        var graph = Read(""""
            public class Orders
            {
                [HttpGet("{id:guid}")]
                public async Task<ActionResult<OrderDto?>> Get([FromRoute] Guid id, CancellationToken ct = default)
                {
                    var url = $"orders/{id}?page={page,5:N0}&x={(a ? "y" : "z")}";
                    var raw = $$"""{"id": {{id}}}""";
                    var items = await _db.Orders.Where(x => x.Total >= 10 && !x.IsDeleted).Select(x => new { x.Id, Name = x.Name ?? "" }).ToListAsync(ct);
                    int[] counts = [1, 2, ..others];
                    if (items is { Count: > 0 } list && list[0]?.Id is not null) { return Ok(items!); }
                    return NotFound();
                }
            }
            """");

        Member(graph.Types.Single(), "Get").Text.ShouldBe(""""
            [HttpGet("{id:guid}")]public async Task<ActionResult<OrderDto?>>Get([FromRoute]Guid id,CancellationToken ct=default){var url=$"orders/{id}?page={page,5:N0}&x={(a?"y":"z")}";var raw=$$"""{"id": {{id}}}""";var items=await _db.Orders.Where(x=>x.Total>=10&&!x.IsDeleted).Select(x=>new{x.Id,Name=x.Name??""}).ToListAsync(ct);int[]counts=[1,2,..others];if(items is{Count:>0}list&&list[0]?.Id is not null){return Ok(items!);}return NotFound();}
            """");
    }

    [Fact]
    public void Top_level_statements_are_a_unit_with_their_lambdas_code_and_not_the_types_declared_after_them()
    {
        var program = Units.Files.Single().TopLevel!;
        program.Calls.Select(x => x.Name).ShouldBe(["CreateBuilder", "AddScoped", "AddSwaggerGen", "CustomSchemaIds", "Build", "MapGet", "Send", "MapPost", "BadRequest", "Ok", "RunAsync"]);
        program.Calls.Single(x => x.Name == "AddScoped").TypeArguments.Select(x => x.Name).ShouldBe(["IOrders", "Orders"]);
        program.Assignments.Select(x => (x.Target, x.Value.Literal?.Value)).ShouldBe([("o.Title", "Orders")]);
        program.Parameters.ShouldBeEmpty();
        program.IsAsync.ShouldBeTrue();
        program.At.Line.ShouldBe(1);
    }

    [Fact]
    public void A_lambda_is_a_unit_with_its_parameters_attributes_calls_and_returns()
    {
        var program = Units.Files.Single().TopLevel!;
        var get = program.Calls.Single(x => x.Name == "MapGet").Arguments[1].Function!;
        get.Parameters.Select(x => (x.Name, x.Type?.Name)).ShouldBe([("id", "Guid"), ("sender", "ISender")]);
        get.Parameters[0].Attributes.Single().Name.ShouldBe("FromRoute");
        get.IsAsync.ShouldBeTrue();
        get.Calls.Single().ShouldBeSameAs(program.Calls.Single(x => x.Name == "Send"));
        get.Creations.Single().Type.Name.ShouldBe("GetOrder");
        get.Returns.Single().Text.ShouldBe("await sender.Send(new GetOrder(id))");

        // A nested lambda's return isn't the handler's.
        var post = program.Calls.Single(x => x.Name == "MapPost").Arguments[1].Function!;
        post.Returns.Select(x => x.Text).ShouldBe(["Results.BadRequest()", "Results.Ok(new Placed())"]);

        var options = program.Calls.Single(x => x.Name == "AddSwaggerGen").Arguments.Single().Function!;
        options.Parameters.Single().Type.ShouldBeNull();
        options.Assignments.Single().Target.ShouldBe("o.Title");
        options.Assignments.Single().Receiver!.Text.ShouldBe("o");
        options.Returns.ShouldBeEmpty();
    }

    [Fact]
    public void A_member_body_returns_its_expression_body_unless_void_and_not_a_nested_lambdas_return()
    {
        var orders = Units.Types.Single(x => x.Name == "Orders");
        Member(orders, "Count").Body!.Returns.Single().Text.ShouldBe("1");
        Member(orders, "Log").Body!.Returns.ShouldBeEmpty();
        Member(orders, "Find").Body!.Returns.Single().Text.ShouldBe("id");
        Member(orders, "Name").Body.ShouldBeNull();

        // A partial Program's own members aren't top-level code.
        Member(Units.Types.Single(x => x.Name == "Program"), "Seed").Calls.Single().Name.ShouldBe("WriteLine");
    }

    [Fact]
    public void A_using_alias_names_its_type_and_a_namespace_using_decides_between_same_named_types()
    {
        var graph = ReadAll(
            ("Order.cs", "namespace Shop.Domain; public class Order { }"),
            ("ViewOrder.cs", "namespace Shop.Views; public class Order { }"),
            ("Api.cs", """
                using Shop.Views;
                using OrderEntity = Shop.Domain.Order;
                using static Shop.Domain.Order;

                namespace Shop.Api;

                public class Orders
                {
                    public OrderEntity Entity { get; set; }

                    public Order View { get; set; }
                }
                """));
        var imports = graph.Files.Single(x => x.Path == "Api.cs").Imports;
        imports.Select(x => x.Wildcard).ShouldBe([true, false, false]);
        graph.TypeOf(imports[0]).ShouldBeNull();
        graph.TypeOf(imports[1])!.Namespace.ShouldBe("Shop.Domain");
        graph.TypeOf(imports[2])!.Namespace.ShouldBe("Shop.Domain");

        var orders = graph.Types.Single(x => x.Name == "Orders");
        graph.TypeFor(Member(orders, "Entity").Type!, orders)!.Namespace.ShouldBe("Shop.Domain");
        graph.TypeFor(Member(orders, "View").Type!, orders)!.Namespace.ShouldBe("Shop.Views");
    }

    private static IReadOnlyList<CallNode> LiteralCalls() => Literals.Types.Single().Members.Single(x => x.Name == "Configure").Calls;

    private static List<CallNode> ValueAudits() =>
        Values.Types.Single(x => x.Name == "QuoteService").Members.Single(x => x.Name == "Quote").Calls.Where(x => x is { Name: "Audit", IsReference: false }).ToList();

    private static IReadOnlyList<CallNode> Calls(string member) => Targets.Types.Single(x => x.Name == "OrdersController").Members.Single(x => x.Name == member).Calls;

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    /// <summary>
    /// The type <c>LegacyHandler.Handle</c>'s parameter, <c>PlaceOrder</c>, resolves to, where the code's only
    /// <c>PlaceOrder</c> is in <c>Shop.Commands</c>, which the handler's file doesn't import, and <c>OrderHandler</c>'s, whose
    /// file does.
    /// </summary>
    private static (TypeNode? Legacy, TypeNode? Imported, TypeNode Own) PlaceOrderSeen(string? project)
    {
        var files = new List<(string Path, string Source)>
        {
            ("Shop/Commands/PlaceOrder.cs", "namespace Shop.Commands; public class PlaceOrder { }"),
            ("Shop/Legacy/LegacyHandler.cs", "using Contracts.Orders; namespace Shop.Legacy; public class LegacyHandler { public void Handle(PlaceOrder message) { } }"),
            ("Shop/Orders/OrderHandler.cs", "using Shop.Commands; namespace Shop.Orders; public class OrderHandler { public void Handle(PlaceOrder message) { } }")
        };
        if (project is not null)
        {
            files.Add(("Shop/Shop.csproj", project));
        }

        var graph = ReadAll(files.ToArray());

        TypeNode? Seen(string handler)
        {
            var type = graph.Types.Single(x => x.Name == handler);
            return graph.TypeFor(type.Members.Single(x => x.Name == "Handle").Parameters.Single().Type!, type);
        }

        return (Seen("LegacyHandler"), Seen("OrderHandler"), graph.Types.Single(x => x.Name == "PlaceOrder"));
    }

    [Fact]
    public void A_name_a_file_can_t_reach_the_code_s_only_type_of_is_a_library_s_when_its_project_adds_no_imports()
    {
        var (legacy, imported, own) = PlaceOrderSeen("""<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>""");
        legacy.ShouldBeNull();
        imported.ShouldBeSameAs(own);
    }

    [Fact]
    public void A_name_is_the_code_s_only_type_of_it_where_the_file_s_imports_may_not_all_be_known()
    {
        // No project governs the files, so nothing says what else they import.
        var ungoverned = PlaceOrderSeen(null);
        ungoverned.Legacy.ShouldBeSameAs(ungoverned.Own);

        // A project's <Using> items are imports the graph doesn't hold.
        var usings = PlaceOrderSeen("""<Project Sdk="Microsoft.NET.Sdk"><ItemGroup><Using Include="Shop.Commands" /></ItemGroup></Project>""");
        usings.Legacy.ShouldBeSameAs(usings.Own);
    }

    [Fact]
    public void A_using_inside_a_namespace_names_what_it_finds_from_that_namespace_outwards()
    {
        // Projects that add no imports of their own, so a file's imports are all it can name.
        const string project = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>""";
        var graph = ReadAll(
            ("src/Domain/Domain.csproj", project),
            ("src/Domain/Account.cs", """
                namespace Domain;

                using System.Text;
                using Credits;

                public class Account
                {
                    public CreditsCollection Credits { get; } = new();
                }
                """),
            ("src/Domain/Credits/CreditsCollection.cs", """
                namespace Domain.Credits;

                public sealed class CreditsCollection : List<Credit>
                {
                }

                public class Credit
                {
                }
                """),
            ("src/Application/Application.csproj", project),
            ("src/Application/UseCases/Deposit/DepositUseCase.cs", """
                namespace Application.UseCases.Deposit
                {
                    using Services;

                    public class DepositUseCase
                    {
                        public IExchange Exchange { get; } = null!;
                    }
                }
                """),
            ("src/Application/Services/IExchange.cs", """
                namespace Application.Services;

                public interface IExchange
                {
                }
                """));

        // Domain's using Credits is Domain.Credits; a library's namespace stays as written.
        var account = graph.Types.Single(x => x.Name == "Account");
        graph.Files.Single(x => x.Path == "src/Domain/Account.cs").Imports.Select(x => x.Target).ShouldBe(["System.Text", "Domain.Credits"]);
        graph.TypeFor(account.Members.Single(x => x.Name == "Credits").Type!, account).ShouldBeSameAs(graph.Types.Single(x => x.Name == "CreditsCollection"));

        // Inside Application.UseCases.Deposit, using Services is Application.Services, two namespaces out.
        var deposit = graph.Types.Single(x => x.Name == "DepositUseCase");
        graph.Files.Single(x => x.Path == "src/Application/UseCases/Deposit/DepositUseCase.cs").Imports.Single().Target.ShouldBe("Application.Services");
        graph.TypeFor(deposit.Members.Single(x => x.Name == "Exchange").Type!, deposit).ShouldBeSameAs(graph.Types.Single(x => x.Name == "IExchange"));
    }

    private static CodeGraph Read(string source) => ReadAll(("Shop.cs", source));

    private static CodeGraph ReadAll(params (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-csharp-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(root, path))!);
                File.WriteAllText(Path.Combine(root, path), source);
            }

            var graph = new CodeGraph();
            new CSharpExtension().Languages.Single().Read(root, files.Select(x => x.Path).ToList(), graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

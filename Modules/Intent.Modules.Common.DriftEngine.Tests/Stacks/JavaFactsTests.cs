using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Java;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The facts the Java language gives the graph: the same nodes the C# language fills, from Java's own syntax. The shapes
/// the grammar makes awkward (annotations on the parent node, parameters split over nested lists, implements and permits
/// told apart by their keyword, <c>instanceof</c> patterns) are each covered.
/// </summary>
public class JavaFactsTests
{
    private const string Source = """
        package com.shop.domain;

        import java.util.List;
        import static java.util.Objects.requireNonNull;
        import com.shop.common.*;

        @Entity
        @Table(name = "orders", uniqueConstraints = @UniqueConstraint(columnNames = {"customer_id", "number"}))
        public class Order extends AggregateRoot implements Comparable<Order>, Auditable {
            @Id
            private UUID id;
            @Column(nullable = false, length = 50)
            private String customerName;
            int count = 10, limit;
            @NotNull private BigDecimal total;
            @OneToMany(cascade = {CascadeType.ALL, CascadeType.MERGE})
            private List<OrderLine> lines;
            private Optional<Address> deliveryTo;
            private Integer reorderLevel;
            protected static final String KIND = "order";

            protected Order() {
            }

            public Order(UUID id, @NotNull String customerName, String... tags) {
                this.id = id;
            }

            public <T extends Comparable<T>> List<? extends T> sorted(Map.Entry<String, int[]> entry) {
                return null;
            }

            public void check(Object value) {
                if (value instanceof String s) {
                }
            }

            static class Line { }
        }

        public interface OrderRepository extends JpaRepository<Order, UUID> {
            int MAX = 5;

            @Query("select o from Order o")
            List<Order> findOpen();

            default void touch() { }

            enum Mode { Fast, Slow }
        }

        public enum OrderStatus { Placed, @Deprecated Shipped;
            OrderStatus() { }
        }

        public record OrderDto(@JsonProperty("n") String name, int count) implements Serializable { }

        sealed abstract class Shape permits Circle { }
        """;

    private const string BodySource = """
        package com.shop.api;

        public class OrdersController {
            private final OrderService orders;
            private final Services services;

            public OrdersController(OrderService orders, Services services) {
                this.orders = orders;
                this.services = services;
            }

            OrdersController() {
                this(null, null);
            }

            public PagedResult<OrderDto> getOrders(String status, int pageNo, int pageSize) {
                return orders.list(Http.parseEnum(OrderStatus.class, status, "status"), pageNo, pageSize);
            }

            public ResponseEntity<Object> placeOrder(PlaceOrderRequest request) {
                var command = new PlaceOrderCommand(request.customerId());
                Object result = flag() ? new Receipt() : (Object) command;
                audit(result);
                this.orders.place(command);
                services.mediator.send(command);
                return Http.created(orders.place(command));
            }

            public void each(List<Order> list) {
                list.forEach((Order o) -> audit(o));
                list.forEach(o -> audit(o));
            }

            private boolean flag() { return true; }

            private void audit(Object value) { }
        }

        class Services {
            Mediator mediator;
        }
        """;

    private const string TargetSource = """
        package com.shop.api;

        public class OrdersController {
            private final OrderService orders;

            public OrdersController(OrderService orders) {
                this.orders = orders;
            }

            public ResponseEntity<Object> placeOrder(PlaceOrderCommand command, List<Order> list) {
                var placed = orders.place(command);
                audit(placed);
                orders.find(1);
                orders.find("a", 2);
                orders.log("a", "b", "c");
                orders.describe();
                Http.parseEnum(OrderStatus.class, "x", "status");
                audit(orders.first(list));
                list.forEach(this::audit);
                list.forEach(Order::markAsPaid);
                list.stream().map(Order::new);
                return Http.created(orders.place(command));
            }

            private void audit(Object value) { }
        }

        class ServiceBase {
            String describe() { return ""; }
        }

        class OrderService extends ServiceBase {
            OrderDto place(PlaceOrderCommand command) { return null; }
            OrderDto find(int id) { return null; }
            OrderDto find(String name, int page) { return null; }
            void log(String... values) { }
            <T> T first(List<T> items) { return null; }
        }

        class Order {
            void markAsPaid() { }
        }

        final class Http {
            static ResponseEntity<Object> created(Object body) { return null; }
            static <E extends Enum<E>> E parseEnum(Class<E> type, String value, String name) { return null; }
        }
        """;

    private const string LiteralSource = """"
        package com.shop.api;

        import java.util.function.Supplier;

        public class Setup {
            private static final String SCHEMA = "sales";
            private static final String QUERY = """
                select *
                  from orders\
                 where id = ?
                """;

            public void configure() {
                use("orders", "a\tbA", 'x', -1, 18.4f, true, null, OrderStatus.class, (42L));
                use(SCHEMA, "a" + "b");
                var created = this.<Order>create();
                audit(created);
                audit(this.<Order>create());
                Supplier<Order> make = this::<Order>create;
                var request = new Request("POST", "/api/orders");
                int[] sizes = new int[3];
                send(request);
            }

            private void use(Object... values) { }

            private <T> T create() { return null; }

            private void audit(Object value) { }
        }
        """";

    private const string UnitSource = """
        package com.shop.api;

        public class Units {
            private int total;

            public int find(int id) {
                Runnable later = () -> { return; };
                orders.forEach(o -> { o.status = "PAID"; audit(o); });
                items.map((Integer x) -> x * 2);
                Object anonymous = new Object() { public String toString() { return "x"; } };
                this.total = id;
                return id;
            }

            private void audit(Object value) { }
        }
        """;

    private static readonly CodeGraph Graph = Read(Source);

    private static readonly CodeGraph Units = Read(UnitSource);

    private static readonly CodeGraph Bodies = Read(BodySource);

    private static readonly CodeGraph Targets = Read(TargetSource);

    private static readonly CodeGraph Literals = Read(LiteralSource);

    [Fact]
    public void A_file_has_its_package_and_imports()
    {
        var file = Graph.Files.Single();
        file.Language.ShouldBe("java");
        file.Namespaces.ShouldBe(["com.shop.domain"]);
        file.Imports.Select(x => x.Target).ShouldBe(["java.util.List", "java.util.Objects.requireNonNull", "com.shop.common"]);
        file.Imports.Select(x => x.Static).ShouldBe([false, true, false]);
        file.Imports.Select(x => x.Wildcard).ShouldBe([false, false, true]);
    }

    [Fact]
    public void An_import_resolves_to_its_in_scope_type_and_decides_which_of_two_same_named_types_a_name_is()
    {
        var graph = ReadAll(
            ("Order.java", "package com.shop.domain; public class Order { }"),
            ("Line.java", "package com.shop.domain; public class Line { }"),
            ("ViewOrder.java", "package com.shop.views; public class Order { }"),
            ("Date.java", "package com.shop.api; public class Date { }"),
            ("Orders.java", """
                package com.shop.api;

                import com.shop.domain.Order;
                import com.shop.views.*;
                import java.util.Date;
                import static com.shop.domain.Line.of;

                public class Orders {
                    private Order order;
                    private Date placed;
                }
                """),
            ("Views.java", """
                package com.shop.api;

                import com.shop.views.*;

                public class Views {
                    private Order order;
                }
                """));
        var orders = graph.Types.Single(x => x.Name == "Orders");
        var imports = graph.Files.Single(x => x.Path == "Orders.java").Imports;
        graph.TypeOf(imports[0])!.Namespace.ShouldBe("com.shop.domain");
        graph.TypeOf(imports[1]).ShouldBeNull();
        graph.TypeOf(imports[2]).ShouldBeNull();
        graph.TypeOf(imports[3])!.Name.ShouldBe("Line");

        // A single-type import wins over a wildcard one, and a library type's import over the package's own type.
        graph.TypeFor(Member(orders, "order").Type!, orders)!.Namespace.ShouldBe("com.shop.domain");
        graph.TypeFor(Member(orders, "placed").Type!, orders).ShouldBeNull();

        // Of two same-named types in other packages, the one a wildcard import reaches.
        var views = graph.Types.Single(x => x.Name == "Views");
        graph.TypeFor(Member(views, "order").Type!, views)!.Namespace.ShouldBe("com.shop.views");
    }

    [Fact]
    public void A_type_has_its_kind_access_annotations_and_supertypes()
    {
        var order = Type("Order");
        order.Kind.ShouldBe("class");
        order.Namespace.ShouldBe("com.shop.domain");
        order.Access.ShouldBe("public");
        order.At.Line.ShouldBe(7);
        order.Attributes.Select(x => x.Name).ShouldBe(["Entity", "Table"]);
        order.BaseTypes.Select(x => x.Text).ShouldBe(["AggregateRoot", "Comparable<Order>", "Auditable"]);

        Type("OrderRepository").Kind.ShouldBe("interface");
        Type("OrderRepository").BaseTypes.Single().Arguments.Select(x => x.Name).ShouldBe(["Order", "UUID"]);
        Type("OrderStatus").Kind.ShouldBe("enum");
        Type("OrderDto").Kind.ShouldBe("record");
        Type("OrderDto").BaseTypes.Single().Name.ShouldBe("Serializable");
    }

    [Fact]
    public void A_type_without_an_access_modifier_is_package_private_unless_an_interface_declares_it()
    {
        Type("Shape").Access.ShouldBe("package");
        Type("Shape").IsAbstract.ShouldBeTrue();
        Type("Shape").BaseTypes.ShouldBeEmpty();
        Type("Line").Access.ShouldBe("package");
        Type("Line").IsStatic.ShouldBeTrue();
        Type("Mode").Access.ShouldBe("public");
    }

    [Fact]
    public void A_field_has_its_type_access_modifiers_annotations_and_value()
    {
        var order = Type("Order");
        Member(order, "customerName").Access.ShouldBe("private");
        Member(order, "customerName").Type!.Name.ShouldBe("String");
        Member(order, "count").Access.ShouldBe("package");
        Member(order, "count").Value!.Text.ShouldBe("10");
        Member(order, "limit").Type!.Name.ShouldBe("int");
        Member(order, "KIND").IsStatic.ShouldBeTrue();
        Member(order, "KIND").Modifiers.ShouldBe(["final"]);
        Member(order, "lines").Type!.Text.ShouldBe("List<OrderLine>");
        Member(order, "lines").Type!.Arguments.Single().Name.ShouldBe("OrderLine");
    }

    [Fact]
    public void A_primitive_is_not_nullable_a_boxed_type_is_and_another_reference_is_unknown_unless_annotated()
    {
        var order = Type("Order");
        Member(order, "count").Type!.Nullable.ShouldBe("false");
        Member(order, "customerName").Type!.Nullable.ShouldBe("unknown");
        Member(order, "total").Type!.Nullable.ShouldBe("false");
        Member(order, "deliveryTo").Type!.Nullable.ShouldBe("true");
        Member(order, "reorderLevel").Type!.Nullable.ShouldBe("true");
        Member(order, "lines").Type!.Nullable.ShouldBe("unknown");
        Member(Type("Order"), "check").Parameters.Single().Type!.Nullable.ShouldBe("unknown");
        Member(Type("OrderDto"), "count").Type!.Nullable.ShouldBe("false");
        Type("OrderRepository").Members.Single(x => x.Name == "MAX").Type!.Nullable.ShouldBe("false");
    }

    [Fact]
    public void An_annotation_has_its_arguments_structured()
    {
        var table = Type("Order").Attributes.Single(x => x.Name == "Table");
        table.Named["name"].ShouldBe("orders");
        var constraint = table.Values.Single(x => x.Name == "uniqueConstraints").Annotation!;
        constraint.Name.ShouldBe("UniqueConstraint");
        constraint.Values.Single().Items.Select(x => x.Literal).ShouldBe(["customer_id", "number"]);

        var column = Member(Type("Order"), "customerName").Attributes.Single();
        column.Named["nullable"].ShouldBe("false");
        column.Named["length"].ShouldBe("50");

        Member(Type("Order"), "lines").Attributes.Single().Values.Single().Items.Select(x => x.Text).ShouldBe(["CascadeType.ALL", "CascadeType.MERGE"]);
        Member(Type("OrderRepository"), "findOpen").Attributes.Single().Arguments.ShouldBe(["select o from Order o"]);
    }

    [Fact]
    public void A_constructor_and_a_method_have_their_parameters()
    {
        var order = Type("Order");
        var constructors = order.Members.Where(x => x.Kind == "constructor").ToList();
        constructors.Select(x => x.Access).ShouldBe(["protected", "public"]);
        var parameters = constructors[1].Parameters;
        parameters.Select(x => x.Name).ShouldBe(["id", "customerName", "tags"]);
        parameters[1].Attributes.Single().Name.ShouldBe("NotNull");
        parameters[2].Type!.IsArray.ShouldBeTrue();

        var sorted = Member(order, "sorted");
        sorted.Type!.Text.ShouldBe("List<? extends T>");
        sorted.Parameters.Single().Type!.QualifiedName.ShouldBe("Map.Entry");
        sorted.Parameters.Single().Type!.Arguments.Select(x => x.Text).ShouldBe(["String", "int[]"]);
        Member(order, "check").Type!.Name.ShouldBe("void");
    }

    [Fact]
    public void An_interface_member_is_public_and_its_constants_static()
    {
        var repository = Type("OrderRepository");
        Member(repository, "MAX").Access.ShouldBe("public");
        Member(repository, "MAX").IsStatic.ShouldBeTrue();
        Member(repository, "findOpen").Access.ShouldBe("public");
        Member(repository, "touch").Modifiers.ShouldBe(["default"]);
    }

    [Fact]
    public void An_enum_has_its_constants_and_a_private_constructor()
    {
        var status = Type("OrderStatus");
        status.Members.Where(x => x.Kind == "enumMember").Select(x => x.Name).ShouldBe(["Placed", "Shipped"]);
        Member(status, "Shipped").Attributes.Single().Name.ShouldBe("Deprecated");
        status.Members.Single(x => x.Kind == "constructor").Access.ShouldBe("private");
    }

    [Fact]
    public void A_record_has_its_components_as_properties_and_a_canonical_constructor()
    {
        var dto = Type("OrderDto");
        dto.Members.Where(x => x.Kind == "property").Select(x => x.Name).ShouldBe(["name", "count"]);
        Member(dto, "name").Attributes.Single().Arguments.ShouldBe(["n"]);
        var constructor = dto.Members.Single(x => x.Kind == "constructor");
        constructor.IsPrimary.ShouldBeTrue();
        constructor.Parameters.Select(x => x.Name).ShouldBe(["name", "count"]);
    }

    [Fact]
    public void An_instanceof_pattern_parses_without_errors()
    {
        Graph.Failures.ShouldBeEmpty();
        JavaSyntax.Of(Graph)!.Trees.Keys.ShouldBe(["Shop.java"]);
    }

    [Fact]
    public void A_body_has_its_calls_with_what_they_are_made_on_and_their_arguments()
    {
        Bodies.Failures.ShouldBeEmpty();
        var calls = Member(Body("OrdersController"), "getOrders").Calls;
        calls.Select(x => x.Name).ShouldBe(["list", "parseEnum"]);

        var list = calls[0];
        list.Receiver.ShouldBe("orders");
        list.ReceiverType!.Name.ShouldBe("OrderService");
        list.Arguments.Select(x => x.Text).ShouldBe(["Http.parseEnum(OrderStatus.class, status, \"status\")", "pageNo", "pageSize"]);
        list.Arguments[1].Types.Single().Name.ShouldBe("int");

        // A static call's receiver is a type, which the code doesn't type.
        var parseEnum = calls[1];
        parseEnum.Receiver.ShouldBe("Http");
        parseEnum.ReceiverType.ShouldBeNull();
        parseEnum.Text.ShouldBe("Http.parseEnum(OrderStatus.class, status, \"status\")");
        parseEnum.Arguments[0].IsKnown.ShouldBeFalse();
        parseEnum.Arguments[1].Types.Single().Name.ShouldBe("String");
        parseEnum.At.Line.ShouldBe(17);
    }

    [Fact]
    public void A_value_is_typed_by_its_creation_local_cast_conditional_and_field_path()
    {
        var placeOrder = Member(Body("OrdersController"), "placeOrder");
        placeOrder.Calls.Select(x => x.Name).ShouldBe(["customerId", "flag", "audit", "place", "send", "created", "place"]);
        placeOrder.Calls[0].ReceiverType!.Name.ShouldBe("PlaceOrderRequest");
        placeOrder.Calls[1].Receiver.ShouldBeNull();

        // A typed local is what it's given: one branch creates a Receipt, the other casts the command.
        placeOrder.Calls[2].Arguments.Single().Types.Select(x => x.Name).ShouldBe(["Receipt", "PlaceOrderCommand"]);
        placeOrder.Calls[3].Receiver.ShouldBe("this.orders");
        placeOrder.Calls[3].ReceiverType!.Name.ShouldBe("OrderService");
        placeOrder.Calls[3].Arguments.Single().Types.Single().Name.ShouldBe("PlaceOrderCommand");
        placeOrder.Calls[4].ReceiverType!.Name.ShouldBe("Mediator");
        placeOrder.Calls[5].Arguments.Single().Text.ShouldBe("orders.place(command)");
        placeOrder.Creates.Select(x => x.Name).ShouldBe(["PlaceOrderCommand", "Receipt"]);
    }

    [Fact]
    public void A_lambda_parameter_is_typed_only_when_written_and_a_constructor_call_is_not_a_call()
    {
        var calls = Member(Body("OrdersController"), "each").Calls.Where(x => x.Name == "audit").ToList();
        calls[0].Arguments.Single().Types.Single().Name.ShouldBe("Order");
        calls[1].Arguments.Single().IsKnown.ShouldBeFalse();

        Body("OrdersController").Members.Where(x => x.Kind == "constructor").SelectMany(x => x.Calls).ShouldBeEmpty();
    }

    [Fact]
    public void A_call_has_the_method_it_reaches_by_name_and_arity_through_the_type_it_is_made_on()
    {
        Targets.Failures.ShouldBeEmpty();
        var calls = Member(Target("OrdersController"), "placeOrder").Calls;
        var find = calls.Where(x => x.Name == "find").ToList();
        find[0].Target!.Member.Parameters.Select(x => x.Name).ShouldBe(["id"]);
        find[1].Target!.Member.Parameters.Select(x => x.Name).ShouldBe(["name", "page"]);
        calls.Single(x => x.Name == "log").Target!.Member.Name.ShouldBe("log");

        // An inherited method is reached on the type that declares it.
        calls.Single(x => x.Name == "describe").Target!.Type.Name.ShouldBe("ServiceBase");

        // A call on a type name reaches its static methods.
        var parseEnum = calls.Single(x => x.Name == "parseEnum");
        parseEnum.Target!.Type.Name.ShouldBe("Http");
        parseEnum.Target.Returns.ShouldBeNull();
        calls.Single(x => x.Name == "forEach" && x.Arguments.Single().Text == "this::audit").Target.ShouldBeNull();
    }

    [Fact]
    public void A_call_used_as_a_value_is_what_its_method_returns()
    {
        var calls = Member(Target("OrdersController"), "placeOrder").Calls;
        var audits = calls.Where(x => x is { Name: "audit", IsReference: false }).ToList();
        audits[0].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");

        // A method's own type parameter isn't a type the code names.
        audits[1].Arguments.Single().IsKnown.ShouldBeFalse();
        calls.Single(x => x.Name == "created").Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");
    }

    [Fact]
    public void A_method_reference_is_a_call_to_the_method_and_a_constructor_reference_is_not()
    {
        var references = Member(Target("OrdersController"), "placeOrder").Calls.Where(x => x.IsReference).ToList();
        references.Select(x => x.Text).ShouldBe(["this::audit", "Order::markAsPaid"]);
        references[0].Receiver.ShouldBeNull();
        references[0].Target!.Type.Name.ShouldBe("OrdersController");
        references[1].Receiver.ShouldBe("Order");
        references[1].Target!.Type.Name.ShouldBe("Order");
        references[1].At.Line.ShouldBe(20);
    }

    [Fact]
    public void A_call_made_on_a_call_has_that_call_which_is_also_one_of_the_body()
    {
        var calls = Member(Target("OrdersController"), "placeOrder").Calls;
        var map = calls.Single(x => x.Name == "map");
        map.On.ShouldBeSameAs(calls.Single(x => x.Name == "stream"));
        map.On!.On.ShouldBeNull();
        calls.Single(x => x.Name == "created").On.ShouldBeNull();
    }

    [Fact]
    public void A_literal_argument_has_its_value_with_its_escapes_resolved_and_a_name_or_an_expression_is_no_literal()
    {
        var uses = LiteralCalls().Where(x => x.Name == "use").ToList();
        uses[0].Arguments.Select(x => (x.Literal!.Kind, x.Literal.Value)).ShouldBe([
            ("string", "orders"), ("string", "a\tbA"), ("char", "x"), ("number", "-1"), ("number", "18.4f"), ("boolean", "true"), ("null", null),
            ("type", null), ("number", "42L")
        ]);
        uses[0].Arguments[7].Literal!.Type!.Name.ShouldBe("OrderStatus");
        uses[1].Arguments.ShouldAllBe(x => x.Literal == null);
    }

    [Fact]
    public void A_field_has_its_literal_and_a_text_block_is_its_lines_without_their_indentation()
    {
        var setup = Literals.Types.Single();
        Member(setup, "SCHEMA").Value!.Literal!.Value.ShouldBe("sales");
        Member(setup, "QUERY").Value!.Literal!.Value.ShouldBe("select *\n  from orders where id = ?\n");
    }

    [Fact]
    public void A_call_has_its_type_arguments_and_a_method_returning_its_type_parameter_gives_the_one_passed()
    {
        var calls = LiteralCalls();
        var create = calls.Where(x => x.Name == "create").ToList();
        create.Count.ShouldBe(3);
        create.ShouldAllBe(x => x.TypeArguments.Single().Name == "Order");
        create.Single(x => x.IsReference).Target!.Member.Name.ShouldBe("create");
        calls.First(x => x.Name == "use").TypeArguments.ShouldBeEmpty();

        var audits = calls.Where(x => x.Name == "audit").ToList();
        audits[0].Arguments.Single().Types.Single().Name.ShouldBe("Order");
        audits[1].Arguments.Single().Types.Single().Name.ShouldBe("Order");
    }

    [Fact]
    public void A_creation_has_its_arguments_and_a_value_the_creation_it_is_through_a_local()
    {
        var configure = Member(Literals.Types.Single(), "configure");

        // An array creation isn't one.
        var request = configure.Creations.Single();
        request.Type.Name.ShouldBe("Request");
        request.Arguments.Select(x => x.Literal!.Value).ShouldBe(["POST", "/api/orders"]);
        configure.Calls.Single(x => x.Name == "send").Arguments.Single().Creations.Single().ShouldBeSameAs(request);
    }

    [Fact]
    public void A_body_and_a_lambda_are_units_with_their_assignments_and_own_returns()
    {
        var find = Member(Units.Types.Single(), "find").Body!;
        find.Parameters.Single().Name.ShouldBe("id");

        // Neither a lambda's nor an anonymous class's return is the method's.
        find.Returns.Single().Text.ShouldBe("id");
        find.Assignments.Select(x => x.Target).ShouldBe(["o.status", "this.total"]);

        var each = find.Calls.Single(x => x.Name == "forEach").Arguments.Single().Function!;
        each.Parameters.Single().Name.ShouldBe("o");
        each.Parameters.Single().Type.ShouldBeNull();
        each.Assignments.Single().Value.Literal!.Value.ShouldBe("PAID");
        each.Calls.Single().ShouldBeSameAs(find.Calls.Single(x => x.Name == "audit"));
        each.Returns.ShouldBeEmpty();

        var twice = find.Calls.Single(x => x.Name == "map").Arguments.Single().Function!;
        twice.Parameters.Single().Type!.Name.ShouldBe("Integer");
        twice.Returns.Count.ShouldBe(1);
    }

    private static IReadOnlyList<CallNode> LiteralCalls() => Literals.Types.Single().Members.Single(x => x.Name == "configure").Calls;

    private static TypeNode Target(string name) => Targets.Types.Single(x => x.Name == name);

    private static TypeNode Body(string name) => Bodies.Types.Single(x => x.Name == name);

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    private static CodeGraph Read(string source) => ReadAll(("Shop.java", source));

    private static CodeGraph ReadAll(params (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-java-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                File.WriteAllText(Path.Combine(root, path), source);
            }

            var graph = new CodeGraph();
            new JavaExtension().Languages.Single().Read(root, files.Select(x => x.Path).ToList(), graph);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Python;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The facts the Python language gives the graph: the same nodes the C# and Java languages fill, from Python's own syntax.
/// The shapes Python makes different are each covered: a module's name from its packages, annotations as types and their
/// nullability, <c>self</c> as the type's own, a call to a class as a creation, keyword arguments, and a module's functions
/// and variables as a type of their own.
/// </summary>
public class PythonFactsTests
{
    private const string Source = """"
        """Orders."""
        from __future__ import annotations

        import uuid
        from dataclasses import dataclass, field
        from decimal import Decimal
        from enum import Enum, auto
        from typing import Annotated, Any, ClassVar, Final, Optional, Protocol

        from shop.common import *
        from .base import AggregateRoot as Root


        @dataclass(frozen=True)
        class Order(Root):
            id: uuid.UUID
            customer_name: str
            total: Decimal = Decimal("0")
            lines: list[OrderLine] = field(default_factory=list)
            delivery_to: Optional[Address] = None
            reorder_level: int | None = None
            notes: "str | None" = None
            code: Annotated[str, Field(max_length=10)] = ""
            KIND: ClassVar[str] = "order"
            _audit: Any = None

            def __init__(self, id: uuid.UUID, customer_name: str, *tags: str, **options) -> None:
                self.id = id
                self.customer_name = customer_name
                self._count = 0

            @property
            def count(self) -> int:
                return self._count

            @count.setter
            def count(self, value: int) -> None:
                self._count = value

            def sorted[T](self, entry: dict[str, list[int]], /, limit: int = 10, *, reverse: bool = False) -> list[T]:
                return []

            @staticmethod
            def create(name: str) -> Order:
                return Order(uuid.uuid4(), name)

            @classmethod
            def empty(cls) -> "Order":
                return cls(uuid.uuid4(), "")

            async def place(self) -> None:
                pass

            def __mangle(self):
                pass

            class Line:
                pass


        class OrderRepository(Protocol):
            def find_open(self) -> list[Order]: ...


        class OrderStatus(str, Enum):
            PLACED = "placed"
            SHIPPED = auto()
            _ignore_ = ["x"]

            def label(self) -> str:
                return self.value


        class Shape(ABC):
            @abstractmethod
            def area(self) -> float: ...


        type Ids = list[int]

        MAX_LINES: Final = 50
        default_status = OrderStatus.PLACED


        def make_order(name: str) -> Order:
            return Order.create(name)
        """";

    private const string ControllerSource = """
        from shop.services import OrderService, audit
        from shop import mediator as bus


        class OrdersController:
            def __init__(self, orders: OrderService, services: "Services"):
                self.orders = orders
                self.services = services

            def get_orders(self, status: str, page_no: int, page_size: int = 20):
                return self.orders.list(parse_status(status, "status"), page_no, page_size=page_size)

            def place_order(self, request: PlaceOrderRequest):
                command = PlaceOrderCommand(customer_id=request.customer_id())
                result = Receipt() if self.flag() else command
                audit(result)
                self.orders.place(command)
                self.services.mediator.send(command)
                bus.publish(command)
                return created(self.orders.place(command))

            def each(self, items: list[Order]):
                for item in items:
                    self.log(item)
                list(map(lambda o: self.log(o), items))

            def flag(self) -> bool:
                return True

            def log(self, value: object) -> None:
                pass


        class Services:
            mediator: Mediator


        def parse_status(value: str, name: str) -> OrderStatus:
            return OrderStatus(value)


        def created(body: object) -> Response:
            return Response(body, status_code=201)
        """;

    private const string ServicesSource = """
        class OrderService:
            def list(self, status, page_no, page_size=10) -> list[OrderDto]: ...

            def place(self, command: PlaceOrderCommand) -> OrderDto: ...


        def audit(value) -> None: ...
        """;

    private const string LiteralSource = """
        SCHEMA = "sales"
        QUERY = '''select *
          from orders'''
        BASE = "/api/"


        class Setup:
            def configure(self, order_id: int):
                use("orders", 'a\tb\x41', r"c:\temp", -1, 18.4, True, None, b"raw", "ab" "cd")
                use(SCHEMA, "a" + "b", f"{BASE}orders/{order_id}", f"{{literal}}", f"{order_id!r:>10}")
                request = Request("POST", f"{BASE}orders")
                send(request)
                url = BASE + "items/"
                fetch(url.rstrip("/"))
        """;

    private const string UnitSource = """
        class Units:
            def find(self, id: int) -> int:
                later = lambda: None
                for o in orders:
                    o.status = "PAID"

                def helper():
                    return 1

                self.total = id
                self.count += 1
                return id
        """;

    private const string SyntaxSource = """
        import asyncio, os.path as osp
        from typing import (
            Any,
            Generic,
        )

        counter = 0


        def handle(command, *, retries=3):
            global counter
            match command:
                case {"action": "move", "to": [x, y]}:
                    return x, y
                case Point(x=0, y=0) | Origin() if retries > 0:
                    return None
                case [first, *rest]:
                    return first
                case str() as text:
                    return text
                case _:
                    raise ValueError(f"unknown {command!r}") from None


        async def run(items: list[int]) -> dict[str, int]:
            async with open("a") as a, open("b") as b:
                pass
            with (open("c") as c,
                  open("d") as d):
                pass
            try:
                result = {k: v for k, v in zip("ab", items) if v}
            except* (ValueError, TypeError) as error:
                result = {}
            else:
                pass
            finally:
                counter += 1
            async for item in stream():
                if (n := len(item)) > 10: continue
            values = [*items, *[1, 2]]; total = sum(x ** 2 for x in values)
            nested = f"{result["a"]:{width}}" \
                f"{'x' if total else 'y'}"
            return {**result, "n": total}


        class Box[T](Generic[T]):
            def get[U: int = 0](self, default: U) -> T | U: ...
        """;

    private static readonly CodeGraph Graph = ReadAll(("shop/__init__.py", ""), ("shop/domain/__init__.py", ""), ("shop/domain/orders.py", Source));

    private static readonly CodeGraph Bodies = ReadAll(
        ("shop/__init__.py", ""), ("shop/api/__init__.py", ""), ("shop/api/orders.py", ControllerSource), ("shop/services.py", ServicesSource), ("shop/mediator.py", "def publish(message) -> None: ..."));

    private static readonly CodeGraph Literals = ReadAll(("setup.py", LiteralSource));

    private static readonly CodeGraph Units = ReadAll(("units.py", UnitSource));

    [Fact]
    public void A_file_has_its_module_from_its_packages_and_its_imports_qualified()
    {
        Graph.Failures.ShouldBeEmpty();
        var file = Graph.Files.Single(x => x.Path == "shop/domain/orders.py");
        file.Language.ShouldBe("python");
        file.Namespaces.ShouldBe(["shop.domain.orders"]);
        file.Imports.Select(x => x.Target).ShouldBe([
            "__future__.annotations", "uuid", "dataclasses.dataclass", "dataclasses.field", "decimal.Decimal", "enum.Enum", "enum.auto",
            "typing.Annotated", "typing.Any", "typing.ClassVar", "typing.Final", "typing.Optional", "typing.Protocol", "shop.common", "shop.domain.base.AggregateRoot"
        ]);
        file.Imports.Single(x => x.Wildcard).Target.ShouldBe("shop.common");
        file.Imports[^1].Alias.ShouldBe("Root");
        file.Imports[^1].Module.ShouldBe(".base");
        file.Imports.ShouldAllBe(x => x.File == null);
        Graph.Files.Single(x => x.Path == "shop/domain/__init__.py").Namespaces.ShouldBe(["shop.domain"]);
    }

    [Fact]
    public void A_module_without_packages_is_named_by_what_imports_it_and_an_import_has_the_file_it_reaches()
    {
        var graph = ReadAll(
            ("src/shop/domain/order.py", "class Order: ..."),
            ("src/shop/api/orders.py", "from shop.domain.order import Order\nimport shop.domain.order as orders\n\nclass Controller:\n    order: Order\n    other: orders.Order\n"));
        graph.Files.Single(x => x.Path == "src/shop/domain/order.py").Namespaces.ShouldBe(["shop.domain.order"]);
        var imports = graph.Files.Single(x => x.Path == "src/shop/api/orders.py").Imports;
        imports.Select(x => x.File).ShouldBe(["src/shop/domain/order.py", "src/shop/domain/order.py"]);
        graph.TypeOf(imports[0])!.Name.ShouldBe("Order");

        var controller = graph.Types.Single(x => x.Name == "Controller");
        graph.TypeFor(Member(controller, "order").Type!, controller)!.Namespace.ShouldBe("shop.domain.order");
        graph.TypeFor(Member(controller, "other").Type!, controller)!.Namespace.ShouldBe("shop.domain.order");
    }

    [Fact]
    public void A_class_has_its_kind_access_decorators_and_bases()
    {
        var order = Type("Order");
        order.Kind.ShouldBe("class");
        order.Namespace.ShouldBe("shop.domain.orders");
        order.Access.ShouldBe("public");
        order.At.Line.ShouldBe(14);
        order.Attributes.Single().Name.ShouldBe("dataclass");
        order.Attributes.Single().Named["frozen"].ShouldBe("true");
        order.BaseTypes.Select(x => x.Text).ShouldBe(["Root"]);

        // Protocol and ABC say the type's kind, not a base it has.
        Type("OrderRepository").Kind.ShouldBe("interface");
        Type("OrderRepository").BaseTypes.ShouldBeEmpty();
        Type("Shape").IsAbstract.ShouldBeTrue();
        Type("Shape").BaseTypes.ShouldBeEmpty();
        Type("Line").Namespace.ShouldBe("shop.domain.orders");
        Type("Ids").Kind.ShouldBe("typeAlias");
        Type("Ids").Aliases!.Text.ShouldBe("list[int]");
    }

    [Fact]
    public void An_enum_has_its_members_but_not_its_sunder_names_or_methods()
    {
        var status = Type("OrderStatus");
        status.Kind.ShouldBe("enum");
        status.BaseTypes.ShouldBeEmpty();
        status.Members.Where(x => x.Kind == "enumMember").Select(x => x.Name).ShouldBe(["PLACED", "SHIPPED"]);
        Member(status, "PLACED").Value!.Literal!.Value.ShouldBe("placed");
        Member(status, "label").Kind.ShouldBe("method");
    }

    [Fact]
    public void A_field_has_its_annotation_as_its_type_and_what_init_assigns_self_is_a_field_too()
    {
        var order = Type("Order");
        Member(order, "id").Type!.QualifiedName.ShouldBe("uuid.UUID");
        Member(order, "id").Type!.Name.ShouldBe("UUID");
        Member(order, "id").IsStatic.ShouldBeFalse();
        Member(order, "lines").Type!.Text.ShouldBe("list[OrderLine]");
        Member(order, "lines").Type!.Arguments.Single().Name.ShouldBe("OrderLine");
        Member(order, "total").Value!.Text.ShouldBe("Decimal(\"0\")");
        Member(order, "KIND").IsStatic.ShouldBeTrue();
        Member(order, "KIND").Type!.Name.ShouldBe("str");
        Member(order, "code").Type!.Name.ShouldBe("str");
        Member(order, "code").Attributes.Single().Named["max_length"].ShouldBe("10");
        Member(order, "_audit").Access.ShouldBe("protected");

        // Declared in the class, an attribute __init__ assigns is the class's one member.
        order.Members.Count(x => x.Name == "id").ShouldBe(1);
        Member(order, "_count").Kind.ShouldBe("field");
    }

    [Fact]
    public void An_annotation_states_nullability_with_optional_or_none_and_any_says_nothing()
    {
        var order = Type("Order");
        Member(order, "customer_name").Type!.Nullable.ShouldBe("false");
        Member(order, "delivery_to").Type!.Nullable.ShouldBe("true");
        Member(order, "reorder_level").Type!.Nullable.ShouldBe("true");
        Member(order, "reorder_level").Type!.Union.Select(x => x.Text).ShouldBe(["int", "None"]);
        Member(order, "notes").Type!.Nullable.ShouldBe("true");
        Member(order, "_audit").Type!.Nullable.ShouldBe("unknown");
    }

    [Fact]
    public void A_method_has_its_parameters_without_self_and_its_decorators_say_what_it_is()
    {
        var order = Type("Order");
        var constructor = order.Members.Single(x => x.Kind == "constructor");
        constructor.Name.ShouldBe("Order");
        constructor.Parameters.Select(x => x.Name).ShouldBe(["id", "customer_name", "tags", "options"]);
        constructor.Parameters[2].Type!.IsArray.ShouldBeTrue();
        constructor.Parameters[2].IsOptional.ShouldBeTrue();
        constructor.Parameters[3].IsOptional.ShouldBeTrue();

        var sorted = Member(order, "sorted");
        sorted.TypeParameters.ShouldBe(["T"]);
        sorted.Parameters.Select(x => x.Name).ShouldBe(["entry", "limit", "reverse"]);
        sorted.Parameters[0].Type!.Text.ShouldBe("dict[str, list[int]]");
        sorted.Parameters[1].Default.ShouldBe("10");
        sorted.Type!.Text.ShouldBe("list[T]");

        // A property's setter is the property's.
        order.Members.Single(x => x.Name == "count").Kind.ShouldBe("property");
        Member(order, "count").Type!.Name.ShouldBe("int");
        Member(order, "create").IsStatic.ShouldBeTrue();
        Member(order, "create").Parameters.Single().Name.ShouldBe("name");
        Member(order, "empty").IsStatic.ShouldBeTrue();
        Member(order, "empty").Modifiers.ShouldBe(["classmethod"]);
        Member(order, "empty").Parameters.ShouldBeEmpty();
        Member(order, "place").Modifiers.ShouldBe(["async"]);
        Member(order, "place").Body!.IsAsync.ShouldBeTrue();
        Member(order, "__mangle").Access.ShouldBe("private");
        Member(Type("Shape"), "area").Modifiers.ShouldBe(["abstract"]);
    }

    [Fact]
    public void A_module_is_a_type_of_its_functions_and_variables()
    {
        var module = Graph.Types.Single(x => x.Kind == "module");
        module.Name.ShouldBe("orders"); // as Python names the module, without the file's extension
        module.Namespace.ShouldBe("shop.domain.orders");
        module.Members.Select(x => x.Name).ShouldBe(["MAX_LINES", "default_status", "make_order"]);
        module.Members.ShouldAllBe(x => x.IsStatic);
        Member(module, "MAX_LINES").Modifiers.ShouldBe(["final"]);
        Member(module, "MAX_LINES").Value!.Literal!.Value.ShouldBe("50");
        Member(module, "make_order").Kind.ShouldBe("method");

        // A module that declares nothing outside its classes is no type.
        Graph.Types.Count(x => x.Kind == "module").ShouldBe(1);
    }

    [Fact]
    public void A_body_has_its_calls_with_what_they_are_made_on_and_their_keyword_arguments()
    {
        Bodies.Failures.ShouldBeEmpty();
        var calls = Member(Body("OrdersController"), "get_orders").Calls;
        calls.Select(x => x.Name).ShouldBe(["list", "parse_status"]);

        var list = calls[0];
        list.Receiver.ShouldBe("self.orders");
        list.ReceiverType!.Name.ShouldBe("OrderService");
        list.Target!.Type.Name.ShouldBe("OrderService");
        list.Arguments.Select(x => x.Text).ShouldBe(["parse_status(status, \"status\")", "page_no", "page_size"]);
        list.Arguments.Select(x => x.Name).ShouldBe([null, null, "page_size"]);
        list.Arguments[1].Types.Single().Name.ShouldBe("int");

        // A function called by its name is the module's, never the class's own method.
        var parse = calls[1];
        parse.Receiver.ShouldBeNull();
        parse.Target!.Type.Kind.ShouldBe("module");
        parse.Target.Member.Name.ShouldBe("parse_status");
        parse.At.Line.ShouldBe(11);
    }

    [Fact]
    public void A_value_is_typed_by_its_creation_local_conditional_and_attribute_path_and_a_call_to_a_class_is_a_creation()
    {
        var place = Member(Body("OrdersController"), "place_order");
        place.Calls.Select(x => x.Name).ShouldBe(["customer_id", "flag", "audit", "place", "send", "publish", "created", "place"]);
        place.Calls[0].ReceiverType!.Name.ShouldBe("PlaceOrderRequest");
        place.Calls[1].Receiver.ShouldBeNull();
        place.Calls[1].Target!.Type.Name.ShouldBe("OrdersController");

        // A function another module declares, reached through its import.
        place.Calls[2].Target!.Member.Name.ShouldBe("audit");
        place.Calls[2].Arguments.Single().Types.Select(x => x.Name).ShouldBe(["Receipt", "PlaceOrderCommand"]);
        place.Calls[3].ReceiverType!.Name.ShouldBe("OrderService");
        place.Calls[3].Arguments.Single().Types.Single().Name.ShouldBe("PlaceOrderCommand");

        // services is annotated "Services", a forward reference: its mediator is a Mediator.
        place.Calls[4].ReceiverType!.Name.ShouldBe("Mediator");
        place.Calls[5].Receiver.ShouldBe("bus");
        place.Calls[5].Target!.Type.Namespace.ShouldBe("shop.mediator");
        place.Calls[6].Arguments.Single().Text.ShouldBe("self.orders.place(command)");
        place.Calls[6].Arguments.Single().Types.Single().Name.ShouldBe("OrderDto");

        place.Creates.Select(x => x.Name).ShouldBe(["PlaceOrderCommand", "Receipt"]);
        place.Creations[0].Arguments.Single().Name.ShouldBe("customer_id");
        place.Calls[3].Arguments.Single().Creations.Single().ShouldBeSameAs(place.Creations[0]);
    }

    [Fact]
    public void A_loop_target_and_a_lambda_parameter_are_untyped_and_a_lambda_is_a_unit()
    {
        var each = Member(Body("OrdersController"), "each");
        var logs = each.Calls.Where(x => x.Name == "log").ToList();
        logs.Count.ShouldBe(2);
        logs.ShouldAllBe(x => !x.Arguments.Single().IsKnown);

        var lambda = each.Calls.Single(x => x.Name == "map").Arguments[0].Function!;
        lambda.Parameters.Single().Name.ShouldBe("o");
        lambda.Parameters.Single().Type.ShouldBeNull();
        lambda.Returns.Single().Text.ShouldBe("self.log(o)");
        lambda.Calls.Single().ShouldBeSameAs(logs[1]);
    }

    [Fact]
    public void A_creation_has_its_keyword_arguments_with_their_literals()
    {
        var created = Member(Bodies.Types.Single(x => x is { Kind: "module", Namespace: "shop.api.orders" }), "created");
        var response = created.Creations.Single();
        response.Type.Name.ShouldBe("Response");
        response.Arguments.Select(x => x.Name).ShouldBe([null, "status_code"]);
        response.Arguments[1].Literal!.Value.ShouldBe("201");
        created.Body!.Returns.Single().Creations.Single().ShouldBeSameAs(response);
    }

    [Fact]
    public void An_import_binds_the_in_scope_files_it_reaches()
    {
        var imports = Bodies.Files.Single(x => x.Path == "shop/api/orders.py").Imports;
        imports.Select(x => x.Target).ShouldBe(["shop.services.OrderService", "shop.services.audit", "shop.mediator"]);
        imports.Select(x => x.File).ShouldBe(["shop/services.py", "shop/services.py", "shop/mediator.py"]);
        imports[2].Alias.ShouldBe("bus");
        Bodies.TypeOf(imports[0])!.Name.ShouldBe("OrderService");
    }

    [Fact]
    public void A_literal_argument_has_its_value_with_its_escapes_resolved_and_its_neighbours_joined()
    {
        var uses = LiteralCalls().Where(x => x.Name == "use").ToList();
        uses[0].Arguments.Select(x => (x.Literal?.Kind, x.Literal?.Value)).ShouldBe([
            ("string", "orders"), ("string", "a\tbA"), ("string", @"c:\temp"), ("number", "-1"), ("number", "18.4"), ("boolean", "true"), ("null", null),
            (null, null), ("string", "abcd")
        ]);
        uses[1].Arguments[0].Literal.ShouldBeNull();
        uses[1].Arguments[3].Literal!.Value.ShouldBe("{literal}");
    }

    [Fact]
    public void A_string_value_follows_module_constants_concatenation_f_strings_locals_and_trims()
    {
        var uses = LiteralCalls().Where(x => x.Name == "use").ToList();
        Literals.StringOf(uses[1].Arguments[0]).Value.ShouldBe("sales");
        Literals.StringOf(uses[1].Arguments[1]).Value.ShouldBe("ab");

        var path = Literals.StringOf(uses[1].Arguments[2]);
        path.Kind.ShouldBe("interpolated");
        path.Parts.Select(x => x.Text).ShouldBe(["/api/orders/", "order_id"]);
        uses[1].Arguments[4].Literal!.Parts.Single().Text.ShouldBe("order_id");

        var fetch = LiteralCalls().Single(x => x.Name == "fetch").Arguments.Single();
        fetch.Trimmed!.End.ShouldBeTrue();
        fetch.Trimmed.Start.ShouldBeFalse();
        fetch.Trimmed.Characters.ShouldBe(['/']);
        Literals.StringOf(fetch).Value.ShouldBe("/api/items");

        var module = Literals.Types.Single(x => x.Kind == "module");
        Member(module, "QUERY").Value!.Literal!.Value.ShouldBe("select *\n  from orders");
    }

    [Fact]
    public void A_value_has_the_creation_it_is_through_a_local()
    {
        var configure = Member(Literals.Types.Single(x => x.Name == "Setup"), "configure");
        var request = configure.Creations.Single();
        request.Type.Name.ShouldBe("Request");
        request.Arguments[0].Literal!.Value.ShouldBe("POST");
        configure.Calls.Single(x => x.Name == "send").Arguments.Single().Creations.Single().ShouldBeSameAs(request);
    }

    [Fact]
    public void A_body_and_a_lambda_are_units_with_their_assignments_changes_and_own_returns()
    {
        var find = Member(Units.Types.Single(), "find").Body!;
        find.Parameters.Single().Name.ShouldBe("id");

        // Neither a nested function's return nor a lambda's is the method's.
        find.Returns.Single().Text.ShouldBe("id");

        // An attribute of self is the type's own member, by its name.
        find.Assignments.Select(x => x.Target).ShouldBe(["later", "o.status", "total"]);
        find.Assignments.Select(x => x.Receiver?.Text).ShouldBe([null, "o", null]);
        find.Assignments[1].Value.Literal!.Value.ShouldBe("PAID");
        find.Assignments[0].Value.Function!.Returns.Single().Literal!.Kind.ShouldBe("null");
        find.Changes.ShouldBe(["count"]);
    }

    [Fact]
    public void Modern_syntax_parses_without_errors()
    {
        var graph = ReadAll(("modern.py", SyntaxSource));
        graph.Failures.ShouldBeEmpty();
        PythonSyntax.Of(graph)!.Trees.Keys.ShouldBe(["modern.py"]);
        var run = Member(graph.Types.Single(x => x.Kind == "module"), "run");
        run.Body!.IsAsync.ShouldBeTrue();
        run.Type!.Text.ShouldBe("dict[str, int]");
        graph.Types.Single(x => x.Name == "Box").TypeParameters.ShouldBe(["T"]);
        Member(graph.Types.Single(x => x.Name == "Box"), "get").Type!.Union.Select(x => x.Text).ShouldBe(["T", "U"]);
    }

    [Fact]
    public void A_statement_that_cannot_be_read_is_reported_and_the_rest_of_the_file_is_still_read()
    {
        var graph = ReadAll(("broken.py", "def broken(:\n    pass\n\n\nclass After:\n    name: str\n"));
        graph.Failures.Single().File.ShouldBe("broken.py");
        graph.Failures.Single().Error.ShouldContain("line 1:");
        Member(graph.Types.Single(x => x.Name == "After"), "name").Type!.Name.ShouldBe("str");
    }

    private static IReadOnlyList<CallNode> LiteralCalls() => Literals.Types.Single(x => x.Name == "Setup").Members.Single(x => x.Name == "configure").Calls;

    private static TypeNode Body(string name) => Bodies.Types.Single(x => x.Name == name);

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    private static CodeGraph ReadAll(params (string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-python-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            new PythonExtension().Languages.Single().Read(root, files.Select(x => x.Path).ToList(), graph, TestContext.Current.CancellationToken);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

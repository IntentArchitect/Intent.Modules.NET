using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.Protobuf;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The facts the Protobuf language gives the graph: a package as the namespace, messages, enums and services as types,
/// fields as properties numbered by their value, rpcs as methods, options as annotations, imports resolved to in-scope
/// files, and nullability as a field's presence, from proto2, proto3 and editions.
/// </summary>
public class ProtobufFactsTests
{
    private static readonly (string Path, string Source)[] Files =
    [
        ("Protos/common.proto", """
            syntax = "proto3";

            package shop.common;

            message Money {
              string currency = 1;
              int64 units = 2;
            }
            """),
        ("Protos/orders.proto", """
            // Orders, as the API serves them.
            syntax = "proto3";

            option csharp_namespace = "Shop.Grpc";

            import "google/protobuf/wrappers.proto";
            import "google/protobuf/empty.proto";
            import "common.proto";
            import "google/api/annotations.proto";

            package shop.orders;

            service Orders {
              rpc GetOrder(GetOrderRequest) returns (OrderReply) {
                option (google.api.http) = {
                  get: "/v1/orders/{order_id}"
                  additional_bindings { get: "/v2/orders/{order_id}" }
                };
              }
              rpc Upload(stream OrderReply) returns (google.protobuf.Empty);
              rpc Watch(GetOrderRequest) returns (stream OrderReply) {}
            }

            message GetOrderRequest {
              string order_id = 1;
            }

            message OrderReply {
              string id = 1;
              optional string note = 2;
              repeated Line lines = 3;
              map<string, int32> totals = 4;
              shop.common.Money total = 5;
              Status status = 6;
              google.protobuf.Int32Value discount = 7;
              acme.Rating rating = 8;
              bytes   receipt = 9 [deprecated = true, json_name = "r"]; // as sent
              oneof payment {
                string card = 10;
                Status voucher = 11;
              }

              message Line {
                int32 product_id = 1;
                Line.Kind kind = 2;

                enum Kind {
                  KIND_UNSPECIFIED = 0;
                  KIND_GIFT = 0x10;
                }
              }
            }

            enum Status {
              option allow_alias = true;
              STATUS_UNSPECIFIED = 0;
              STATUS_PAID = 1;
              STATUS_SETTLED = 1 [deprecated = true];
              STATUS_LOST = -1;
            }
            """),
        ("legacy.proto", """
            package legacy;

            message Search {
              required string query = 1;
              optional int32 page = 2 [default = 10];
              optional Kind kind = 3;
              repeated group Result = 4 {
                required string url = 5;
              }
              extensions 100 to 199;

              enum Kind { KIND_ALL = 0; }
            }

            extend Search {
              optional bool fuzzy = 100;
            }
            """),
        ("editions.proto", """
            edition = "2023";

            package ed;

            option features.field_presence = IMPLICIT;

            message Plain {
              int32 count = 1;
              Plain next = 2;
              int32 kept = 3 [features.field_presence = EXPLICIT];
            }

            message Explicit {
              option features.field_presence = EXPLICIT;
              int32 count = 1;
              int32 must = 2 [features.field_presence = LEGACY_REQUIRED];
            }
            """),
        ("broken.proto", """
            syntax = "proto3";
            package broken;

            message First {
              string name = ;
              int32 count = 2;
            }

            message Second {
              string id = 1;
            }
            """)
    ];

    private static readonly CodeGraph Graph = ReadAll(Files);

    [Fact]
    public void A_package_is_the_namespace_of_its_file_and_its_types()
    {
        var file = Graph.Files.Single(x => x.Path == "Protos/orders.proto");
        file.Language.ShouldBe("protobuf");
        file.Namespaces.ShouldBe(["shop.orders"]);

        var reply = Type("OrderReply");
        reply.Kind.ShouldBe("message");
        reply.Namespace.ShouldBe("shop.orders");
        reply.Files.ShouldBe(["Protos/orders.proto"]);
        Type("Status").Kind.ShouldBe("enum");
        Type("Orders").Kind.ShouldBe("service");

        // A nested message or enum is named as written, in its file's package.
        Type("Line").Namespace.ShouldBe("shop.orders");
        Type("Kind", "Protos/orders.proto").Kind.ShouldBe("enum");
    }

    [Fact]
    public void An_import_names_its_file_and_resolves_to_the_in_scope_one_it_means()
    {
        var imports = Graph.Files.Single(x => x.Path == "Protos/orders.proto").Imports;
        imports.Select(x => x.Module).ShouldBe(["google/protobuf/wrappers.proto", "google/protobuf/empty.proto", "common.proto", "google/api/annotations.proto"]);

        // common.proto is found beside the file that imports it; its package is what the import makes visible.
        var common = imports.Single(x => x.Module == "common.proto");
        common.File.ShouldBe("Protos/common.proto");
        common.Target.ShouldBe("shop.common");
        common.Wildcard.ShouldBeTrue();
        common.Text.ShouldBe("import \"common.proto\";");

        var wellKnown = imports.Single(x => x.Module == "google/protobuf/empty.proto");
        wellKnown.File.ShouldBeNull();
        wellKnown.Target.ShouldBe("google/protobuf/empty.proto");
    }

    [Fact]
    public void A_files_syntax_and_options_are_kept_beside_the_graph()
    {
        var syntax = ProtobufSyntax.Of(Graph)!;
        var orders = syntax.Files["Protos/orders.proto"];
        orders.Syntax.ShouldBe("proto3");
        orders.Package.ShouldBe("shop.orders");
        orders.Options.Single().Name.ShouldBe("csharp_namespace");
        orders.Options.Single().Arguments.ShouldBe(["Shop.Grpc"]);

        syntax.Files["legacy.proto"].Syntax.ShouldBe("proto2");
        syntax.Files["editions.proto"].Syntax.ShouldBe("editions");
        syntax.Files["editions.proto"].Edition.ShouldBe("2023");
    }

    [Fact]
    public void A_field_is_a_property_with_its_type_as_written_and_its_number_as_its_value()
    {
        var reply = Type("OrderReply");
        reply.Members.Select(x => x.Name).ShouldBe(["id", "note", "lines", "totals", "total", "status", "discount", "rating", "receipt", "card", "voucher"]);
        reply.Members.ShouldAllBe(x => x.Kind == "property");

        var id = Member(reply, "id");
        id.Type!.Name.ShouldBe("string");
        id.Type.IsPredefinedValueType.ShouldBeFalse();
        id.Value!.Literal!.Kind.ShouldBe("number");
        id.Value.Literal.Value.ShouldBe("1");

        var total = Member(reply, "total").Type!;
        total.Text.ShouldBe("shop.common.Money");
        total.Name.ShouldBe("Money");
        total.QualifiedName.ShouldBe("shop.common.Money");
        total.Language.ShouldBe("protobuf");

        Member(Type("Line"), "product_id").Type!.IsPredefinedValueType.ShouldBeTrue();
        Member(reply, "note").Modifiers.ShouldBe(["optional"]);
    }

    [Fact]
    public void A_repeated_field_is_a_collection_and_a_map_is_map_of_its_key_and_value()
    {
        var reply = Type("OrderReply");
        var lines = Member(reply, "lines");
        lines.Modifiers.ShouldBe(["repeated"]);
        lines.Type!.IsArray.ShouldBeTrue();
        lines.Type.Text.ShouldBe("repeated Line");
        lines.Type.Name.ShouldBe("Line");
        lines.Type.Arguments.Single().Name.ShouldBe("Line");

        var totals = Member(reply, "totals").Type!;
        totals.Text.ShouldBe("map<string,int32>");
        totals.Name.ShouldBe("map");
        totals.Arguments.Select(x => x.Name).ShouldBe(["string", "int32"]);
        totals.Nullable.ShouldBe("false");
    }

    [Fact]
    public void A_proto3_field_is_nullable_when_it_has_presence()
    {
        var reply = Type("OrderReply");
        string Nullable(string field) => Member(reply, field).Type!.Nullable;

        Nullable("id").ShouldBe("false");
        Nullable("note").ShouldBe("true");
        Nullable("lines").ShouldBe("false");
        Member(reply, "lines").Type!.Arguments.Single().Nullable.ShouldBe("false");

        // A message has presence, a wrapper included; an enum, resolved from the package's scope, hasn't.
        Nullable("total").ShouldBe("true");
        Nullable("discount").ShouldBe("true");
        Nullable("status").ShouldBe("false");

        // A oneof's fields have presence, whatever their type.
        Nullable("card").ShouldBe("true");
        Nullable("voucher").ShouldBe("true");

        // A library's type may be a message or an enum.
        Nullable("rating").ShouldBe("unknown");

        // A nested enum, named through the message that holds it.
        Member(Type("Line"), "kind").Type!.Nullable.ShouldBe("false");
    }

    [Fact]
    public void A_proto2_field_is_nullable_unless_required_and_a_group_is_a_message_and_its_field()
    {
        var search = Type("Search");
        search.Members.Select(x => x.Name).ShouldBe(["query", "page", "kind", "result"]);
        Member(search, "query").Type!.Nullable.ShouldBe("false");
        Member(search, "query").Modifiers.ShouldBe(["required"]);
        Member(search, "page").Type!.Nullable.ShouldBe("true");
        Member(search, "kind").Type!.Nullable.ShouldBe("true");

        var result = Member(search, "result");
        result.Type!.IsArray.ShouldBeTrue();
        result.Type.Name.ShouldBe("Result");
        result.Value!.Literal!.Value.ShouldBe("4");
        Member(Type("Result"), "url").Type!.Nullable.ShouldBe("false");

        // An option is an annotation: [default = 10].
        var page = Member(search, "page").Attributes.Single();
        page.Name.ShouldBe("default");
        page.Arguments.ShouldBe(["10"]);
    }

    [Fact]
    public void An_editions_field_has_the_presence_its_features_give_it()
    {
        Member(Type("Plain"), "count").Type!.Nullable.ShouldBe("false");
        Member(Type("Plain"), "next").Type!.Nullable.ShouldBe("true");
        Member(Type("Plain"), "kept").Type!.Nullable.ShouldBe("true");
        Member(Type("Explicit"), "count").Type!.Nullable.ShouldBe("true");
        Member(Type("Explicit"), "must").Type!.Nullable.ShouldBe("false");
    }

    [Fact]
    public void An_enum_value_is_an_enum_member_with_its_number_and_options()
    {
        var status = Type("Status");
        status.Members.Select(x => x.Name).ShouldBe(["STATUS_UNSPECIFIED", "STATUS_PAID", "STATUS_SETTLED", "STATUS_LOST"]);
        status.Members.ShouldAllBe(x => x.Kind == "enumMember");
        status.Members.Select(x => x.Value!.Literal!.Value).ShouldBe(["0", "1", "1", "-1"]);
        Member(status, "STATUS_SETTLED").Attributes.Single().Name.ShouldBe("deprecated");
        status.Attributes.Single().Name.ShouldBe("allow_alias");
        Member(Type("Kind", "Protos/orders.proto"), "KIND_GIFT").Value!.Literal!.Value.ShouldBe("0x10");
    }

    [Fact]
    public void An_rpc_is_a_method_taking_its_request_and_a_stream_is_stream_of_its_message()
    {
        var orders = Type("Orders");
        orders.Members.Select(x => x.Name).ShouldBe(["GetOrder", "Upload", "Watch"]);
        orders.Members.ShouldAllBe(x => x.Kind == "method");

        var get = Member(orders, "GetOrder");
        get.Parameters.Single().Name.ShouldBe("request");
        get.Parameters.Single().Type!.Name.ShouldBe("GetOrderRequest");
        get.Parameters.Single().Type!.Nullable.ShouldBe("false");
        get.Type!.Name.ShouldBe("OrderReply");

        var upload = Member(orders, "Upload");
        upload.Parameters.Single().Type!.Text.ShouldBe("stream OrderReply");
        upload.Parameters.Single().Type!.Name.ShouldBe("stream");
        upload.Parameters.Single().Type!.Arguments.Single().Name.ShouldBe("OrderReply");
        upload.Type!.QualifiedName.ShouldBe("google.protobuf.Empty");

        var watch = Member(orders, "Watch");
        watch.Parameters.Single().Type!.Name.ShouldBe("GetOrderRequest");
        watch.Type!.Name.ShouldBe("stream");
        watch.Type.Arguments.Single().Name.ShouldBe("OrderReply");
    }

    [Fact]
    public void An_option_is_an_annotation_and_a_message_value_has_its_fields_as_items()
    {
        var receipt = Member(Type("OrderReply"), "receipt");
        receipt.Attributes.Select(x => x.Name).ShouldBe(["deprecated", "json_name"]);
        receipt.Attributes.Select(x => x.Arguments.Single()).ShouldBe(["true", "r"]);

        var http = Member(Type("Orders"), "GetOrder").Attributes.Single();
        http.Name.ShouldBe("(google.api.http)");
        var value = http.Values.Single();
        value.Items.Select(x => x.Name).ShouldBe(["get", "additional_bindings"]);
        value.Items[0].Literal.ShouldBe("/v1/orders/{order_id}");
        value.Items[1].Items.Single().Name.ShouldBe("get");
        value.Items[1].Items.Single().Literal.ShouldBe("/v2/orders/{order_id}");
    }

    [Fact]
    public void Canonical_text_is_the_tokens_without_comments_and_a_node_is_placed_where_it_is_written()
    {
        var receipt = Member(Type("OrderReply"), "receipt");
        receipt.Text.ShouldBe("bytes receipt=9[deprecated=true,json_name=\"r\"];");
        receipt.At.File.ShouldBe("Protos/orders.proto");
        receipt.At.Line.ShouldBe(37);
        receipt.At.Column.ShouldBe(3);

        var request = Type("GetOrderRequest");
        request.Text.ShouldBe("message GetOrderRequest{string order_id=1;}");
        request.At.Line.ShouldBe(24);
        request.At.EndLine.ShouldBe(26);
    }

    [Fact]
    public void A_syntax_error_is_a_failure_of_its_file_and_the_rest_of_the_file_is_read()
    {
        Graph.Failures.Single().File.ShouldBe("broken.proto");
        Graph.Failures.Single().Error.ShouldBe("line 5:17 expected a number, found ';'");

        Type("First").Members.Select(x => x.Name).ShouldBe(["count"]);
        Type("Second").Members.Select(x => x.Name).ShouldBe(["id"]);
    }

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static TypeNode Type(string name, string file) => Graph.Types.Single(x => x.Name == name && x.Files.Contains(file));

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    private static CodeGraph ReadAll((string Path, string Source)[] files)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-proto-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            new ProtobufExtension().Languages.Single().Read(root, files.Select(x => x.Path).ToList(), graph);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}

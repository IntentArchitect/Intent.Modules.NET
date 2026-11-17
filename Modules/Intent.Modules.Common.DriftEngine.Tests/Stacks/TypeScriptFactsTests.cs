using Intent.Modules.Common.DriftEngine.Code;
using Intent.Modules.Common.DriftEngine.Stacks.TypeScript;

namespace Intent.Modules.Common.DriftEngine.Tests.Stacks;

/// <summary>
/// The facts the TypeScript language gives the graph, from TypeScript's own parser run on Node.js: modules as namespaces,
/// what a module declares outside its types as a <c>module</c> type, an object literal <c>const</c> as an <c>object</c>
/// type, a Vue single-file component's script as a <c>component</c>, imports resolved through <c>tsconfig.json</c>, and
/// nullability as its <c>strict</c> says.
/// </summary>
public class TypeScriptFactsTests
{
    private static readonly (string Path, string Source)[] Files =
    [
        ("tsconfig.json", """
            {
              // create-vue's options, comments and all.
              "compilerOptions": { "strict": true, "baseUrl": ".", "paths": { "@/*": ["src/*"] }, },
            }
            """),
        ("src/api/types.ts", """
            export interface ProductDto {
              id: string;
              name: string;
              description?: string;
              tags: readonly string[];
              category: Category | null;
            }

            export type Status = 'Draft' | 'Paid';

            export type Paged<T> = PageInfo & { items: T[] };

            export interface PageInfo { total: number }

            export enum Category { Food = 'food', Toys = 4 }

            export class ApiError extends Error {
              constructor(public readonly status: number, message?: string) {
                super(message);
              }

              get isNotFound(): boolean {
                return this.status === 404;
              }
            }
            """),
        ("src/api/client.ts", """
            import axios from 'axios';
            import type { ProductDto, Paged } from './types';
            import { ApiError as Failure } from '@/api/types';
            import * as types from './types';

            const http = axios.create({ baseURL: '/api' });
            export const pageSize = 20;

            export async function getProducts(page = 1, filter?: string): Promise<Paged<ProductDto>> {
              const response = await http.get<Paged<ProductDto>>(`/products?page=${page}`);
              return response.data;
            }

            export const deleteProduct = async (id: string): Promise<void> => {
              await http.delete(`/products/${id}`);
            };

            export const api = {
              getProducts,
              async getProduct(id: string): Promise<ProductDto> {
                const page = await getProducts();
                return page.items[0];
              },
              status: 'Draft' as types.Status,
            };

            @Injectable({ providedIn: 'root' })
            export class ProductService {
              private readonly cache = new Map<string, ProductDto>();

              constructor(private readonly client: Client) {}

              @Cached({ ttl: 60 })
              async find(id: string): Promise<ProductDto | undefined> {
                const found = this.cache.get(id);
                this.client.send('find', id);
                return found ?? (await api.getProduct(id));
              }
            }

            export class Client {
              send(name: string, ...args: unknown[]): void {}
            }
            """),
        ("src/components/ProductCard.vue", """
            <template>
              <div @click="select">{{ product?.name }}</div>
            </template>

            <script setup lang="ts">
            import { ref, onMounted } from 'vue';
            import type { ProductDto } from '@/api/types';
            import { api } from '../api/client';

            const props = withDefaults(defineProps<{ id: string; compact?: boolean }>(), { compact: false });
            const emit = defineEmits<{ (e: 'selected', id: string): void; (e: 'closed'): void }>();
            const quantity = defineModel<number>('quantity');

            const product = ref<ProductDto | null>(null);

            function select() {
              emit('selected', props.id);
            }

            onMounted(async () => {
              product.value = await api.getProduct(props.id);
            });
            </script>
            """),
        ("src/App.vue", """
            <script setup lang="ts">
            import ProductCard from './components/ProductCard.vue';
            </script>

            <template>
              <ProductCard id="1" />
            </template>
            """),
        ("legacy/tsconfig.json", """{ "compilerOptions": { "target": "es5" } }"""),
        ("legacy/old.ts", "export interface Old { name: string; note?: string }"),
        ("src/broken.ts", "export class { x: }")
    ];

    private static readonly CodeGraph Graph = ReadAll(Files);

    [Fact]
    public void A_module_is_a_namespace_named_by_its_path_and_declares_its_types()
    {
        var dto = Type("ProductDto");
        dto.Kind.ShouldBe("interface");
        dto.Namespace.ShouldBe("src/api/types");
        dto.Access.ShouldBe("public");
        dto.Files.ShouldBe(["src/api/types.ts"]);

        var file = Graph.Files.Single(x => x.Path == "src/api/types.ts");
        file.Language.ShouldBe("typescript");
        file.Namespaces.ShouldBe(["src/api/types"]);

        var category = Type("Category");
        category.Kind.ShouldBe("enum");
        category.Members.Select(x => x.Value!.Literal!.Value).ShouldBe(["food", "4"]);
    }

    [Fact]
    public void A_type_alias_has_what_it_names_and_an_object_types_members()
    {
        var status = Type("Status");
        status.Kind.ShouldBe("typeAlias");
        status.Aliases!.Union.Select(x => x.Literal!.Value).ShouldBe(["Draft", "Paid"]);

        var paged = Type("Paged");
        paged.TypeParameters.ShouldBe(["T"]);
        paged.BaseTypes.Single().Name.ShouldBe("PageInfo");
        Member(paged, "items").Type!.IsArray.ShouldBeTrue();
        Member(paged, "items").Type!.Name.ShouldBe("T");
    }

    [Fact]
    public void A_class_has_its_bases_its_parameter_properties_and_its_accessors()
    {
        var error = Type("ApiError");
        error.Kind.ShouldBe("class");
        error.BaseTypes.Single().Name.ShouldBe("Error");
        Member(error, "ApiError").Parameters.Select(x => x.Name).ShouldBe(["status", "message"]);
        Member(error, "ApiError").Parameters[1].IsOptional.ShouldBeTrue();

        var status = Member(error, "status");
        status.Kind.ShouldBe("property");
        status.Access.ShouldBe("public");
        status.Modifiers.ShouldBe(["readonly"]);
        status.Type!.Name.ShouldBe("number");

        var notFound = Member(error, "isNotFound");
        notFound.Kind.ShouldBe("property");
        notFound.Modifiers.ShouldBe(["get"]);
        notFound.Type!.Name.ShouldBe("boolean");
        notFound.Body!.Returns.Single().Text.ShouldBe("this.status===404");
    }

    [Fact]
    public void A_type_is_nullable_when_it_is_optional_or_a_union_with_null_under_strict_and_unknown_without_it()
    {
        var dto = Type("ProductDto");
        Member(dto, "name").Type!.Nullable.ShouldBe("false");
        Member(dto, "description").Type!.Nullable.ShouldBe("true");
        Member(dto, "category").Type!.Nullable.ShouldBe("true");
        Member(dto, "category").Type!.Union.Select(x => x.Text).ShouldBe(["Category", "null"]);
        Member(dto, "tags").Type!.IsArray.ShouldBeTrue();
        Member(dto, "tags").Type!.Name.ShouldBe("string");

        // legacy/tsconfig.json is the nearest, and turns on neither strict nor strictNullChecks.
        var old = Type("Old");
        Member(old, "name").Type!.Nullable.ShouldBe("unknown");
        Member(old, "note").Type!.Nullable.ShouldBe("unknown");
    }

    [Fact]
    public void What_a_module_declares_outside_its_types_is_its_module_type()
    {
        var client = Type("client.ts");
        client.Kind.ShouldBe("module");
        client.Namespace.ShouldBe("src/api/client");
        client.Members.Select(x => x.Name).ShouldBe(["http", "pageSize", "getProducts", "deleteProduct", "api"]);
        client.Members.ShouldAllBe(x => x.IsStatic);

        Member(client, "http").Access.ShouldBe("private");
        Member(client, "http").Modifiers.ShouldBe(["const"]);
        Member(client, "pageSize").Access.ShouldBe("public");
        Member(client, "pageSize").Value!.Literal!.Value.ShouldBe("20");

        var getProducts = Member(client, "getProducts");
        getProducts.Kind.ShouldBe("method");
        getProducts.Modifiers.ShouldBe(["async"]);
        getProducts.Type!.Text.ShouldBe("Promise<Paged<ProductDto>>");
        getProducts.Parameters[0].Default.ShouldBe("1");
        getProducts.Parameters[1].IsOptional.ShouldBeTrue();
        getProducts.Parameters[1].Type!.Nullable.ShouldBe("true");

        var delete = Member(client, "deleteProduct");
        delete.Kind.ShouldBe("method");
        delete.Modifiers.ShouldBe(["async"]);
        delete.Body!.IsAsync.ShouldBeTrue();
    }

    [Fact]
    public void A_const_holding_an_object_literal_is_an_object_type()
    {
        var api = Type("api");
        api.Kind.ShouldBe("object");
        api.Namespace.ShouldBe("src/api/client");
        api.Members.Select(x => (x.Name, x.Kind)).ShouldBe([("getProducts", "property"), ("getProduct", "method"), ("status", "property")]);

        // 'Draft' as types.Status names the alias through the namespace import.
        var status = Member(api, "status").Value!;
        status.Types.Single().QualifiedName.ShouldBe("types.Status");
        Graph.TypeFor(status.Types.Single(), api).ShouldBeSameAs(Type("Status"));
    }

    [Fact]
    public void An_import_is_one_node_per_binding_resolved_through_relative_paths_and_tsconfig_paths()
    {
        var imports = Graph.Files.Single(x => x.Path == "src/api/client.ts").Imports;
        imports.Select(x => (x.Target, x.Alias, x.Module, x.File, x.TypeOnly, x.Wildcard)).ShouldBe([
            ("axios.default", "axios", "axios", null, false, false),
            ("src/api/types.ProductDto", null, "./types", "src/api/types.ts", true, false),
            ("src/api/types.Paged", null, "./types", "src/api/types.ts", true, false),
            ("src/api/types.ApiError", "Failure", "@/api/types", "src/api/types.ts", false, false),
            ("src/api/types", "types", "./types", "src/api/types.ts", false, true)
        ]);
        Graph.TypeOf(imports[1]).ShouldBeSameAs(Type("ProductDto"));
        Graph.TypeOf(imports[3]).ShouldBeSameAs(Type("ApiError"));

        // A default import names what the module's default export is: a component's is the component.
        var card = Graph.Files.Single(x => x.Path == "src/App.vue").Imports.Single();
        card.Target.ShouldBe("src/components/ProductCard.ProductCard");
        Graph.TypeOf(card).ShouldBeSameAs(Type("ProductCard"));
    }

    [Fact]
    public void A_vue_component_has_its_props_emits_model_and_setup_members()
    {
        var card = Type("ProductCard");
        card.Kind.ShouldBe("component");
        card.Namespace.ShouldBe("src/components/ProductCard");
        card.Members.Select(x => (x.Name, x.Kind)).ShouldBe([
            ("id", "property"), ("compact", "property"), ("selected", "event"), ("closed", "event"), ("quantity", "property"), ("product", "field"), ("select", "method")
        ]);

        Member(card, "id").Type!.Nullable.ShouldBe("false");
        Member(card, "compact").Type!.Nullable.ShouldBe("true");
        Member(card, "compact").Value!.Literal!.Value.ShouldBe("false");
        Member(card, "selected").Parameters.Single().Type!.Name.ShouldBe("string");
        Member(card, "closed").Parameters.ShouldBeEmpty();
        Member(card, "quantity").Type!.Name.ShouldBe("number");
        Member(card, "quantity").Modifiers.ShouldBe(["model"]);

        // Positions are the .vue file's.
        Member(card, "select").At.Line.ShouldBe(16);
    }

    [Fact]
    public void A_call_reaches_a_module_function_an_object_method_or_a_class_method_through_what_it_is_called_on()
    {
        var getProduct = Member(Type("api"), "getProduct").Body!;
        getProduct.Calls.Single().Target!.Member.ShouldBeSameAs(Member(Type("client.ts"), "getProducts"));

        var find = Member(Type("ProductService"), "find").Body!;
        var get = find.Calls.Single(x => x.Name == "get");
        get.Receiver.ShouldBe("this.cache");
        get.ReceiverType!.Text.ShouldBe("Map<string,ProductDto>");

        var send = find.Calls.Single(x => x.Name == "send");
        send.Target!.Type.ShouldBeSameAs(Type("Client"));
        send.Arguments[0].Literal!.Value.ShouldBe("find");

        find.Calls.Single(x => x.Name == "getProduct").Target!.Type.ShouldBeSameAs(Type("api"));
        find.Returns.Single().Text.ShouldBe("found??(await api.getProduct(id))");
    }

    [Fact]
    public void A_body_has_its_type_arguments_template_literals_and_awaited_values()
    {
        var getProducts = Member(Type("client.ts"), "getProducts").Body!;
        var get = getProducts.Calls.Single(x => x.Name == "get");
        get.Receiver.ShouldBe("http");
        get.TypeArguments.Single().Text.ShouldBe("Paged<ProductDto>");
        var path = get.Arguments.Single().Literal!;
        path.Kind.ShouldBe("interpolated");
        path.Parts.Select(x => x.Text).ShouldBe(["/products?page=", "page"]);
        getProducts.Returns.Single().Text.ShouldBe("response.data");

        // await gives what the promise resolves to: getProducts() returns Promise<Paged<ProductDto>>.
        var getProduct = Member(Type("api"), "getProduct").Body!;
        getProduct.Returns.Single().Text.ShouldBe("page.items[0]");
    }

    [Fact]
    public void A_decorator_is_an_attribute_with_an_object_literals_properties_as_named_arguments()
    {
        var injectable = Type("ProductService").Attributes.Single();
        injectable.Name.ShouldBe("Injectable");
        injectable.Named["providedIn"].ShouldBe("root");

        var cached = Member(Type("ProductService"), "find").Attributes.Single();
        cached.Name.ShouldBe("Cached");
        cached.Named["ttl"].ShouldBe("60");
    }

    [Fact]
    public void A_components_script_setup_code_is_its_top_level_unit()
    {
        var setup = Graph.Files.Single(x => x.Path == "src/components/ProductCard.vue").TopLevel!;
        setup.Calls.Select(x => x.Name).ShouldBe(["ref", "onMounted", "getProduct"]);
        setup.Calls[2].Target!.Type.ShouldBeSameAs(Type("api"));
        setup.Assignments.Single().Target.ShouldBe("product.value");
        setup.Calls[1].Arguments.Single().Function!.IsAsync.ShouldBeTrue();
    }

    [Fact]
    public void A_file_the_parser_cannot_read_is_a_failure_and_the_others_are_still_read()
    {
        Graph.Failures.Single().File.ShouldBe("src/broken.ts");
        Graph.Types.ShouldContain(x => x.Name == "ProductDto");
    }

    [Fact]
    public void Without_nodejs_every_file_is_a_failure_that_says_why()
    {
        var graph = ReadAll([("src/a.ts", "export const a = 1;")], new NoNode());
        graph.Failures.Single().File.ShouldBe("src/a.ts");
        graph.Failures.Single().Error.ShouldContain("Node.js");
        graph.Types.ShouldBeEmpty();
    }

    [Fact]
    public void The_worker_reads_its_request_with_the_arguments_intent_architects_nodejs_passes_before_it()
    {
        var graph = ReadAll([("src/a.ts", "export interface A { id: string }")], new IntentLikeNode());
        graph.Failures.ShouldBeEmpty();
        graph.Types.Single().Name.ShouldBe("A");
    }

    private static readonly (string Path, string Source)[] ReactFiles =
    [
        ("web/src/types.ts", """
            export interface Product { id: string; name: string }
            """),
        ("web/src/ProductCard.tsx", """
            import { memo, useCallback, useState } from 'react';
            import type { Product } from './types';

            interface ProductCardProps {
              product: Product;
              compact?: boolean;
              onSelect(id: string): void;
            }

            export const ProductCard = memo(({ product, compact = false, onSelect }: ProductCardProps) => {
              const [open, setOpen] = useState<boolean>(false);
              const select = useCallback(() => onSelect(product.id), [product]);
              return (
                <>
                  <h2 className="title">{product.name}</h2>
                  {open && <Badge label="new" />}
                </>
              );
            });

            function Badge({ label }: { label: string }) {
              return <span>{label}</span>;
            }

            export function formatName(name: string) {
              return name.trim();
            }
            """),
        ("web/src/Catalog.jsx", """
            import { Link, useNavigate } from 'react-router-dom';
            import { ProductCard as Card } from './ProductCard';

            export default function Catalog(props) {
              const navigate = useNavigate();
              function open(id) {
                navigate(`/products/${id}`);
              }

              return (
                <ul>
                  {props.items.map((p) => (
                    <li key={p.id}>
                      <Card product={p} onSelect={open} {...props.extra} />
                      <Link to={`/products/${p.id}`}>View {p.name}</Link>
                    </li>
                  ))}
                </ul>
              );
            }
            """),
        ("web/src/main.jsx", """
            import { createBrowserRouter } from 'react-router-dom';
            import Catalog from './Catalog';

            export const router = createBrowserRouter([
              { path: '/', element: <Catalog />, children: [{ path: 'products/:id', lazy: () => import('./ProductCard') }] },
            ]);
            """),
        ("web/src/cart.js", """
            import { create } from 'zustand';

            export const useCartStore = create()((set) => ({ items: [], add: (item) => set((s) => ({ items: [...s.items, item] })) }));
            """),
        ("web/src/vendor.min.js", "!function(){}();"),
    ];

    private static readonly CodeGraph ReactGraph = ReadAll(ReactFiles);

    private static TypeNode ReactType(string name) => ReactGraph.Types.Single(x => x.Name == name);

    [Fact]
    public void A_capitalised_function_returning_jsx_is_a_component_and_javascript_is_read_too()
    {
        ReactType("ProductCard").Kind.ShouldBe("component");
        ReactType("Badge").Kind.ShouldBe("component");
        ReactType("Catalog").Kind.ShouldBe("component");
        ReactType("Catalog").Files.ShouldBe(["web/src/Catalog.jsx"]);

        // A function that returns anything else is still the module's.
        Member(ReactType("ProductCard.tsx"), "formatName").Kind.ShouldBe("method");

        // A default export names the component, so a default import of its file resolves to it.
        var catalog = ReactGraph.Files.Single(x => x.Path == "web/src/main.jsx").Imports.Single(x => x.Alias == "Catalog");
        ReactGraph.TypeOf(catalog).ShouldBeSameAs(ReactType("Catalog"));

        // A minified bundle is a build's output.
        ReactGraph.Files.ShouldNotContain(x => x.Path == "web/src/vendor.min.js");
    }

    [Fact]
    public void A_components_members_are_its_props_its_state_its_functions_and_its_render_body()
    {
        var card = ReactType("ProductCard");
        card.Members.Select(x => (x.Name, x.Kind, x.Access)).ShouldBe([
            ("product", "property", "public"), ("compact", "property", "public"), ("onSelect", "property", "public"),
            ("ProductCard", "constructor", "private"), ("open", "property", "private"), ("select", "method", "private")
        ]);
        Member(card, "product").Type!.Name.ShouldBe("Product");
        Member(card, "open").Modifiers.ShouldBe(["state"]);
        Member(card, "open").Type!.Name.ShouldBe("boolean");
        Member(card, "select").Body!.Calls.Single().Name.ShouldBe("onSelect");

        // Its render body holds its hooks and what its JSX calls.
        Member(card, "ProductCard").Body!.Calls.Select(x => x.Name).ShouldBe(["useState", "useCallback", "onSelect"]);

        // JavaScript props are the names it reads from its props.
        var catalog = ReactType("Catalog");
        catalog.Members.Where(x => x.Kind == "property").Select(x => x.Name).ShouldBe(["items", "extra"]);
        Member(catalog, "open").Body!.Calls.Single().Name.ShouldBe("navigate");
    }

    [Fact]
    public void Jsx_is_markup_with_its_fragments_flattened_and_its_values_as_code()
    {
        var card = ReactGraph.Files.Single(x => x.Path == "web/src/ProductCard.tsx");
        card.Markup.Select(x => x.Tag).ShouldBe(["h2", "Badge", "span"]);
        card.Markup[0].Content!.Parts.Single().Text.ShouldBe("product.name");
        card.Markup[0].Attributes.Single().Literal!.Value.ShouldBe("title");
        ReactGraph.TypeFor(card.Markup[1].Component!, ReactType("ProductCard")).ShouldBeSameAs(ReactType("Badge"));

        var catalog = ReactGraph.Files.Single(x => x.Path == "web/src/Catalog.jsx");
        var li = catalog.Markup.Single().Children.Single();
        li.Children.Select(x => x.Tag).ShouldBe(["Card", "Link"]);

        // An aliased import's tag resolves to the component it imports.
        ReactGraph.TypeFor(li.Children[0].Component!, ReactType("Catalog")).ShouldBeSameAs(ReactType("ProductCard"));
        li.Children[0].Attributes.Select(x => x.Name).ShouldBe(["product", "onSelect", "..."]);
        li.Children[0].Attributes[1].Literal!.Parts.Single().Text.ShouldBe("open");

        var to = li.Children[1].Attributes.Single().Literal!;
        to.Kind.ShouldBe("interpolated");
        to.Parts.Select(x => x.Text).ShouldBe(["/products/", "p.id"]);
        li.Children[1].Content!.Parts.Select(x => x.Text).ShouldBe(["View", "p.name"]);

        // An element given as an attribute's value is markup of its own.
        ReactGraph.Files.Single(x => x.Path == "web/src/main.jsx").Markup.Single().Tag.ShouldBe("Catalog");
    }

    [Fact]
    public void A_call_of_what_a_call_returns_is_named_as_that_call()
    {
        var create = ReactGraph.Files.Single(x => x.Path == "web/src/cart.js").TopLevel!.Calls.Single(x => x is { Name: "create", Arguments.Count: 1 });
        create.On!.Name.ShouldBe("create");
        create.Arguments[0].Function!.Returns.Single().Members.Select(x => x.Member).ShouldBe(["items", "add"]);
    }

    private sealed class NoNode : INodeRunner
    {
        public NodeRun Run(string script, IReadOnlyList<string> arguments) => new(null, "Node.js isn't on the path.");
    }

    /// <summary>Electron run as Node.js, as Intent Architect runs a script: <c>&lt;exe&gt; &lt;script&gt; --node &lt;script&gt; ...arguments</c>.</summary>
    private sealed class IntentLikeNode : INodeRunner
    {
        public NodeRun Run(string script, IReadOnlyList<string> arguments) => MachineNodeRunner.Instance.Run(script, ["--node", script, ..arguments]);
    }

    private static TypeNode Type(string name) => Graph.Types.Single(x => x.Name == name);

    private static MemberNode Member(TypeNode type, string name) => type.Members.Single(x => x.Name == name);

    private static CodeGraph ReadAll((string Path, string Source)[] files, INodeRunner? node = null)
    {
        var root = Directory.CreateTempSubdirectory("driftengine-ts-").FullName;
        try
        {
            foreach (var (path, source) in files)
            {
                var full = Path.Combine(root, path);
                Directory.CreateDirectory(Path.GetDirectoryName(full)!);
                File.WriteAllText(full, source);
            }

            var graph = new CodeGraph();
            var language = new TypeScriptExtension(node).Languages.Single();
            language.Read(root, files.Select(x => x.Path).Where(language.Reads).ToList(), graph);
            return graph;
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
